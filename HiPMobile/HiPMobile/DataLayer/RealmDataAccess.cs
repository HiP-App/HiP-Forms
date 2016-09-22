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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using de.upb.hip.mobile.pcl.Common;
using de.upb.hip.mobile.pcl.DataAccessLayer;
using de.upb.hip.mobile.pcl.Helpers;
using Realms;

namespace de.upb.hip.mobile.pcl.DataLayer {
    /// <summary>
    ///     Class encapsulating the access to the Realm Database
    /// </summary>
    public class RealmDataAccess : IDataAccess {

        /// <summary>
        ///     Object used for mutual exclusion.
        /// </summary>
        private static readonly object locker = new object ();

        public T GetItem<T> (string id) where T : RealmObject, IIdentifiable
        {
            lock (locker)
            {
                // Realm has problems when using LINQ expression here
                var objects = Instance.All<T> ();
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
                return Instance.All<T> ();
            }
        }

        public bool DeleteItem<T> (string id) where T : RealmObject, IIdentifiable
        {
            var item = GetItem<T> (id);
            Instance.Write (() => Instance.Remove (item));

            if (item != null)
                return true;
            return false;
        }

        public BaseTransaction StartTransaction ()
        {
            var transactionInstance = Instance;
            var transaction = transactionInstance.BeginWrite ();
            return  new RealmTransaction (transaction, transactionInstance);
        }

        public T CreateObject<T> () where T : RealmObject, IIdentifiable, new ()
        {
            // create the instance
            T instance = null;
            instance = Instance.CreateObject<T>();

            // generate a unique id
            string id;
            do
            {
                id = GenerateId();
            } while (GetItem<T>(id) != null);

            // assign the id and return the instance
            instance.Id = id;
            return instance;
        }

        public int GetVersion ()
        {
            lock (locker)
            {
                return Convert.ToInt32(Configuragion.SchemaVersion);
            }
        }

        public void DeleteDatabase ()
        {
            lock (locker)
            {
                Realm.DeleteRealm (Configuragion);
            }
        }

        public void CreateDatabase (int version)
        {
            Configuragion = new RealmConfiguration {SchemaVersion = Convert.ToUInt64(version)};
        }

        private static string GenerateId()
        {
            return Guid.NewGuid().ToString();
        }

        private Realm Instance => Realm.GetInstance(Configuragion);

        private static RealmConfiguration config;

        private static RealmConfiguration Configuragion {
            get {
                if (config == null)
                {
                    config = new RealmConfiguration {SchemaVersion = Convert.ToUInt64(Settings.DatabaseVersion)};
                }
                return config;
            }
            set { config = value; }
        }

    }
}