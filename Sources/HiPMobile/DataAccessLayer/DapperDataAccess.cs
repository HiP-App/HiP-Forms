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
using Dapper;
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
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetItems<T>() where T : IIdentifiable
        {
            throw new NotImplementedException();
        }

        public bool DeleteItem<T>(string id) where T : IIdentifiable
        {
            throw new NotImplementedException();
        }

        public BaseTransaction StartTransaction()
        {
            throw new NotImplementedException();
        }

        public T CreateObject<T>() where T : IIdentifiable, new()
        {
            throw new NotImplementedException();
        }

        public T CreateObject<T>(string id, bool updateCurrent = false) where T : IIdentifiable, new()
        {
            throw new NotImplementedException();
        }

        public int GetVersion()
        {
            throw new NotImplementedException();
        }

        public void DeleteDatabase()
        {
            throw new NotImplementedException();
        }

        public void CreateDatabase(int version)
        {
            // TODO TextPage, TimeSliderPage, UserRating
            // TODO ON DELETE + ON UPDATE
            // TODO What to do with properties like Exhibit.Tags? Shouldn't that be of type List<Tag>?
            const string stmt = @"
                CREATE TABLE Exhibit (
                    Id TEXT PRIMARY KEY, Name TEXT, Description TEXT, Location TEXT, Radius INTEGER, Image TEXT, LastNearbyTime DATETIME,
                    DetailsDataLoaded BOOLEAN, Unlocked BOOLEAN, IdForRestApi INTEGER, Timestamp DATETIME,
                    FOREIGN KEY(Image) REFERENCES Image(Id)
                );
                CREATE TABLE Route (
                    Id TEXT PRIMARY KEY, Name TEXT, Description TEXT, Duration INTEGER, Distance DOUBLE, LastTimeDismissed DATETIME, DetailsDataLoaded BOOLEAN,
                    Image TEXT, Audio TEXT, IdForRestApi INTEGER, Timestamp DATETIME,
                    FOREIGN KEY(Image) REFERENCES Image(Id),
                    FOREIGN KEY(Audio) REFERENCES Audio(Id)
                );
                CREATE TABLE ExhibitsToRoutes (
                    ExhibitId TEXT, RouteId TEXT,
                    PRIMARY KEY(ExhibitId, RouteId),
                    FOREIGN KEY(ExhibitId) REFERENCES Exhibit(Id),
                    FOREIGN KEY(RouteId) REFERENCES Route(Id)
                );

                CREATE TABLE Page (
                    Id TEXT PRIMARY KEY, Audio INTEGER, AppetizerPage TEXT, ImagePage TEXT, TextPage TEXT, TimeSliderPage TEXT, IdForRestApi INTEGER, Timestamp DATETIME,
                    FOREIGN KEY(Audio) REFERENCES Audio(Id),
                    FOREIGN KEY(AppetizerPage) REFERENCES AppetizerPage(Id),
                    FOREIGN KEY(ImagePage) REFERENCES ImagePage(Id),
                    FOREIGN KEY(TextPage) REFERENCES TextPage(Id),
                    FOREIGN KEY(TimeSliderPage) REFERENCES TimeSliderPage(Id)
                );
                CREATE TABLE AppetizerPage (
                    Id TEXT PRIMARY KEY, `Text` TEXT, Image TEXT, FOREIGN KEY(Image) REFERENCES Image(Id)
                );
                CREATE TABLE TextPage (
                    Id TEXT PRIMARY KEY, `Text` TEXT, FontFamily TEXT, Title TEXT, Description TEXT
                );
                CREATE TABLE PagesToPages (
                    PageId TEXT, AdditionalInformationPageId TEXT,
                    PRIMARY KEY(PageId, AdditionalInformationPageId),
                    FOREIGN KEY(PageId) REFERENCES Page(Id),
                    FOREIGN KEY(AdditionalInformationPageId) REFERENCES Page(Id)
                );
                CREATE TABLE ExhibitsToPages (
                    ExhibitId TEXT, PageId TEXT,
                    PRIMARY KEY(ExhibitId, PageId),
                    FOREIGN KEY(ExhibitId) REFERENCES Exhibit(Id),
                    FOREIGN KEY(PageId) REFERENCES Page(Id)
                );

                CREATE TABLE Image (
                    Id TEXT PRIMARY KEY, DataPath TEXT, Title TEXT, Description TEXT, IdForRestApi INTEGER, Timestamp TEXT
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

    /// <summary>
    /// Formats <see cref="GeoLocation"/> as a string "latitude,longitude".
    /// </summary>
    class GeoLocationTypeHandler : SqlMapper.TypeHandler<GeoLocation>
    {
        public override GeoLocation Parse(object value)
        {
            if (value?.ToString().Split(',') is string[] parts &&
                parts.Length == 2 &&
                double.TryParse(parts[0], out var lat) &&
                double.TryParse(parts[1], out var lon))
            {
                return new GeoLocation(lat, lon);
            }

            return default(GeoLocation);
        }

        public override void SetValue(IDbDataParameter parameter, GeoLocation value)
        {
            parameter.Value = $"{value.Latitude},{value.Longitude}";
        }
    }

    /// <summary>
    /// Formats <see cref="Rectangle"/> as a string "left,top,right,bottom".
    /// </summary>
    class RectangleTypeHandler : SqlMapper.TypeHandler<Rectangle>
    {
        public override Rectangle Parse(object value)
        {
            if (value?.ToString().Split(',') is string[] parts &&
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

        public override void SetValue(IDbDataParameter parameter, Rectangle value)
        {
            parameter.Value = $"{value.Left},{value.Top},{value.Right},{value.Bottom}";
        }
    }
}