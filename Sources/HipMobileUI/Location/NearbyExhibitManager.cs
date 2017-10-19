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
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.UI.NotificationPlayer;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Location
{
    public delegate void ExhibitVisitedDelegate(object sender, Exhibit exhibit);

    public interface INearbyExhibitManager
    {
        event ExhibitVisitedDelegate ExhibitVisitedEvent;

        /// <summary>
        /// Opens an alert dialogue if the user is near to an exhibit
        /// </summary>
        /// <param name="exhibits">The exhibits to check</param>
        /// <param name="gpsLocation">The gps location of the user</param>
        /// <param name="considerTimeouts">Parameter indicating if timeouts for displaying the exhibit nearby message should be taken into account.</param>
        /// <param name="appMinimized">True if the method was called while the app is minimized.</param>
        void CheckNearExhibit(IEnumerable<Exhibit> exhibits, GeoLocation gpsLocation, bool considerTimeouts, bool appMinimized);

        void InvokeExhibitVistedEvent(Exhibit exhibit);
    }

    public class NearbyExhibitManager : INearbyExhibitManager
    {
        private readonly TimeSpan dialogTimeout = TimeSpan.FromMinutes(1);
        public event ExhibitVisitedDelegate ExhibitVisitedEvent;

        public async void CheckNearExhibit(IEnumerable<Exhibit> exhibits, GeoLocation gpsLocation, bool considerTimeouts, bool appMinimized)
        {
            foreach (Exhibit e in exhibits)
            {
                var dist = MathUtil.CalculateDistance(e.Location, gpsLocation);
                if (dist < AppSharedData.ExhibitRadius)
                {
                    using (IoCManager.Resolve<IDataAccess>().StartTransaction())
                    {
                        e.Unlocked = true;
                    }
                    if (considerTimeouts)
                    {
                        DateTimeOffset now = DateTimeOffset.Now;
                        if (e.LastNearbyTime.HasValue)
                        {
                            if (now.Subtract(e.LastNearbyTime.Value) <= dialogTimeout)
                            {
                                // dialog for this exhibit was shown recently, skip it
                                continue;
                            }
                        }

                        // update the time the dialog was last shown
                        using (DbManager.StartTransaction())
                        {
                            e.LastNearbyTime = now;
                        }
                    }

                    // Display popup page or local notification
                    if (appMinimized)
                    {
                        var notificationPlayer = IoCManager.Resolve<INotificationPlayer>();
                        notificationPlayer.DisplaySimpleNotification(e.Name, "Es befindet sich eine Sehenswürdigkeit in der Nähe. Geh sie besuchen!", e.Image.Data);
                    }
                    else
                    {
                        NavigationViewModel nv = new ExhibitPreviewViewModel(e, this);
                        await
                            IoCManager.Resolve<INavigationService>()
                                      .PushModalAsync(nv);
                    }
                }
            }
        }

        public void InvokeExhibitVistedEvent(Exhibit exhibit)
        {
            ExhibitVisitedEvent?.Invoke(this, exhibit);
        }
    }
}