// Copyright (C) 2016 History in Paderborn App - Universität Paderborn
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//       http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using JetBrains.Annotations;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers
{
    /// <summary>
    /// Access to database methods.
    /// </summary>
    public static class DbManager
    {
        private static readonly IDataAccess DataAccess = IoCManager.Resolve<IDataAccess>();

        /// <summary>
        /// Creates an object of type T that is synced to the database.
        /// </summary>
        /// <typeparam name="T">The type of the object being created. T needs to be subtype of RealmObject and implement the IIdentifiable interface.</typeparam>
        /// <returns>The instance.</returns>
        public static T CreateBusinessObject<T>() where T : IIdentifiable, new()
        {
            return DataAccess.CreateObject<T>();
        }

        /// <summary>
        /// Creates an object of type T that is synced to the database.
        /// </summary>
        /// <param name="id">The ID to assign to the object.</param>
        /// <param name="updateCurrent">If true, first removes any object of the same type with the id.</param>
        /// <typeparam name="T">The type of the object being created. T needs to be subtype of RealmObject and implement the IIdentifiable interface.</typeparam>
        /// <returns>The instance.</returns>
        public static T CreateBusinessObject<T>([NotNull] string id, bool updateCurrent = false) where T : IIdentifiable, new()
        {
            return DataAccess.CreateObject<T>(id, updateCurrent);
        }

        /// <summary>
        /// Delete an object of type T from the database.
        /// </summary>
        /// <typeparam name="T">The type of the object. T needs to be subtype of RealmObject and implement the IIdentifiable interface.</typeparam>
        /// <param name="entitiy">The object to be deleted.</param>
        /// <returns>True if deletion was successful. False otherwise.</returns>
        public static bool DeleteBusinessEntity<T>(T entitiy) where T : IIdentifiable
        {
            if (entitiy != null)
            {
                return DataAccess.DeleteItem<T>(entitiy.Id);
            }

            return true;
        }

        /// <summary>
        /// Starts a new write transaction. Make sure to close the transaction by either committing it or rolling back.
        /// </summary>
        /// <returns>The transaction object which can perform committing or rolling back.</returns>
        public static BaseTransaction StartTransaction()
        {
            return DataAccess.StartTransaction();
        }

        /// <summary>
        /// Deletes the whole database and restarts the app
        /// </summary>
        public static void DeleteDatabase()
        {
            // delete from "cache" to see the changes instantly
            var exhibits = ExhibitManager.GetExhibits();
            foreach (var exhibit in exhibits)
            {
                ExhibitManager.DeleteExhibit(exhibit);
            }

            var routes = RouteManager.GetRoutes();
            foreach (var route in routes)
            {
                RouteManager.DeleteRoute(route);
            }

            DataAccess.DeleteDatabase();
            IoCManager.Resolve<IDbChangedHandler>().NotifyAll();
        }
    }
}