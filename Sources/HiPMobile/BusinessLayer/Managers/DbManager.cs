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

using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.DtoToModelConverters;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;
using System.Collections.Generic;
using System.Linq;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers
{
    /// <summary>
    /// Provides access to database methods.
    /// </summary>
    public static class DbManager
    {
        /// <summary>
        /// Provides read-only database access. To modify the database in any way,
        /// use <see cref="StartTransaction(IEnumerable{object})"/> instead.
        /// </summary>
        public static IReadOnlyDataAccess DataAccess { get; } = IoCManager.Resolve<IDataAccess>();

        /// <summary>
        /// Starts a new write transaction. Make sure to close the transaction by either committing it or rolling back.
        /// </summary>
        /// <param name="itemsToTrack">
        /// Entities that should be added to the change tracker.
        /// See <see cref="IDataAccess.StartTransaction(IEnumerable{object})"/> for an example scenario where this is needed.
        /// Note that <see cref="BackupData.BackupImage"/> and <see cref="BackupData.BackupImageTag"/> are included by default
        /// so they can be freely assigned to any other entity without being considered "new".
        /// </param>
        /// <returns>The transaction object which can perform committing or rolling back.</returns>
        public static BaseTransaction StartTransaction(IEnumerable<object> itemsToTrack) =>
            IoCManager.Resolve<IDataAccess>().StartTransaction(itemsToTrack.Concat(new[] { BackupData.BackupImage, BackupData.BackupImageTag }));

        /// <summary>
        /// Starts a new write transaction. Make sure to close the transaction by either committing it or rolling back.
        /// </summary>
        /// <param name="itemsToTrack">
        /// Entities that should be added to the change tracker.
        /// See <see cref="IDataAccess.StartTransaction(IEnumerable{object})"/> for an example scenario where this is needed.
        /// Note that <see cref="BackupData.BackupImage"/> and <see cref="BackupData.BackupImageTag"/> are included by default
        /// so they can be freely assigned to any other entity without being considered "new".
        /// </param>
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

            IoCManager.Resolve<IDataAccess>().DeleteDatabase();
            IoCManager.Resolve<IDbChangedHandler>().NotifyAll();
        }
    }
}