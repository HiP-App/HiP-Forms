// /*
//  * Copyright (C) 2016 History in Paderborn App - Universität Paderborn
//  *
//  * Licensed under the Apache License, Version 2.0 (the "License");
//  * you may not use this file except in compliance with the License.
//  * You may obtain a copy of the License at
//  *
//  *      http://www.apache.org/licenses/LICENSE-2.0
//  *
//  * Unless required by applicable law or agreed to in writing, software
//  * distributed under the License is distributed on an "AS IS" BASIS,
//  * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  * See the License for the specific language governing permissions and
//  * limitations under the License.
//  */

using System.Collections.Generic;
using System.Linq;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using de.upb.hip.mobile.pcl.DataAccessLayer;
using Realms;

namespace de.upb.hip.mobile.pcl.DataLayer {
    /// <summary>
    ///     Class encapsulating the access to the Realm Database
    /// </summary>
    public class RealmDataAccess : IDataAccess {

        /// <summary>
        ///     Object used for mutual exclusion.
        /// </summary>
        private static object locker = new object ();

        public T GetItem<T> (string id) where T : RealmObject, IIdentifiable
        {
            lock (locker)
            {
                // Realm has problems when using LINQ expression here
                var objects = Realm.GetInstance ().All<T> ();
                foreach (T realmResult in objects)
                {
                    if (!string.IsNullOrEmpty(realmResult.Id) && realmResult.Id.Equals (id))
                        return realmResult;
                }
                return null;
            }
        }

        public IEnumerable<T> GetItems<T> () where T : RealmObject, IIdentifiable
        {
            lock (locker)
            {
                return Realm.GetInstance ().All<T> ();
            }
        }

        public bool DeleteItem<T> (string id) where T : RealmObject, IIdentifiable
        {
            var item = GetItem<T> (id);
            Realm.GetInstance ().Write (() => GetInstance ().Remove (item));

            if (item != null)
                return true;
            return false;
        }

        // Singleton pattern

        public static Realm GetInstance ()
        {
            return Realm.GetInstance ();
        }

    }
}