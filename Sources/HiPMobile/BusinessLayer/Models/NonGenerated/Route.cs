﻿// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
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
using de.upb.hip.mobile.pcl.Common;
using de.upb.hip.mobile.pcl.DataAccessLayer;

namespace de.upb.hip.mobile.pcl.BusinessLayer.Models
{
    public partial class Route {

        public IList<Waypoint> Waypoints => ActiveSet.Concat (PassiveSet).ToList ();

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
                    bool exists = ActiveSet.Remove(waypoint);
                    if (exists)
                    {
                        PassiveSet.Add(waypoint);
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
                    ActiveSet.Add (waypoint);
                }
                PassiveSet.Clear ();
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
