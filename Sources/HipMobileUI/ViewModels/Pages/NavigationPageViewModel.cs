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

using System.Linq;
using System.Windows.Input;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Location;
using Plugin.Geolocator.Abstractions;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages {
    class NavigationPageViewModel : NavigationViewModel, ILocationListener {

        private ExhibitSet exhibitSet;
        private GeoLocation gpsLocation;
        private ILocationManager locationManager;
        private INearbyExhibitManager nearbyExhibitManager;
        private Route detailsRoute;

        private bool showNavigation;

        private ICommand mapFocusCommand;

        public NavigationPageViewModel (Route route)
        {
            DetailsRoute = route;
            ShowNavigation = true;
            Title = "Navigation";
            locationManager = IoCManager.Resolve<ILocationManager> ();
            nearbyExhibitManager = IoCManager.Resolve<INearbyExhibitManager> ();
            FocusGps = new Command(FocusGpsClicked);
            SkipExhibit = new Command(SkipExhibitClicked);
        }

        /// <summary>
        /// Handles the button click
        /// </summary>
        void FocusGpsClicked ()
        {
            MapFocusCommand.Execute (GpsLocation);
        }

        void SkipExhibitClicked ()
        {
            // TODO: Call ExhibitVisited (object sender, Exhibit exhibit) -> exhibit is missing!
            return;
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

        public ICommand SkipExhibit { get; }

        public ICommand MapFocusCommand {
            get { return mapFocusCommand; }
            set { SetProperty (ref mapFocusCommand, value); }
        }

        public void LocationChanged (object sender, PositionEventArgs args)
        {
            GpsLocation = args.Position.ToGeoLocation();
            nearbyExhibitManager.CheckNearExhibit (detailsRoute.ActiveSet.Select (waypoint => waypoint.Exhibit), GpsLocation, false);
        }

        public override void OnAppearing ()
        {
            base.OnAppearing ();

            locationManager.AddLocationListener (this);
            nearbyExhibitManager.ExhibitVisitedEvent+=ExhibitVisited;
        }

        private void ExhibitVisited (object sender, Exhibit exhibit)
        {
            Waypoint waypoint = DetailsRoute.Waypoints.First (wp => Equals (wp.Exhibit, exhibit));
            bool moved = DetailsRoute.MoveToPassiveSet (waypoint);
            if (moved)
            {
                OnPropertyChanged (nameof(DetailsRoute));
            }
        }

        public override void OnHidden ()
        {
            base.OnHidden ();

            locationManager.RemoveLocationListener(this);
        }

        public override void OnRevealed ()
        {
            base.OnRevealed ();

            locationManager.AddLocationListener(this);
        }

        public override void OnDisappearing ()
        {
            base.OnDisappearing ();

            locationManager.RemoveLocationListener (this);

            if (DetailsRoute.IsRouteFinished ())
            {
                DetailsRoute.ResetRoute ();
            }
            nearbyExhibitManager.ExhibitVisitedEvent -= ExhibitVisited;
        }

    }
}