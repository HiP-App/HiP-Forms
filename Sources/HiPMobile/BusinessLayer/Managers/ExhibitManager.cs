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

using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;
using System;
using System.Collections.Generic;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers
{
    /// <summary>
    /// ExhibitManager class for getting access to exhibits and exhibitsets.
    /// </summary>
    public static class ExhibitManager
    {
        public static ReadExtensions Exhibits(this IReadOnlyDataAccess dataAccess) => new ReadExtensions(dataAccess);

        public static ReadWriteExtensions Exhibits(this ITransactionDataAccess dataAccess) => new ReadWriteExtensions(dataAccess);

        public class ReadExtensions
        {
            private readonly IReadOnlyDataAccess dataAccess;

            public ReadExtensions(IReadOnlyDataAccess dataAccess)
            {
                this.dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
            }

            /// <summary>
            /// Get an exhibit with a specific id including its image and the IDs of its pages.
            /// </summary>
            /// <param name="id">The id of the exhibit to be retrived.</param>
            /// <returns>The exhibit with the given id. If no exhibit exists, null is returned.</returns>
            public Exhibit GetExhibit(string id)
            {
                if (!string.IsNullOrEmpty(id))
                {
                    return dataAccess.GetItem<Exhibit>(id,
                        nameof(Exhibit.Image),
                        "PagesRefs.Page.Audio");
                }
                return null;
            }

            /// <summary>
            /// Gets all available exhibits including their image and the IDs of their pages.
            /// </summary>
            /// <returns>The enumerable of all available exhibits.</returns>
            public IEnumerable<Exhibit> GetExhibits()
            {
                return dataAccess.GetItems<Exhibit>(
                    nameof(Exhibit.Image),
                    "PagesRefs.Page.Audio");
            }
        }

        public class ReadWriteExtensions : ReadExtensions
        {
            private readonly ITransactionDataAccess dataAccess;

            public ReadWriteExtensions(ITransactionDataAccess dataAccess) : base(dataAccess)
            {
                this.dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
            }

            public void AddExhibit(Exhibit exhibit)
            {
                dataAccess.AddItem(exhibit);
            }

            /// <summary>
            /// Deletes the exhibit from the app.
            /// </summary>
            /// <param name="exhibit">The exhibit to be deleted.</param>
            /// <returns>True, if deletion was succesful, false otherwise.</returns>
            public bool DeleteExhibit(Exhibit exhibit)
            {
                if (exhibit != null)
                    dataAccess.DeleteItem(exhibit);

                return true;
            }
        }
    }
}