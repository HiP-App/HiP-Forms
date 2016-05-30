// Copyright (C) 2016 History in Paderborn App - Universität Paderborn
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
using System.Linq;
using Realms;

namespace de.upb.hip.mobile.pcl.BusinessLayer.Models {
    public class BusinessEntitiyFactory {

        /// <summary>
        /// Creates an object of type T that is synced to the database.
        /// </summary>
        /// <typeparam name="T">The type of the object being created. T needs to be subtype of RealmObject and implement the IIdentifiable interface.</typeparam>
        /// <returns>The instance.</returns>
        public static T CreateBusinessObject<T> () where T : RealmObject, IIdentifiable, new ()
        {
            // create the instance
            T instance = null;
            Realm.GetInstance ().Write (() => instance = Realm.GetInstance ().CreateObject<T> ());

            // generate a unique id
            string id;
            do
            {
                id = GenerateId ();
            } while (Realm.GetInstance ().All<T> ().Count (element => element.Id == id) > 0);

            // assign the id and return the instance
            instance.Id = id;
            return instance;
        }

        private static string GenerateId ()
        {
            return Guid.NewGuid ().ToString();
        }

    }
}