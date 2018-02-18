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

using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using System.Collections.Generic;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer
{
    /// <summary>
    /// The interface describing the access to the data layer in the scope of a transaction.
    /// </summary>
    public interface ITransactionDataAccess
    {
        /// <summary>
        /// Gets an item with the given key from the database.
        /// </summary>
        /// <typeparam name="T">The type of the item being retrieved.</typeparam>
        /// <param name="id">The key of the item.</param>
        /// <returns>The item if it exists, null otherwise.</returns>
        T GetItem<T>(string id, params string[] pathsToInclude) where T : class, IIdentifiable;

        /// <summary>
        /// Gets all items of the specified type from the database.
        /// </summary>
        /// <typeparam name="T">The type of the items being retrieved.</typeparam>
        /// <returns>The enumerable of items.</returns>
        IEnumerable<T> GetItems<T>(params string[] pathsToInclude) where T : class, IIdentifiable;

        /// <summary>
        /// Inserts an item into the database.
        /// </summary>
        void AddItem<T>(T item) where T : class, IIdentifiable;

        /// <summary>
        /// Deletes an item from the database. If the item doesn't exists, nothing is changed.
        /// </summary>
        void DeleteItem<T>(T item) where T : class;
    }

    /// <summary>
    /// The interface describing the access to the data layer.
    /// </summary>
    public interface IDataAccess : ITransactionDataAccess
    {
        /// <summary>
        /// Starts a transaction in which items can be added, updated and deleted in a whole graph of items.
        /// The method enables change tracking for the specified root items and all directly or
        /// indirectly referenced items. All changes within the transaction are recorded and saved when the
        /// transaction is committed.
        /// Important: The items to be tracked must already exist in the database.
        /// Furthermore, changes are only detected within the scope of the transaction,
        /// i.e. if some item in the graph already has unsaved changes at the time of calling
        /// <see cref="StartTransaction(IEnumerable{object})"/>, these changes won't be detected and won't be saved.
        /// </summary>
        /// <returns>The transaction object.</returns>
        BaseTransaction StartTransaction(IEnumerable<object> itemsToTrack);
        
        /// <summary>
        /// Gets the version number for the currently saved data.
        /// </summary>
        /// <returns>The version of the data.</returns>
        int GetVersion();

        /// <summary>
        /// Deletes the whole database and restarts the app.
        /// </summary>
        void DeleteDatabase();

        /// <summary>
        /// Creates an empty database with the specified version.
        /// </summary>
        /// <param name="version">The version of the new database.</param>
        void CreateDatabase(int version);

        /// <summary>
        /// The database path.
        /// </summary> 
        string DatabasePath { get; }
    }
}