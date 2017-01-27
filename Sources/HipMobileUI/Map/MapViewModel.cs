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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using de.upb.hip.mobile.pcl.Helpers;
using HipMobileUI.Helpers;
using HipMobileUI.ViewModels;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Xamarin.Forms;

namespace HipMobileUI.Map {
    class MapViewModel : NavigationViewModel {

        private ExhibitSet exhibitSet;
        private GeoLocation gpsLocation;
        private Route detailsRoute;
        private bool showDetailsRoute;

        //TODO just for testing can be deleted later
        public MapViewModel ()
        {
           
            ShowDetailsRoute = false;
            Title = "Map";
            ExhibitSet = ExhibitManager.GetExhibitSets ().First ();
            gpsLocation = new GeoLocation (AppSharedData.PaderbornMainStation.Latitude,AppSharedData.PaderbornMainStation.Longitude);
            var locator = CrossGeolocator.Current;
            locator.PositionChanged += position_Changed;
            locator.StartListeningAsync (minTime: AppSharedData.MinTimeBwUpdates, minDistance: AppSharedData.MinDistanceChangeForUpdates);
           

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
            ExhibitPopUp ();

        }

        private void ExhibitPopUp ()
        {
            double dist;
            foreach (Exhibit e in exhibitSet)
            {
                dist = ExtensionMethods.DistanceLatLon (e.Location, gpsLocation);
                if (dist < 30)
                {
                    var b = Navigation.DisplayAlert ("Sehenwürdigkeit in der Nähe", "Möchten sie sich " + e.Name + " genauer ansehen", "Ja", "Nein");
            
                        //TODO go to exhibit
                    break;
                }
            }
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

        public bool ShowDetailsRoute
        {
            get { return showDetailsRoute; }
            set { SetProperty(ref showDetailsRoute, value); }
        }



    }
}