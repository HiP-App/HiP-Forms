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
using System.Reflection;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using Realms;
using Realms.Exceptions;

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

        private readonly Dictionary<string, object> _objects = new Dictionary<string, object>();

        public T GetItem<T>(string id) where T : RealmObject, IIdentifiable
        {
            lock (Locker)
            {
                return _objects.TryGetValue(id, out var o) && o is T t ? t : null;
            }
        }

        public IEnumerable<T> GetItems<T>() where T : RealmObject, IIdentifiable
        {
            lock (Locker)
            {
                return _objects.Values.OfType<T>();
            }
        }

        public bool DeleteItem<T>(string id) where T : RealmObject, IIdentifiable
        {
            return _objects.Remove(id);
        }

        public BaseTransaction StartTransaction()
        {
            try
            {
                //var transactionInstance = Instance;
                //var transaction = transactionInstance.BeginWrite();
                //return new RealmTransaction(transaction);
                return new FakeTransaction();
            }
            catch (RealmInvalidTransactionException e)
            {
                throw new InvalidTransactionException("Invalid transaction!", e);
            }
        }

        public T CreateObject<T>(string id, bool updateCurrent) where T : RealmObject, IIdentifiable, new()
        {
            var instance = new T { Id = id };
            _objects[id] = instance;
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

            _objects.Add(id, instance);
            return instance;
        }

        public int GetVersion()
        {
            lock (Locker)
            {
                //return Convert.ToInt32(Configuration.SchemaVersion);
                return 1;
            }
        }

        public void DeleteDatabase()
        {
            Settings.ShouldDeleteDbOnLaunch = true;
            IoCManager.Resolve<IAppCloser>().RestartOrClose();
        }

        public void CreateDatabase(int version)
        {
            //Configuration = new RealmConfiguration { SchemaVersion = Convert.ToUInt64(version) };
        }

        private static string GenerateId()
        {
            return Guid.NewGuid().ToString();
        }

        //private static RealmConfiguration config;

        //private static RealmConfiguration Configuration
        //{
        //    get => config ?? (config = new RealmConfiguration { SchemaVersion = Convert.ToUInt64(Settings.DatabaseVersion) });
        //    set => config = value;
        //}

        public string DatabasePath => "FakeDB.db"; // Configuration.DatabasePath;
    }

    public class FakeTransaction : BaseTransaction
    {
        public override void Commit() { }

        public override void Rollback() { }
    }
}