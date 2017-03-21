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

using System;
using System.Collections.Generic;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using de.upb.hip.mobile.pcl.Common;
using de.upb.hip.mobile.pcl.Helpers;
using HipMobileUI.Helpers;
using HipMobileUI.Navigation;
using HipMobileUI.Resources;
using HipMobileUI.ViewModels.Pages;

namespace HipMobileUI.Location
{
    public delegate void ExhibitVisitedDelegate (object sender, Exhibit exhibit);
    public interface INearbyExhibitManager {

        event ExhibitVisitedDelegate ExhibitVisitedEvent;

        /// <summary>
        /// Opens an alert dialogue if the user is near to an exhibit
        /// </summary>
        /// <param name="exhibits">The exhibits to check</param>
        /// <param name="gpsLocation">The gps location of the user</param>
        /// <param name="considerTimeouts">Parameter indicating if timeouts for displaying the exhibit nearby message should be taken into account.</param>
        void CheckNearExhibit(IEnumerable<Exhibit> exhibits, GeoLocation gpsLocation, bool considerTimeouts);

    }

    public class NearbyExhibitManager : INearbyExhibitManager
    {
        private readonly TimeSpan dialogTimeout = TimeSpan.FromMinutes (20);
        public event ExhibitVisitedDelegate ExhibitVisitedEvent;

        public async void CheckNearExhibit (IEnumerable<Exhibit> exhibits, GeoLocation gpsLocation, bool considerTimeouts)
        {
            foreach (Exhibit e in exhibits)
            {
                var dist = MathUtil.CalculateDistance(e.Location, gpsLocation);
                if (dist < AppSharedData.ExhibitRadius)
                {
                    if (considerTimeouts)
                    {
                        DateTimeOffset now = DateTimeOffset.Now;
                        if (e.LastNearbyTime.HasValue)
                        {
                            if (now.Subtract (e.LastNearbyTime.Value) <= dialogTimeout)
                            {
                                // dialog for this exhibit was shown recently, skip it
                                continue;
                            }
                        }

                        // update the time the dialog was last shown
                        using (DbManager.StartTransaction ())
                        {
                            e.LastNearbyTime = now;
                        }
                    }

                    var result =
                        await
                            IoCManager.Resolve<INavigationService> ()
                                      .DisplayAlert (Strings.ExhibitNearby_ExhibitNearby, Strings.ExhibitNearby_Question_Part1 + " \"" + e.Name + "\" " +Strings.ExhibitNearby_Question_Part2,
                                                     Strings.ExhibitNearby_Confirm, Strings.ExhibitNearby_Reject);

                    if (result)
                    {
                        await IoCManager.Resolve<INavigationService>().PushAsync(new ExhibitDetailsViewModel(e.Id));
                        ExhibitVisitedEvent?.Invoke(this, e);
                        break;
                    }
                }
            }
        }

    }
}
