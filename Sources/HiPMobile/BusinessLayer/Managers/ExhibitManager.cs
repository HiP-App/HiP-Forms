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
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;
using System.Collections.Generic;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers
{
    /// <summary>
    /// ExhibitManager class for getting access to exhibits and exhibitsets.
    /// </summary>
    public static class ExhibitManager
    {
        private static readonly IDataAccess DataAccess = IoCManager.Resolve<IDataAccess>();
        
        /// <summary>
        /// Get an exhibit with a specific id.
        /// </summary>
        /// <param name="id">The id of the exhibit to be retrived.</param>
        /// <returns>The exhibit with the given id. If no exhibit exists, null is returned.</returns>
        public static Exhibit GetExhibit(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                return DataAccess.GetItem<Exhibit>(id);
            }
            return null;
        }

        /// <summary>
        /// Gets all available exhibits.
        /// </summary>
        /// <returns>The enumerable of all available exhibits.</returns>
        public static IEnumerable<Exhibit> GetExhibits()
        {
            return DataAccess.GetItems<Exhibit>();
        }

        /// <summary>
        /// Deletes the exhibit from the app.
        /// </summary>
        /// <param name="exhibit">The exhibit to be deleted.</param>
        /// <returns>True, if deletion was succesful, false otherwise.</returns>
        public static bool DeleteExhibit(Exhibit exhibit)
        {
            if (exhibit != null)
            {
                return DataAccess.DeleteItem<Exhibit>(exhibit.Id);
            }
            return true;
        }
    }
}