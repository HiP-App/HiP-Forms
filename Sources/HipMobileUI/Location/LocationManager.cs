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
using PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Location
{
    public interface ILocationListener
    {
        /// <summary>
        /// Called when the location changed.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="args">The event parameters including the lcoation.</param>
        void LocationChanged(object sender, PositionEventArgs args);
    }

    public interface ILocationManager
    {
        /// <summary>
        /// Adds a location listener which will receive location updates.
        /// If it was the firtst listener added, location updates are turned on.
        /// </summary>
        /// <param name="listener">The listener to add.</param>
        void AddLocationListener(ILocationListener listener);

        /// <summary>
        /// Removes a location listener which will no longer receive location updates.
        /// If the last listener was removed, location updates are turned off.
        /// </summary>
        /// <param name="listener">The listener to remove.</param>
        void RemoveLocationListener(ILocationListener listener);

        /// <summary>
        /// THe last known location. Might be null in the beginning.
        /// </summary>
        Position LastKnownLocation { get; }

        /// <summary>
        /// Gets whether the geolocation is available. The value might be a false positive on an emulator/simulator.
        /// </summary>
        /// <returns>True if the location is available, false otherwise.</returns>
        bool IsLocationAvailable();

        /// <summary>
        /// Indicates whether the manager listens to position changes in the background (app minimized)
        /// </summary>
        /// <returns>True if app in background, false otherwise.</returns>
        bool ListeningInBackground { get; }
    }

    public class LocationManager : ILocationManager
    {
        private readonly IGeolocator locator;
        private readonly List<ILocationListener> registeredListeners;

        public LocationManager()
        {
            registeredListeners = new List<ILocationListener>();
            locator = CrossGeolocator.Current;
            locator.PositionChanged += (sender, args) => { LastKnownLocation = args.Position; };

            // subscribe to events when the app sleeps/wakes up to enable/disable location updates
            MessagingCenter.Subscribe<App>(this, AppSharedData.WillSleepMessage, WillSleep);
            MessagingCenter.Subscribe<App>(this, AppSharedData.WillWakeUpMessage, WillWakeUp);
        }

        public void AddLocationListener(ILocationListener listener)
        {
            if (listener != null)
            {
                // remember the listener and start location updates if necessary
                var needToStartLocator = !locator.IsListening && registeredListeners.Count == 0;
                registeredListeners.Add(listener);
                locator.PositionChanged += listener.LocationChanged;
                if (needToStartLocator)
                {
                    locator.StartListeningAsync(
                        TimeSpan.FromMilliseconds(AppSharedData.MinTimeBwUpdates),
                        AppSharedData.MinDistanceChangeForUpdates);
                }
            }
        }

        public void RemoveLocationListener(ILocationListener listener)
        {
            if (listener != null)
            {
                // remove the listener and stop location updates if necessary
                locator.PositionChanged -= listener.LocationChanged;
                registeredListeners.Remove(listener);
                if (locator.IsListening && registeredListeners.Count == 0)
                {
                    locator.StopListeningAsync();
                }
            }
        }

        public Position LastKnownLocation { get; private set; }

        public bool IsLocationAvailable()
        {
            return locator.IsGeolocationAvailable;
        }

        public bool ListeningInBackground { get; private set; }

        /// <summary>
        /// Called when the app will go to the background or the screen is locked.
        /// </summary>
        /// <param name="obj">The caller.</param>
        private void WillSleep(App obj)
        {
            ListeningInBackground = true;
        }

        /// <summary>
        /// Called when the app will wake up.
        /// </summary>
        /// <param name="obj">The sender of the event.</param>
        private void WillWakeUp(App obj)
        {
            ListeningInBackground = false;
        }
    }
}