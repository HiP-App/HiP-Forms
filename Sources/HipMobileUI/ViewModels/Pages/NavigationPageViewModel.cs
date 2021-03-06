﻿// Copyright (C) 2016 History in Paderborn App - Universität Paderborn
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

using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Location;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using Plugin.Geolocator.Abstractions;
using Xamarin.Forms;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages
{
    public class NavigationPageViewModel : NavigationViewModel, ILocationListener
    {
        private GeoLocation gpsLocation;
        private readonly ILocationManager locationManager;
        private readonly INearbyExhibitManager nearbyExhibitManager;
        private Route detailsRoute;

        private bool showNavigation;

        private ICommand mapFocusCommand;

        public NavigationPageViewModel(Route route)
        {
            DetailsRoute = route;
            ShowNavigation = true;
            Title = Strings.NavigationPageViewModel_Title;
            locationManager = IoCManager.Resolve<ILocationManager>();
            nearbyExhibitManager = IoCManager.Resolve<INearbyExhibitManager>();
            FocusGps = new Command(FocusGpsClicked);
            SkipExhibit = new Command(SkipExhibitClicked);
        }

        /// <summary>
        /// Handles the button click
        /// </summary>
        private void FocusGpsClicked()
        {
            MapFocusCommand.Execute(GpsLocation);
        }

        private async void SkipExhibitClicked()
        {
            var exhibits = detailsRoute.ActiveSet.Select(waypoint => waypoint.Exhibit);

            if (!detailsRoute.ActiveSet.Any())
            {
                await Navigation.DisplayAlert(Strings.SkipExhibit_Title, Strings.SkipExhibit_Message, Strings.Ok);
            }
            else
            {
                SkipExhibitVisited(exhibits);
            }
        }

        public GeoLocation GpsLocation
        {
            get { return gpsLocation; }
            set { SetProperty(ref gpsLocation, value); }
        }

        public Route DetailsRoute
        {
            get { return detailsRoute; }
            set { SetProperty(ref detailsRoute, value); }
        }

        public bool ShowNavigation
        {
            get { return showNavigation; }
            set { SetProperty(ref showNavigation, value); }
        }

        public ICommand FocusGps { get; }

        public ICommand SkipExhibit { get; }

        public ICommand MapFocusCommand
        {
            get { return mapFocusCommand; }
            set { SetProperty(ref mapFocusCommand, value); }
        }

        public void LocationChanged(object sender, PositionEventArgs args)
        {
            GpsLocation = args.Position.ToGeoLocation();
            nearbyExhibitManager.CheckNearExhibit(detailsRoute.ActiveSet.Select(waypoint => waypoint.Exhibit), GpsLocation, false, locationManager.ListeningInBackground);
        }

        public override void OnAppearing()
        {
            base.OnAppearing();

            locationManager.AddLocationListener(this);
            nearbyExhibitManager.ExhibitVisitedEvent += ExhibitVisited;
        }

        private async void ExhibitVisited(object sender, Exhibit exhibit)
        {
            var waypoint = DetailsRoute.Waypoints.First(wp => Equals(wp.Exhibit, exhibit));
            var moved = await DetailsRoute.MoveToPassiveSet(waypoint);
            if (moved)
            {
                DbManager.InTransaction(transaction =>
                {
                    exhibit.Unlocked = true;
                });
                OnPropertyChanged(nameof(DetailsRoute));
            }
        }

        private async void SkipExhibitVisited(IEnumerable<Exhibit> exhibits)
        {
            var e = exhibits.First();
            var result =
                await
                    IoCManager.Resolve<INavigationService>()
                              .DisplayAlert(Strings.SkipExhibit_Title, Strings.SkipExhibit_Question_Part1 + " \"" + e.Name + "\" " + Strings.SkipExhibit_Question_Part2,
                                            Strings.Yes, Strings.No);

            if (result)
            {
                ExhibitVisited(this, e);
            }
        }

        public override async void OnDisappearing()
        {
            base.OnDisappearing();

            locationManager.RemoveLocationListener(this);

            if (DetailsRoute.IsRouteFinished())
            {
                await DetailsRoute.ResetRoute();
            }

            nearbyExhibitManager.ExhibitVisitedEvent -= ExhibitVisited;
        }
    }
}