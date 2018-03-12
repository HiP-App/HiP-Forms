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

        /// <summary>
        /// Opens the details view of the route with the given id
        /// </summary>
        /// <param name="routeId">The ID of the route for the details view</param>
        void OpenRouteDetailsView(string routeId);

        /// <summary>
        /// Closes the pop-up for the route preview
        /// </summary>
        void ClosePopUp();
    }

    public class NearbyRouteManager : INearbyRouteManager
    {
        private bool popupActive;

        public async void CheckNearRoute(IEnumerable<Route> routes, GeoLocation gpsLocation) // Optimize for later: just send non-visited routes
        {
            if (popupActive)
                return;

            foreach (var r in routes)
            {
                // TODO: Add condition "r.Waypoints.Count == 0 || ..."
                if (!(MathUtil.CalculateDistance(r.Waypoints.First().Location, gpsLocation) < AppSharedData.RouteRadius))
                    continue;

                var now = DateTimeOffset.Now;
                if (r.LastTimeDismissed.HasValue)
                {
                    if (now.Subtract(r.LastTimeDismissed.Value) <= TimeSpan.FromMinutes(30))
                    {
                        // This route was dismissed in the last 30 minutes; don't show it again yet
                        continue;
                    }
                }

                await IoCManager.Resolve<INavigationService>().PushModalAsync(new RoutePreviewPageViewModel(r, this));
                popupActive = true;

                using (DbManager.StartTransaction())
                {
                    r.LastTimeDismissed = now;
                }
            }
        }

        public void OpenRouteDetailsView(string routeId)
        {
            IoCManager.Resolve<INavigationService>().PushAsync(new RouteDetailsPageViewModel(routeId));
        }

        public void ClosePopUp()
        {
            IoCManager.Resolve<INavigationService>().PopModalAsync();
            popupActive = false;
        }
    }
}