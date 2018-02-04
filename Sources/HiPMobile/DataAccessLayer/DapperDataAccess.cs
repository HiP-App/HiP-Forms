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

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer
{
    public class DapperDataAccess: IDataAccess
    {
        private static readonly string DbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "db.sqlite");

        private IDbConnection Connection() => IoCManager.Resolve<IDbConnectionProvider>().ProvideIDbConnection(DbPath);

        public T GetItem<T>(string id) where T : IIdentifiable
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<T> GetItems<T>() where T : IIdentifiable
        {
            throw new System.NotImplementedException();
        }

        public bool DeleteItem<T>(string id) where T : IIdentifiable
        {
            throw new System.NotImplementedException();
        }

        public BaseTransaction StartTransaction()
        {
            throw new System.NotImplementedException();
        }

        public T CreateObject<T>() where T : IIdentifiable, new()
        {
            throw new System.NotImplementedException();
        }

        public T CreateObject<T>(string id, bool updateCurrent = false) where T : IIdentifiable, new()
        {
            throw new System.NotImplementedException();
        }

        public int GetVersion()
        {
            throw new System.NotImplementedException();
        }

        public void DeleteDatabase()
        {
            throw new System.NotImplementedException();
        }

        public void CreateDatabase(int version)
        {
            // TODO Exhibit, ExhibitSet, ImagePage, Page, Route, RouteSet, TimeSliderPage, UserRating
            // TODO ON DELETE + ON UPDATE
            const string stmt = @"
                CREATE TABLE AppetizerPage (
                    Id TEXT PRIMARY KEY, `Text` TEXT, Image TEXT, FOREIGN KEY(Image) REFERENCES Image(Id)
                );
                CREATE TABLE Audio (
                    Id TEXT PRIMARY KEY, DataPath TEXT, Title TEXT, Caption TEXT, IdForRestApi INTEGER, Timestamp TEXT
                );
                CREATE Table ExhibitsVisitedAchievement (
                    Id TEXT PRIMARY KEY, Title TEXT, Description TEXT, ThumbnailUrl TEXT, NextId TEXT, Count INTEGER, IsUnlocked INTEGER, Points INTEGER
                );
                CREATE TABLE ExhibitsVisitedAchievementPendingNotification (
                    Id TEXT PRIMARY KEY, Achievement TEXT, FOREIGN KEY(Achievement) REFERENCES ExhibitsVisitedAchievement(Id)
                );
                CREATE TABLE GeoLocation (
                    Id TEXT PRIMARY KEY, Latitude REAL, Longitude REAL
                );
                CREATE TABLE Image (
                    Id TEXT PRIMARY KEY, DataPath TEXT, Title TEXT, Description TEXT, IdForRestApi INTEGER, Timestamp TEXT
                );
                CREATE TABLE LongElement (
                    Id TEXT PRIMARY KEY, Value INTEGER
                );
                CREATE TABLE MapMarker (
                    Id TEXT PRIMARY KEY, Title TEXT, `Text` TEXT
                );
                CREATE TABLE Rectangle (
                    Id TEXT PRIMARY KEY, Top INTEGER, Bottom INTEGER, Left INTEGER, Right INTEGER
                );
                CREATE TABLE RouteFinishedAchievement (
                    Id TEXT PRIMARY KEY, Title TEXT, Description TEXT, ThumbnailUrl TEXT, NextId TEXT, IsUnlocked INTEGER, Points INTEGER, RouteId INTEGER
                );
                CREATE TABLE RouteFinishedAchievementPendingNotification (
                    Id TEXT PRIMARY KEY, Achievement TEXT, FOREIGN KEY(Achievement) REFERENCES RouteFinishedAchievement(Id)
                );
                CREATE TABLE RouteTag (
                    Id TEXT PRIMARY KEY, Tag TEXT, Name TEXT, Image TEXT, IdForRestApi INTEGER, Timestamp TEXT, FOREIGN KEY(Image) REFERENCES Image(Id)
                );
                CREATE TABLE SliderImage (
                    Id TEXT PRIMARY KEY, ImageName TEXT, Year INTEGER
                );
                CREATE TABLE StringElement (
                    Id TEXT PRIMARY KEY, Value TEXT
                );
                CREATE TABLE TextPage (
                    Id TEXT PRIMARY KEY, `Text` TEXT, FontFamily TEXT, Title TEXT, Description TEXT
                );
                CREATE TABLE ViaPointData (
                    Id TEXT PRIMARY KEY, Location TEXT, Title TEXT, Description TEXT, ExhibitId TEXT, FOREIGN KEY(Location) REFERENCES GeoLocation(Id)
                );
                CREATE TABLE Waypoint (
                    Id TEXT PRIMARY KEY, Location TEXT, Visited INTEGER, Exhibit TEXT, FOREIGN KEY(Location) REFERENCES GeoLocation(Id), FOREIGN KEY(Exhibit) REFERENCES Exhibit(Id)
                );
            ";
            using (var conn = Connection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = stmt;
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public string DatabasePath => DbPath;
    }
}