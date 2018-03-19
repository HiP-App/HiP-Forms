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

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer
{
    /// <summary>
    /// Data access interface used with the IoC container.
    /// Provides read-only access to the data layer and some additional DB utility methods.
    /// </summary>
    public interface IDataAccess : IReadOnlyDataAccess
    {
        /// <summary>
        /// Starts a transaction in which items can be added, updated and deleted in a whole graph of items.
        /// The method enables change tracking for the specified root items and all directly or
        /// indirectly referenced items. All changes within the transaction are recorded and saved when the
        /// transaction is committed.
        /// Note: Changes are only detected within the scope of the transaction, i.e. if some item in the graph
        /// already has unsaved changes at the time of calling <see cref="StartTransaction(IEnumerable{object})"/>, 
        /// these changes won't be detected and won't be saved.
        /// </summary>
        /// <param name="itemsToTrack">
        /// Existing entities that should be attached to the transaction scope.
        /// Example: Assume you have already retrieved an <see cref="Image"/> entity from the database. Now you start
        /// a transaction in which you create a new <see cref="Exhibit"/> entity and assign the image to it. In this case
        /// you MUST pass the image as one of the <paramref name="itemsToTrack"/>. Otherwise the transaction scope doesn't
        /// know about the image and assumes that it has been created within the transaction and needs to be inserted into
        /// the database which, of course, would be wrong.
        /// </param>
        /// <returns>The transaction object.</returns>
        BaseTransaction StartTransaction(IEnumerable<object> itemsToTrack = null);
        
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
    }
}