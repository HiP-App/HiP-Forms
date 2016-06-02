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
using System.Collections.Generic;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using de.upb.hip.mobile.pcl.Common;
using de.upb.hip.mobile.pcl.DataAccessLayer;
using Microsoft.Practices.Unity;

namespace de.upb.hip.mobile.pcl.BusinessLayer.Managers {
    /// <summary>
    /// ExhibitManager class for getting access to exhibits and exhibitsets.
    /// </summary>
    public static class ExhibitManager {

        private static readonly IDataAccess dataAccess = IoCManager.UnityContainer.Resolve<IDataAccess> ();

        /// <summary>
        /// Gets an exhibitset with a specific id.
        /// </summary>
        /// <param name="id">The id of the exhibitset to be retrived.</param>
        /// <returns>The exhibitSet with the given id. If it doesn't exist, null is returned.</returns>
        public static ExhibitSet GetExhibitSet (string id)
        {
            if (!string.IsNullOrEmpty (id))
            {
                return dataAccess.GetItem<ExhibitSet> (id);
            }
            return null;
        }

        /// <summary>
        /// Get all available exhibitsets.
        /// </summary>
        /// <returns>The enumerable of all available exibitsets.</returns>
        public static IEnumerable<ExhibitSet> GetExhibitSets ()
        {
            return dataAccess.GetItems<ExhibitSet> ();           
        }

        /// <summary>
        /// Deletes the exhibitSet from the app.
        /// </summary>
        /// <param name="exhibitSet">The exhibitSet to be deleted.</param>
        /// <returns>True, if deletion was succesful, false otherwise.</returns>
        public static bool DeleteExhibitSet(ExhibitSet exhibitSet)
        {
            if (exhibitSet != null)
            {
                return dataAccess.DeleteItem<ExhibitSet>(exhibitSet.Id);
            }
            return true;
        }

        /// <summary>
        /// Get an exhibitSet with a specific id.
        /// </summary>
        /// <param name="id">The id of the exhibitSet to be retrived.</param>
        /// <returns>The exhibitSet with the given id. If no exhibitSet exists, null is returned.</returns>
        public static Exhibit GetExhibit (string id)
        {
            if (string.IsNullOrEmpty (id))
            {
                return dataAccess.GetItem<Exhibit> (id);
            }
            return null;
        }

        /// <summary>
        /// Gets all available exhibits.
        /// </summary>
        /// <returns>The enumerable of all available exhibits.</returns>
        public static IEnumerable<Exhibit> GetExhibits ()
        {
            return dataAccess.GetItems<Exhibit> ();
        }

        /// <summary>
        /// Deletes the exhibitSet from the app.
        /// </summary>
        /// <param name="exhibit">The exhibitSet to be deleted.</param>
        /// <returns>True, if deletion was succesful, false otherwise.</returns>
        public static bool DeleteExhibit(Exhibit exhibit)
        {
            if (exhibit != null)
            {
                return dataAccess.DeleteItem<Exhibit>(exhibit.Id);
            }
            return true;
        }

    }
}