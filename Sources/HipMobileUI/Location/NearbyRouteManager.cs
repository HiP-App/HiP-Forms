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

using System;
using System.Collections.Generic;
using System.Linq;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Location
{

    public interface INearbyRouteManager
    {

        /// <summary>
        /// Opens a notification if the user is near a route
        /// </summary>
        /// <param name="routes">The routes to check</param>
        /// <param name="gpsLocation">The gps location of the user</param>
        void CheckNearRoute(IEnumerable<Route> routes, GeoLocation gpsLocation);

    }

    public class NearbyRouteManager : INearbyRouteManager
    {

        public async void CheckNearRoute(IEnumerable<Route> routes, GeoLocation gpsLocation)    // Optimize for later: just send non-visited routes
        {
            foreach (Route r in routes)
            {
                var dist = MathUtil.CalculateDistance (r.Waypoints.First ().Location, gpsLocation);
                if (dist < AppSharedData.RouteRadius)
                {
                    DateTimeOffset now = DateTimeOffset.Now;
                    if (r.LastTimeDismissed.HasValue)
                    {
                        if (now.Subtract (r.LastTimeDismissed.Value) <= TimeSpan.FromMinutes (30))
                        {
                            continue; // This route was dismissed in the last 30 minutes
                        }
                    }

                    var result =
                        await IoCManager.Resolve<INavigationService> ()
                                        .DisplayAlert (Strings.RouteNearby, Strings.ExhibitOrRouteNearby_Question_Part1 + " \"" + r.Title + "\" " + Strings.ExhibitOrRouteNearby_Question_Part2,
                                                        Strings.ExhibitOrRouteNearby_Confirm, Strings.ExhibitOrRouteNearby_Reject);

                    if (result) // Switch to details
                    {
                        using (DbManager.StartTransaction())
                        {
                            r.LastTimeDismissed = now;
                        }
                        await IoCManager.Resolve<INavigationService> ().PushAsync (new RouteDetailsPageViewModel (r.Id));
                        return;
                    }
                    else // Dismissed
                    {
                        using (DbManager.StartTransaction())
                        {
                            r.LastTimeDismissed = now;
                        }
                    }
                }
            }
        }

    }
}