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
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using Realms;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.DataLayer
{
    /// <summary>
    ///     Class encapsulating the access to the Realm Database
    /// </summary>
    public class RealmDataAccess : IDataAccess
    {
        /// <summary>
        ///     Object used for mutual exclusion.
        /// </summary>
        private static readonly object Locker = new object();

        public T GetItem<T>(string id) where T : RealmObject, IIdentifiable
        {
            lock (Locker)
            {
                // Realm has problems when using LINQ expression here
                var objects = Instance.All<T>();
                foreach (T realmResult in objects)
                {
                    if (!string.IsNullOrEmpty(realmResult.Id) && realmResult.Id.Equals(id))
                        return realmResult;
                }
                return null;
            }
        }

        public IEnumerable<T> GetItems<T>() where T : RealmObject, IIdentifiable
        {
            lock (Locker)
            {
                return Instance.All<T>();
            }
        }

        public bool DeleteItem<T>(string id) where T : RealmObject, IIdentifiable
        {
            var item = GetItem<T>(id);
            if (item == null)
            {
                return false;
            }
            
            if (!Instance.IsInTransaction)
            {
                Instance.Write(() => Instance.Remove(item));
            }
            else
            {
                Instance.Remove(item);
            }

            return true;
        }

        public BaseTransaction StartTransaction()
        {
            var transactionInstance = Instance;
            var transaction = transactionInstance.BeginWrite();
            return new RealmTransaction(transaction);
        }

        T IDataAccess.CreateObject<T>(string id, bool updateCurrent)
        {
            // create the instance
            var instance = new T { Id = id };

            if (updateCurrent)
            {
                DeleteItem<T>(id);
            }
            Instance.Add(instance);
            return instance;
        }

        public T CreateObject<T>() where T : RealmObject, IIdentifiable, new()
        {
            // create the instance
            var instance = new T();

            string id;
            // generate a unique id
            do
            {
                id = GenerateId();
            } while (GetItem<T>(id) != null);

            instance.Id = id;

            Instance.Add(instance);
            return instance;
        }

        public int GetVersion()
        {
            lock (Locker)
            {
                return Convert.ToInt32(Configuration.SchemaVersion);
            }
        }

        public void DeleteDatabase()
        {
            lock (Locker)
            {
                Realm.DeleteRealm(Configuration);
            }
        }

        public void CreateDatabase(int version)
        {
            Configuration = new RealmConfiguration { SchemaVersion = Convert.ToUInt64(version) };
        }

        private static string GenerateId()
        {
            return Guid.NewGuid().ToString();
        }

        private Realm Instance => Realm.GetInstance(Configuration);

        private static RealmConfiguration config;

        private static RealmConfiguration Configuration
        {
            get
            {
                if (config == null)
                {
                    config = new RealmConfiguration { SchemaVersion = Convert.ToUInt64(Settings.DatabaseVersion) };
                }
                return config;
            }
            set { config = value; }
        }

        public string DatabasePath
        {
            get { return Configuration.DatabasePath; }
        }
    }
}