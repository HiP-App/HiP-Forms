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

using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;
using System.Collections.Generic;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers
{
    /// <summary>
    /// Access to database methods.
    /// </summary>
    public static class DbManager
    {
        /// <summary>
        /// TODO: Validate comment about read-only access
        /// This should be used for read-only access. To modify the database in any way,
        /// use <see cref="StartTransaction(IEnumerable{object})"/> instead.
        /// </summary>
        public static IDataAccess DataAccess { get; } = IoCManager.Resolve<IDataAccess>();

        /// <summary>
        /// Starts a new write transaction. Make sure to close the transaction by either committing it or rolling back.
        /// </summary>
        /// <returns>The transaction object which can perform committing or rolling back.</returns>
        public static BaseTransaction StartTransaction(IEnumerable<object> itemsToTrack) =>
            DataAccess.StartTransaction(itemsToTrack);

        /// <summary>
        /// Starts a new write transaction. Make sure to close the transaction by either committing it or rolling back.
        /// </summary>
        /// <returns>The transaction object which can perform committing or rolling back.</returns>
        public static BaseTransaction StartTransaction(params object[] itemsToTrack) =>
            StartTransaction(itemsToTrack as IEnumerable<object>);

        /// <summary>
        /// Deletes the whole database and restarts the app
        /// </summary>
        public static void DeleteDatabase()
        {
            using (var transaction = StartTransaction())
            {
                var dataAccess = transaction.DataAccess;

                // delete from "cache" to see the changes instantly
                var exhibits = dataAccess.Exhibits().GetExhibits();
                foreach (var exhibit in exhibits)
                {
                    dataAccess.Exhibits().DeleteExhibit(exhibit);
                }

                var routes = dataAccess.Routes().GetRoutes();
                foreach (var route in routes)
                {
                    dataAccess.Routes().DeleteRoute(route);
                }
            }

            DataAccess.DeleteDatabase();
            IoCManager.Resolve<IDbChangedHandler>().NotifyAll();
        }
    }
}