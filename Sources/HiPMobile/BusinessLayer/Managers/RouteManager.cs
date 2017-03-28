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

using System.Collections.Generic;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers {
    public static class RouteManager {

        private static readonly IDataAccess DataAccess = IoCManager.Resolve<IDataAccess> ();

        /// <summary>
        ///     Returns a Route, with specific id
        /// </summary>
        /// <param name="id">The id of the specific Route to be passed</param>
        /// <returns>the Route with given id. If Route does not exits, return null</returns>
        public static Route GetRoute (string id)
        {
            if (!string.IsNullOrEmpty (id))
            {
                return DataAccess.GetItem<Route> (id);
            }
            return null;
        }

        /// <summary>
        ///     Returns all existing Routes
        /// </summary>
        /// <returns>The enumerable of all avaible routes</returns>
        public static IEnumerable<Route> GetRoutes ()
        {
            return DataAccess.GetItems<Route> ();
        }

        /// <summary>
        ///     Deletes the Route
        /// </summary>
        /// <param name="route"> The Route to be deleted</param>
        /// <returns>true, if deletion was sucessfull, false otherwise</returns>
        public static bool DeleteRoute (Route route)
        {
            if (route != null)
            {
                return DataAccess.DeleteItem<Route> (route.Id);
            }
            return true;
        }

        /// <summary>
        ///     Returns a RouteSet, with specific id
        /// </summary>
        /// <param name="id">The id of the specific RouteSet to be passed</param>
        /// <returns></returns>
        public static RouteSet GetRouteSet (string id)
        {
            if (!string.IsNullOrEmpty (id))
            {
                return DataAccess.GetItem<RouteSet> (id);
            }
            return null;
        }

        /// <summary>
        ///     Returns all existing RouteSets
        /// </summary>
        /// <returns>The enumerable of all avaible route sets</returns>
        public static IEnumerable<RouteSet> GetRouteSets ()
        {
            return DataAccess.GetItems<RouteSet> ();
        }

        /// <summary>
        ///     Deletes the RouteSet
        /// </summary>
        /// <param name="routeSet"> The RouteSet to be deleted</param>
        /// <returns>true, if deletion was sucessfull, false otherwise</returns>
        public static bool DeleteRouteSet (RouteSet routeSet)
        {
            if (routeSet != null)
            {
                return DataAccess.DeleteItem<RouteSet> (routeSet.Id);
            }
            return true;
        }

    }
}