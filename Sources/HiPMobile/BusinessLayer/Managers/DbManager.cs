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

using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using Realms;
using DbDummyDataFiller = PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.DbDummyDataFiller;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers {
    /// <summary>
    /// Access to database methods.
    /// </summary>
    public static class DbManager {

        private static readonly IDataAccess DataAccess = IoCManager.Resolve<IDataAccess>();

        /// <summary>
        /// Creates an object of type T that is synced to the database.
        /// </summary>
        /// <typeparam name="T">The type of the object being created. T needs to be subtype of RealmObject and implement the IIdentifiable interface.</typeparam>
        /// <returns>The instance.</returns>
        public static T CreateBusinessObject<T> () where T : RealmObject, IIdentifiable, new ()
        {
            return DataAccess.CreateObject<T> ();
        }

        /// <summary>
        /// Delete an object of type T from the database.
        /// </summary>
        /// <typeparam name="T">The type of the object. T needs to be subtype of RealmObject and implement the IIdentifiable interface.</typeparam>
        /// <param name="entitiy">The object to be deleted.</param>
        /// <returns>True if deletion was successful. False otherwise.</returns>
        public static bool DeleteBusinessEntity<T> (T entitiy) where T : RealmObject, IIdentifiable
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
        public static BaseTransaction StartTransaction ()
        {
            return DataAccess.StartTransaction ();
        }

        public static void UpdateDatabase (IProgressListener listener=null)
        {
            if (DataAccess.GetVersion () < DbDummyDataFiller.DatabaseVersion)
            {
                DataAccess.DeleteDatabase ();
                DataAccess.CreateDatabase (DbDummyDataFiller.DatabaseVersion);
                DbDummyDataFiller dbDataFiller = new DbDummyDataFiller ();
                if (listener != null)
                {
                    dbDataFiller.ProgressChanged += listener.UpdateProgress;
                }
                dbDataFiller.InsertData ();
                Settings.DatabaseVersion = DbDummyDataFiller.DatabaseVersion;
                if (listener != null)
                {
                    dbDataFiller.ProgressChanged -= listener.UpdateProgress;
                }
            }
        }

        public static void DeleteDatabase ()
        {
            DataAccess.DeleteDatabase ();
            IoCManager.Resolve<IDbChangedHandler> ().NotifyAll ();
        }

        /// <summary>
        /// Returns if the db is up to date.
        /// </summary>
        /// <returns>True if up to date, false otherwise.</returns>
        public static bool IsDatabaseUpToDate ()
        {
            return DataAccess.GetVersion () >= DbDummyDataFiller.DatabaseVersion;
        }

    }
}