// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Microsoft.EntityFrameworkCore;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer
{
    /// <remarks>
    /// This type is used for both, general data access and transaction scopes.
    /// * In the general case, each method call uses a fresh, transient DB context. Before a method call
    ///   returns, changes are saved and the DB context is disposed.
    /// * In a transaction scope, the transaction provides us with a single, permanent DB context with
    ///   change tracking enabled. Each method call uses this DB context without disposing it or saving
    ///   changes - the transaction is responsible to do these things when it is committed.
    /// </remarks>
    public class EFCoreDataAccess : IDataAccess
    {
        public static readonly string DbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "db.sqlite");

        private AppDatabaseContext _sharedDbContext;

        // Creates an IDisposable that, if no shared DbContext is set, provides a transient
        // DbContext and disposes it when the scope is disposed.
        private DbScope Scope() => new DbScope(_sharedDbContext);

        /// <summary>
        /// Creates an instance in which each method is executed atomically,
        /// i.e. a database context is created and disposed in each method call.
        /// </summary>
        public EFCoreDataAccess()
        {
        }

        /// <summary>
        /// Creates an instance in which all methods use the same database context.
        /// The database context is never disposed. This is used when working with
        /// transactions.
        /// </summary>
        public EFCoreDataAccess(AppDatabaseContext sharedDbContext)
        {
            _sharedDbContext = sharedDbContext;
        }

        public T GetItem<T>(string id) where T : class, IIdentifiable
        {
            using (var scope = Scope())
                return scope.Db.Find<T>(id);
        }

        public IEnumerable<T> GetItems<T>() where T : class, IIdentifiable
        {
            using (var scope = Scope())
                return scope.Db.Set<T>().ToListAsync().Result;
        }

        public void AddItem<T>(T item) where T : class, IIdentifiable
        {
            using (var scope = Scope())
            {
                scope.Db.Add(item);

                if (scope.IsDbContextTransient)
                    scope.Db.SaveChangesAndDetach();
            }
        }

        public void DeleteItem<T>(T item) where T : class
        {
            using (var scope = Scope())
            {
                scope.Db.Remove(item);

                if (scope.IsDbContextTransient)
                    scope.Db.SaveChangesAndDetach();
            }
        }

        public BaseTransaction StartTransaction(IEnumerable<object> itemsToTrack)
        {
            if (_sharedDbContext != null)
                throw new InvalidOperationException($"{nameof(StartTransaction)} must not be called from within the scope of a transaction");

            var db = new AppDatabaseContext(QueryTrackingBehavior.TrackAll);
            db.AttachRange(itemsToTrack);
            return new EFCoreTransaction(db);
        }

        public int GetVersion() => 0;

        public void DeleteDatabase()
        {
            using (var scope = Scope())
                scope.Db.Database.EnsureDeleted();
        }

        public void CreateDatabase(int version)
        {
            using (var scope = Scope())
                scope.Db.Database.EnsureCreated();
        }

        public string DatabasePath => DbPath;


        class DbScope : IDisposable
        {
            public bool IsDbContextTransient { get; }

            public AppDatabaseContext Db { get; }

            public DbScope(AppDatabaseContext sharedDbContext)
            {
                Db = sharedDbContext ?? new AppDatabaseContext(QueryTrackingBehavior.NoTracking);
                IsDbContextTransient = (sharedDbContext == null);
            }

            public void Dispose()
            {
                if (IsDbContextTransient)
                    Db.Dispose();
            }
        }
    }

    // TODO: Wait for EF Core 2.1 which will support value type transformations like these

    /// <summary>
    /// Formats <see cref="GeoLocation"/> as a string "latitude,longitude".
    /// </summary>
    static class GeoLocationConverter
    {
        public static string ToString(GeoLocation p) => $"{p.Latitude},{p.Longitude}";

        public static GeoLocation FromString(string s)
        {
            if (s?.Split(',') is string[] parts &&
                parts.Length == 2 &&
                double.TryParse(parts[0], out var lat) &&
                double.TryParse(parts[1], out var lon))
            {
                return new GeoLocation(lat, lon);
            }

            return default(GeoLocation);
        }
    }

    /// <summary>
    /// Formats <see cref="Rectangle"/> as a string "left,top,right,bottom".
    /// </summary>
    static class RectangleConverter
    {
        public static string ToString(Rectangle rect) => $"{rect.Left},{rect.Top},{rect.Right},{rect.Bottom}";

        public static Rectangle FromString(string s)
        {
            if (s?.Split(',') is string[] parts &&
                parts.Length == 4 &&
                int.TryParse(parts[0], out var left) &&
                int.TryParse(parts[1], out var top) &&
                int.TryParse(parts[2], out var right) &&
                int.TryParse(parts[3], out var bottom))
            {
                return new Rectangle(left, top, right, bottom);
            }

            return default(Rectangle);
        }
    }
}