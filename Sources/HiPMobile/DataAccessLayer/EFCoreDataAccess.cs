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
using Microsoft.EntityFrameworkCore.Storage;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer
{
    public class EFCoreDataAccess : IDataAccess
    {
        public static readonly string DbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "db.sqlite");

        private AppDatabaseContext _db;

        private AppDatabaseContext Db => _db ?? (_db = new AppDatabaseContext());

        public T GetItem<T>(string id) where T : class, IIdentifiable
        {
            return Db.Find<T>(id);
        }

        public IEnumerable<T> GetItems<T>() where T : class, IIdentifiable
        {
            return Db.Set<T>().ToListAsync().Result;
        }

        public bool DeleteItem<T>(string id) where T : class, IIdentifiable
        {
            var entity = GetItem<T>(id);

            if (entity == null)
                return false;

            Db.Remove(entity);
            Db.SaveChanges();
            return true;
        }

        public BaseTransaction StartTransaction()
        {
            return new EFCoreTransaction(Db.Database.BeginTransaction());
        }

        public T CreateObject<T>() where T : class, IIdentifiable, new()
        {
            var entity = new T { Id = Guid.NewGuid().ToString() };
            Db.Add(entity);
            Db.SaveChanges();
            return entity;
        }

        public T CreateObject<T>(string id, bool updateCurrent = false) where T : class, IIdentifiable, new()
        {
            if (updateCurrent)
                DeleteItem<T>(id);

            var entity = new T { Id = id };
            Db.Add(entity);
            Db.SaveChanges();
            return entity;
        }

        public int GetVersion()
        {
            throw new NotImplementedException();
        }

        public void DeleteDatabase()
        {
            Db.Database.EnsureDeleted();
        }

        public void CreateDatabase(int version)
        {
            Db.Database.EnsureCreated();
        }

        public string DatabasePath => DbPath;
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