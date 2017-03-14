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

using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using de.upb.hip.mobile.pcl.Common;
using HipMobileUI.Helpers;
using HipMobileUI.Location;
using HipMobileUI.Resources;
using HipMobileUI.ViewModels;
using HipMobileUI.ViewModels.Pages;
using Xamarin.Forms;

namespace HipMobileUI.Viewmodels.Pages {
    /// <summary>
    /// ViewModel for the RouteDetailsPage.
    /// </summary>
    public class RouteDetailsPageViewModel : NavigationViewModel {

        private GeoLocation gpsLocation;
        private Route detailsRoute;
        private bool showDetailsRoute;

        /// <summary>
        /// Creates a new ViewModel for the route with the specified ID.
        /// Fetches the corresponding <see cref="Route"/> via the <see cref="RouteManager"/>
        /// and passes it to the alternative constructor.
        /// </summary>
        /// <param name="id">The ID of the route the ViewModel is created for.</param>
        public RouteDetailsPageViewModel (string id) : this (RouteManager.GetRoute (id))
        {
            // Intentionally left blank
        }

        /// <summary>
        /// Creates a new ViewModel for the specified <see cref="Route"/>.
        /// </summary>
        /// <param name="route">The <see cref="Route"/> the ViewModel is created for.</param>
        /// <param name="location"></param>
        public RouteDetailsPageViewModel (Route route)
        {
            Title = route.Title;
            Description = route.Description;
            Distance = string.Format (Strings.RouteDetailsPageViewModel_Distance, route.Distance);
            Duration = string.Format (Strings.RouteDetailsPageViewModel_Duration, route.Duration/60);
            Tags = new ObservableCollection<RouteTag> (route.RouteTags);
            var data = route.Image.Data;
            Image = ImageSource.FromStream (() => new MemoryStream (data));
            StartRouteCommand = new Command (StartRoute);
            StartDescriptionPlaybackCommand = new Command (StartDescriptionPlayback);
            Tabs = new ObservableCollection<string> {"Description", "Map"};
            GpsLocation = IoCManager.Resolve<ILocationManager> ().LastKnownLocation.ToGeoLocation ();
            DetailsRoute = route;
            ShowDetailsRoute = true;

        }

        /// <summary>
        /// Starts audio playback for the route's description.
        /// </summary>
        private void StartDescriptionPlayback ()
        {
            Navigation.DisplayAlert (
                "Audio Playback",
                "Audio playback is currently not supported!",
                "Ok"
            );
        }


        public GeoLocation GpsLocation {
            get { return gpsLocation; }
            set { SetProperty (ref gpsLocation, value); }
        }

        public Route DetailsRoute {
            get { return detailsRoute; }
            set { SetProperty (ref detailsRoute, value); }
        }

        public bool ShowDetailsRoute {
            get { return showDetailsRoute; }
            set { SetProperty (ref showDetailsRoute, value); }
        }


        /// <summary>
        /// Starts navigation for the route.
        /// </summary>
        private async void StartRoute ()
        {
            if (DetailsRoute.IsRouteStarted ())
            {
                string result =
                    await
                        Navigation.DisplayActionSheet (
                            Strings.RouteDetailspageViewModel_RouteStarted,
                            Strings.RouteDetailspageViewModel_Back, null, Strings.RouteDetailspageViewModel_ContinueRoute, Strings.RouteDetailspageViewModel_RestartRoute);
                if (result.Equals (Strings.RouteDetailspageViewModel_RestartRoute))
                {
                    DetailsRoute.ResetRoute ();
                }
                else if (result.Equals (Strings.RouteDetailspageViewModel_Back))
                {
                    return;
                }
            }
            await Navigation.PushAsync (new NavigationPageViewModel (DetailsRoute));
        }

        #region Properties

        private string description;

        public string Description {
            get { return description; }
            set { SetProperty (ref description, value); }
        }

        private string distance;

        public string Distance {
            get { return distance; }
            set { SetProperty (ref distance, value); }
        }

        private string duration;

        public string Duration {
            get { return duration; }
            set { SetProperty (ref duration, value); }
        }

        private ImageSource image;

        public ImageSource Image {
            get { return image; }
            set { SetProperty (ref image, value); }
        }

        private ObservableCollection<RouteTag> tags;

        public ObservableCollection<RouteTag> Tags {
            get { return tags; }
            set { SetProperty (ref tags, value); }
        }

        private ObservableCollection<string> tabs;

        public ObservableCollection<string> Tabs {
            get { return tabs; }
            set { SetProperty (ref tabs, value); }
        }

        public ICommand StartRouteCommand { get; }
        public ICommand StartDescriptionPlaybackCommand { get; }

        #endregion Properties
    }
}