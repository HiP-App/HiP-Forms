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

        public BaseTransaction StartTransaction ()
        {
            var transaction = GetInstance ().BeginWrite ();
            return  new RealmTransaction (transaction);
        }

        public T CreateObject<T> () where T : RealmObject, IIdentifiable, new ()
        {
            // create the instance
            T instance = null;
            instance = Realm.GetInstance().CreateObject<T>();

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

        private static string GenerateId()
        {
            return Guid.NewGuid().ToString();
        }

        // Singleton pattern

        public Realm GetInstance ()
        {
            return Instance;
        }

        private Realm _instance;
        private Realm Instance {
            get {
                if (false &&_instance == null)
                {
                    _instance = Realm.GetInstance ();
                }
                _instance = Realm.GetInstance();
                return _instance;
            }
        }

    }
}