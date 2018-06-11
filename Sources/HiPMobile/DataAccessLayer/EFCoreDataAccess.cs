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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using AsyncBridge;
using Microsoft.Extensions.Internal;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers.Threading;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer
{
    /// <remarks>
    /// This type is used for both, general read-only data access and read/write transaction scopes.
    /// 
    /// * In the general case, each method call uses a fresh, transient DB context. Change tracking is
    ///   disabled, so any entities that are returned are (in EF Core terms) in a "detached" state. 
    ///   Before a method call returns, the DB context is disposed of.
    ///   
    /// * In a transaction scope, the transaction provides us with a single, permanent DB context with
    ///   change tracking enabled. Each method call uses this DB context without disposing of it or saving
    ///   changes - the transaction is responsible to do these things when it is committed.
    /// </remarks>
    public class EFCoreDataAccess : IDataAccess, ITransactionDataAccess
    {
        public static readonly string DbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "db.sqlite");

        private static readonly TaskScheduler DbThreadScheduler = new SingleThreadTaskScheduler();
        private static bool isInTransaction = false;

        private readonly AppDatabaseContext sharedDbContext;

        public DbContextDebugView DebugView => Scope().Db.DebugView;

        public string DatabasePath => DbPath;

        // Creates an IDisposable that, if no shared DbContext is set, provides a transient
        // DbContext and disposes it when the scope is disposed.
        private DbScope Scope() => new DbScope(sharedDbContext);

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
            this.sharedDbContext = sharedDbContext;
        }

        public T GetItem<T>(string id, params string[] pathsToInclude) where T : class, IIdentifiable
        {
            using (var scope = Scope())
            {
                var dbSet = scope.Db.Set<T>();

                // First, query local cache
                // (only relevant in transaction scopes; otherwise we have a transient DbContext where the cache is always empty)
                var localResult = BuildQuery(dbSet.Local.AsQueryable(), pathsToInclude)
                    .SingleOrDefault(o => o.Id == id);

                if (localResult != null)
                    return localResult;

                // If entity not cached, query the database
                var dbResult = BuildQuery(dbSet, pathsToInclude)
                    .SingleOrDefault(o => o.Id == id);

                return dbResult; // may still be null if entity doesn't exist
            }
        }

        public IReadOnlyList<T> GetItems<T>(params string[] pathsToInclude) where T : class, IIdentifiable
        {
            using (var scope = Scope())
            {
                // Combine entities from local cache and database
                // (only relevant in transaction scopes; otherwise we have a transient DbContext where the cache is always empty)
                var dbSet = scope.Db.Set<T>();
                var localResults = BuildQuery(dbSet.Local.AsQueryable(), pathsToInclude);
                var dbResults = BuildQuery(dbSet, pathsToInclude);
                var allResults = localResults.Union(dbResults).ToList();
                return allResults;
            }
        }

        public void AddItem<T>(T item) where T : class, IIdentifiable
        {
            using (var scope = Scope())
            {
                scope.Db.Add(item);

                if (scope.IsDbContextTransient)
                    scope.Db.SaveChangesAndDetach();
                else
                    scope.Db.SaveChanges();
            }
        }

        public void DeleteItem<T>(T item) where T : class
        {
            using (var scope = Scope())
            {
                scope.Db.Remove(item);

                if (scope.IsDbContextTransient)
                    scope.Db.SaveChangesAndDetach();
                else
                    scope.Db.SaveChanges();
            }
        }

        private static readonly object Lock = new object();

        public T InTransaction<T>(IEnumerable<object> itemsToTrack, Func<BaseTransaction, T> func)
        {
            if (sharedDbContext != null)
                throw new InvalidOperationException($"{nameof(InTransaction)} must not be called within the scope of a transaction");

            lock (Lock)
            {
                var isInTransactionAlready = isInTransaction;
                isInTransaction = true;

                var db = new AppDatabaseContext(QueryTrackingBehavior.TrackAll);
                db.AttachRange(itemsToTrack?.Distinct() ?? Enumerable.Empty<object>());
                var transaction = new EFCoreTransaction(db, !isInTransactionAlready);

                try
                {
                    var res = func.Invoke(transaction);

                    transaction.Commit();
                    if (!isInTransactionAlready)
                    {
                        isInTransaction = false;
                    }

                    return res;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    transaction.Rollback();
                    if (!isInTransactionAlready)
                    {
                        isInTransaction = false;
                    }

                    throw;
                }
            }
        }

        public int GetVersion() => 1;

        public void DeleteDatabase()
        {
            using (var scope = Scope())
            {
                scope.Db.Database.EnsureDeleted();
            }
        }

        public void CreateDatabase(int version)
        {
            using (var scope = Scope())
            {
                SQLitePCL.Batteries.Init();
                scope.Db.Database.EnsureCreated();
            }
        }

        private static IQueryable<T> BuildQuery<T>(IQueryable<T> dataSource, IEnumerable<string> pathsToInclude) where T : class
        {
            return pathsToInclude.Aggregate(dataSource, (current, path) => current.Include(path));
        }

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
        public static string ToString(GeoLocation p) =>
            $"{p.Latitude.ToString(CultureInfo.InvariantCulture)},{p.Longitude.ToString(CultureInfo.InvariantCulture)}";

        public static GeoLocation FromString(string s)
        {
            if (s?.Split(',') is string[] parts &&
                parts.Length == 2 &&
                double.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out var lat) &&
                double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out var lon))
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