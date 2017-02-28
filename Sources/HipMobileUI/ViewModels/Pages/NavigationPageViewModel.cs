// Copyright (C) 2016 History in Paderborn App - Universität Paderborn
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//       http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.using System;

using System;
using System.Windows.Input;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using HipMobileUI.Helpers;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Xamarin.Forms;

namespace HipMobileUI.ViewModels.Pages {
    class NavigationPageViewModel : ViewModels.NavigationViewModel {

        private ExhibitSet exhibitSet;
        private GeoLocation gpsLocation;
        private Route detailsRoute;
        private bool showNavigation;

        //TODO just for testing can be deleted later
        public NavigationPageViewModel (Route route)
        {
            DetailsRoute = route;
            ShowNavigation = true;
            Title = "Navigation";
            gpsLocation = new GeoLocation (AppSharedData.PaderbornMainStation.Latitude,AppSharedData.PaderbornMainStation.Longitude);
            var locator = CrossGeolocator.Current;
            locator.PositionChanged += position_Changed;
            locator.StartListeningAsync (minTime: AppSharedData.MinTimeBwUpdates, minDistance: AppSharedData.MinDistanceChangeForUpdates);
            FocusGps = new Command(FocusGpsClicked);

        }


        // Callback function for when GPS location changes
        void position_Changed (object obj, PositionEventArgs e)
        {
            UpdateGpsData (e.Position);
        }

        // Update GPS location displays and database
        private void UpdateGpsData (Position position)
        {
            var newGpsLocation = new GeoLocation
            {
                Latitude = position.Latitude,
                Longitude = position.Longitude
            };
            GpsLocation = newGpsLocation;


        }

        void FocusGpsClicked ()
        {

        }


        public ExhibitSet ExhibitSet {
            get { return exhibitSet; }
            set { SetProperty (ref exhibitSet, value); }
        }

        public GeoLocation GpsLocation {
            get { return gpsLocation; }
            set { SetProperty (ref gpsLocation, value); }
        }

        public Route DetailsRoute {
            get { return detailsRoute; }
            set { SetProperty (ref detailsRoute, value); }
        }

        public bool ShowNavigation
        {
            get { return showNavigation; }
            set { SetProperty(ref showNavigation, value); }
        }

        public ICommand FocusGps { get; }

    }
}