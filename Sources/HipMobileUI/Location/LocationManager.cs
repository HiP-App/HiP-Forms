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
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using de.upb.hip.mobile.pcl.Common;
using de.upb.hip.mobile.pcl.Helpers;
using HipMobileUI.Helpers;
using HipMobileUI.Navigation;
using HipMobileUI.ViewModels.Pages;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Xamarin.Forms;

namespace HipMobileUI.Location {
    public interface ILocationListener {

        /// <summary>
        /// Called when the location changed.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="args">The event parameters including the lcoation.</param>
        void LocationChanged (object sender, PositionEventArgs args);

    }

    public interface ILocationManager {

        /// <summary>
        /// Adds a location listener which will receive location updates.
        /// If it was the firtst listener added, location updates are turned on.
        /// </summary>
        /// <param name="listener">The listener to add.</param>
        void AddLocationListener (ILocationListener listener);

        /// <summary>
        /// Removes a location listener which will no longer receive location updates.
        /// If the last listener was removed, location updates are turned off.
        /// </summary>
        /// <param name="listener">The listener to remove.</param>
        void RemoveLocationListener (ILocationListener listener);


        /// <summary>
        /// Opens an alert dialogue if the user is near to an exhibit
        /// </summary>
        /// <param name="exhibitSet">The set of exhibits that should be checked</param>
        /// <param name="route">The Exhibits of a route that should be checked</param>
        /// <param name="gpsLocation">The user location</param>
        void CheckNearExhibit (ExhibitSet exhibitSet, Route route, GeoLocation gpsLocation);


        /// <summary>
        /// THe last known location. Might be null in the beginning.
        /// </summary>
        Position LastKnownLocation { get; }

        /// <summary>
        /// Gets whether the geolocation is available. The value might be a false positive on an emulator/simulator.
        /// </summary>
        /// <returns>True if the location is available, false otherwise.</returns>
        bool IsLocationAvailable ();

        /// <summary>
        /// Pauses location updates manually.
        /// </summary>
        void PauseListening ();

        /// <summary>
        /// Starts location updates manually.
        /// </summary>
        void StartListening ();

    }

    public class LocationManager : ILocationManager {

        private readonly IGeolocator locator;
        private readonly List<ILocationListener> registeredListeners;

        public LocationManager ()
        {
            registeredListeners = new List<ILocationListener> ();
            locator = CrossGeolocator.Current;
            locator.PositionChanged += (sender, args) => { LastKnownLocation = args.Position; };

            // subscribe to events when the app sleeps/wakes up to enable7disable location updates
            MessagingCenter.Subscribe<App> (this, AppSharedData.WillSleepMessage, WillSleep);
            MessagingCenter.Subscribe<App> (this, AppSharedData.WillWakeUpMessage, WillWakeUp);
        }


        public async void CheckNearExhibit (ExhibitSet exhibitSet, Route route, GeoLocation gpsLocation)
        {
            double dist;
            if (exhibitSet != null)
            {
                foreach (Exhibit e in exhibitSet)
                {
                    dist = MathUtil.CalculateDistance (e.Location, gpsLocation);
                    if (dist < AppSharedData.ExhibitRadius)
                    {
                        var result =
                            await
                                IoCManager.Resolve<INavigationService> ()
                                          .DisplayAlert ("Sehenwürdigkeit in der Nähe", "Möchten sie sich " + e.Name + " genauer ansehen", "Ja", "Nein");

                        if (result)
                        {
                            await IoCManager.Resolve<INavigationService> ().PushAsync (new ExhibitDetailsViewModel (e.Id));
                            break;
                        }
                    }
                }
            }
            else if (route != null)
            {
                foreach (Waypoint r in route.Waypoints)
                {
                    dist = MathUtil.CalculateDistance (r.Location, gpsLocation);
                    if (dist < AppSharedData.ExhibitRadius)
                    {
                        var result =
                            await
                                IoCManager.Resolve<INavigationService> ()
                                          .DisplayAlert ("Sehenwürdigkeit in der Nähe", "Möchten sie sich " + r.Exhibit.Name + " genauer ansehen", "Ja", "Nein");

                        if (result)
                        {
                            await IoCManager.Resolve<INavigationService> ().PushAsync (new ExhibitDetailsViewModel (r.Exhibit.Id));
                            break;
                        }
                    }
                }
            }
        }


        public void AddLocationListener (ILocationListener listener)
        {
            if (listener != null)
            {
                // remember the listener and start location updates if necessary
                bool needToStartLocator = !locator.IsListening && registeredListeners.Count == 0;
                registeredListeners.Add (listener);
                locator.PositionChanged += listener.LocationChanged;
                if (needToStartLocator)
                {
                    locator.StartListeningAsync (AppSharedData.MinTimeBwUpdates, AppSharedData.MinDistanceChangeForUpdates);
                }
            }
        }

        public void RemoveLocationListener (ILocationListener listener)
        {
            if (listener != null)
            {
                // remove the listener and stop location updates if necessary
                locator.PositionChanged -= listener.LocationChanged;
                registeredListeners.Remove (listener);
                if (locator.IsListening && registeredListeners.Count == 0)
                {
                    locator.StopListeningAsync ();
                }
            }
        }

        public Position LastKnownLocation { get; private set; }

        public bool IsLocationAvailable ()
        {
            return locator.IsGeolocationAvailable;
        }

        public void PauseListening ()
        {
            if (locator.IsListening)
            {
                locator.StopListeningAsync ();
            }
        }

        public void StartListening ()
        {
            if (!locator.IsListening)
            {
                locator.StartListeningAsync (AppSharedData.MinTimeBwUpdates, AppSharedData.MinDistanceChangeForUpdates);
            }
        }

        /// <summary>
        /// Called when the app will go to the background or the screen is locked.
        /// </summary>
        /// <param name="obj">The caller.</param>
        private void WillSleep (App obj)
        {
            PauseListening ();
        }

        /// <summary>
        /// Called when the app will wake up.
        /// </summary>
        /// <param name="obj">The sender of the event.</param>
        private void WillWakeUp (App obj)
        {
            StartListening ();
        }

    }
}