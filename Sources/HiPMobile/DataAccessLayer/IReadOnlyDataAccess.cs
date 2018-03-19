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
    /// Provides read-only access to the data layer.
    /// </summary>
    public interface IReadOnlyDataAccess
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
        IReadOnlyList<T> GetItems<T>(params string[] pathsToInclude) where T : class, IIdentifiable;

        /// <summary>
        /// The database path.
        /// </summary> 
        string DatabasePath { get; }
    }
}