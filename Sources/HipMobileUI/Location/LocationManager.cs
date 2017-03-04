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
using HipMobileUI.Helpers;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Xamarin.Forms;

namespace HipMobileUI.Location
{
    public interface ILocationListener {

        void LocationChanged (object sender, PositionEventArgs args);

    }

    public interface ILocationManager {

        void AddLocationListener (ILocationListener listener);

        void RemoveLocationListener (ILocationListener listener);

        Position LastKnownLocation { get; }

        void PauseListening ();

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

            MessagingCenter.Subscribe<App>(this, AppSharedData.WillSleepMessage, WillSleep);
            MessagingCenter.Subscribe<App>(this, AppSharedData.WillWakeUpMessage, WillWakeUp);
        }

        public void AddLocationListener (ILocationListener listener)
        {
            if (listener != null)
            {
                bool needToStartLocator = !locator.IsListening && registeredListeners.Count == 0 ;
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
                locator.PositionChanged -= listener.LocationChanged;
                registeredListeners.Remove (listener);
                if (locator.IsListening && registeredListeners.Count == 0)
                {
                    locator.StopListeningAsync ();
                }
            }
        }

        public Position LastKnownLocation { get; private set; }
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
                locator.StartListeningAsync(AppSharedData.MinTimeBwUpdates, AppSharedData.MinDistanceChangeForUpdates);
            }
        }

        /// <summary>
        /// Called when the app will go to the background or the screen is locked.
        /// </summary>
        /// <param name="obj">The caller.</param>
        private void WillSleep(App obj)
        {
            PauseListening ();
        }

        /// <summary>
        /// Called when the app will wake up.
        /// </summary>
        /// <param name="obj">The sender of the event.</param>
        private void WillWakeUp(App obj)
        {
            StartListening ();
        }

    }
}
