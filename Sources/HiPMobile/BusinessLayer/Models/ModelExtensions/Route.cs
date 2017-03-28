// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
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
using System.Linq;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;
using Realms;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models
{
    public partial class Route {

        [Ignored]
        public IList<Waypoint> ActiveSet => Waypoints.Where (wp => !wp.Visited).ToList ();

        [Ignored]
        public IList<Waypoint> PassiveSet => Waypoints.Where(wp => wp.Visited).ToList();

        /// <summary>
        /// Tries to move the waypoint to the passive set.
        /// </summary>
        /// <param name="waypoint">The waypoint to move.</param>
        /// <returns>True, if the waypoint could be moved. False otherwise.</returns>
        public bool MoveToPassiveSet (Waypoint waypoint)
        {
            if (waypoint != null)
            {
                using (IoCManager.Resolve<IDataAccess>().StartTransaction ())
                {
                    bool exists = ActiveSet.Contains (waypoint);
                    waypoint.Visited = true;
                    if (exists)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Moves all waypoints from the passive set to the active set.
        /// </summary>
        public void ResetRoute ()
        {
            using (IoCManager.Resolve<IDataAccess> ().StartTransaction ())
            {
                foreach (Waypoint waypoint in PassiveSet)
                {
                    waypoint.Visited = false;
                }
            }
        }

        /// <summary>
        /// Indicates if the route has been started.
        /// </summary>
        /// <returns></returns>
        public bool IsRouteStarted ()
        {
            return PassiveSet != null && PassiveSet.Count > 0;
        }

        /// <summary>
        /// Indicates if the route has been finished.
        /// </summary>
        /// <returns></returns>
        public bool IsRouteFinished ()
        {
            return ActiveSet == null || ActiveSet.Count == 0;
        }

    }
}
