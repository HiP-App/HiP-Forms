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
using JetBrains.Annotations;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using Realms;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer
{
    /// <summary>
    ///     The interface describing the access to the data layer.
    /// </summary>
    public interface IDataAccess
    {
        /// <summary>
        ///     Gets an item with the given key from the database.
        /// </summary>
        /// <typeparam name="T">The type of the item being retrived. It has to be a subtype of BusinessEntityBase.</typeparam>
        /// <param name="id">The key of the item.</param>
        /// <returns>The item if it exists, null otherwise.</returns>
        T GetItem<T>(string id) where T : RealmObject, IIdentifiable;

        /// <summary>
        ///     Gets an enumeration of items from the database.
        /// </summary>
        /// <typeparam name="T">The type of the items being retrived. It has to be a subtype of BusinessEntityBase.</typeparam>
        /// <returns>The enumerable of items.</returns>
        IEnumerable<T> GetItems<T>() where T : RealmObject, IIdentifiable;

        /// <summary>
        ///     Deletes an item from the database. Id the item doesn't exists, nothing is changed.
        /// </summary>
        /// <param name="id">The item to be deleted.</param>
        /// <returns>True if deletion was successfull, False otherwise.</returns>
        bool DeleteItem<T>(string id) where T : RealmObject, IIdentifiable;

        /// <summary>
        /// Starts a transaction.
        /// </summary>
        /// <returns>The transaction object.</returns>
        BaseTransaction StartTransaction();

        /// <summary>
        /// Creates an object of type T that is synced to the database.
        /// </summary>
        /// <typeparam name="T">The type of the object being created. T needs to be subtype of RealmObject and implement the IIdentifiable interface.</typeparam>
        /// <returns>The instance.</returns>
        T CreateObject<T>() where T : RealmObject, IIdentifiable, new();
        
        /// <summary>
        /// Creates an object of type T that is synced to the database.
        /// </summary>
        /// <param name="id">The ID to assign to the object.</param>
        /// <param name="updateCurrent">If true, first removes any object of the same type with the id.</param>
        /// <typeparam name="T">The type of the object being created. T needs to be subtype of RealmObject and implement the IIdentifiable interface.</typeparam>
        /// <returns>The instance.</returns>
        T CreateObject<T>([NotNull] string id, bool updateCurrent = false) where T : RealmObject, IIdentifiable, new();

        /// <summary>
        /// Gets the version number for the currently saved data.
        /// </summary>
        /// <returns>The version of the data.</returns>
        int GetVersion();

        /// <summary>
        /// Deletes the whole database.
        /// </summary>
        void DeleteDatabase();

        /// <summary>
        /// Creates an empty database with the specified version.
        /// </summary>
        /// <param name="version">The version of the new database.</param>
        void CreateDatabase(int version);

        /// <summary>
        /// Return database path
        /// </summary> 
        string DatabasePath { get; }
    }
}