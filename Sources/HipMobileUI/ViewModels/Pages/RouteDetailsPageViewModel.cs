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

using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.UI.AudioPlayer;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Location;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.DtoToModelConverters;
using Xamarin.Forms;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;
using System.Diagnostics;
using System;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages
{
    /// <summary>
    /// ViewModel for the RouteDetailsPage.
    /// </summary>
    public class RouteDetailsPageViewModel : NavigationViewModel
    {
        private GeoLocation gpsLocation;
        private Route detailsRoute;
        private bool showDetailsRoute;
        private readonly IAudioPlayer audioPlayer;
        private readonly AudioToolbarViewModel audioToolbar;

        /// <summary>
        /// Creates a new ViewModel for the route with the specified ID.
        /// Fetches the corresponding <see cref="Route"/> via the <see cref="RouteManager"/>
        /// and passes it to the alternative constructor.
        /// </summary>
        /// <param name="id">The ID of the route the ViewModel is created for.</param>
        public RouteDetailsPageViewModel(string id) : this(DbManager.DataAccess.Routes().GetRoute(id))
        {
            // Intentionally left blank
        }

        /// <summary>
        /// Creates a new ViewModel for the specified <see cref="Route"/>.
        /// </summary>
        /// <param name="route">The <see cref="Route"/> the ViewModel is created for.</param>
        public RouteDetailsPageViewModel(Route route)
        {
            Title = route.Name;
            Description = route.Audio?.Caption;
            Distance = string.Format(Strings.RouteDetailsPageViewModel_Distance, route.Distance);
            Duration = string.Format(Strings.RouteDetailsPageViewModel_Duration, route.Duration / 60);
            ReadOutCaption = Strings.RouteDetailsPage_PlayAudio;
            Tags = new ObservableCollection<RouteTag>(route.Tags);
            SetRouteImage(route);
            StartRouteCommand = new Command(StartRoute);
            StartDescriptionPlaybackCommand = new Command(StartDescriptionPlayback);

            Tabs = new ObservableCollection<string> { Strings.RouteDetailsPageViewModel_Description, Strings.RouteDetailsPageViewModel_Map };
            GpsLocation = IoCManager.Resolve<ILocationManager>().LastKnownLocation?.ToGeoLocation() ?? new GeoLocation(0, 0);
            DetailsRoute = route;
            ShowDetailsRoute = true;

            // stop audio if necessary
            var player = IoCManager.Resolve<IAudioPlayer>();
            if (player.IsPlaying)
            {
                player.Stop();
            }

            // init the audio toolbar
            audioToolbar = new AudioToolbarViewModel(route.Name, true);

            // It's possible to get no audio data even if it should exist
            try
            {
                audioToolbar.SetNewAudioFile(route.Audio);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.StackTrace);
                audioToolbar.SetNewAudioFile(null);
            }

            //play automatic audio, if wanted
            if (Settings.AutoStartAudio)
            {
                audioToolbar.AudioPlayer.Play();
            }
        }

        private async void SetRouteImage(Route route)
        {
            var imageData = await route.Image.GetDataAsync();
            Image = imageData != null ? ImageSource.FromStream(() => new MemoryStream(imageData)) : ImageSource.FromStream(() => new MemoryStream(BackupData.BackupImageData));
        }

        /// <summary>
        /// Starts audio playback for the route's description.
        /// </summary>
        private void StartDescriptionPlayback()
        {
            if (!audioToolbar.AudioPlayer.IsPlaying)
            {
                audioToolbar.AudioPlayer.Play();
            }
            else
            {
                audioToolbar.AudioPlayer.Pause();
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

        public bool ShowDetailsRoute
        {
            get { return showDetailsRoute; }
            set { SetProperty(ref showDetailsRoute, value); }
        }

        /// <summary>
        /// Starts navigation for the route.
        /// </summary>
        private async void StartRoute()
        {
            if (DetailsRoute.IsRouteStarted())
            {
                var result =
                    await
                        Navigation.DisplayActionSheet(
                            Strings.RouteDetailspageViewModel_RouteStarted,
                            Strings.RouteDetailspageViewModel_Back, null, Strings.RouteDetailspageViewModel_ContinueRoute, Strings.RouteDetailspageViewModel_RestartRoute);
                if (result.Equals(Strings.RouteDetailspageViewModel_RestartRoute))
                {
                    await DetailsRoute.ResetRoute();
                }
                else if (result.Equals(Strings.RouteDetailspageViewModel_Back))
                {
                    return;
                }
            }
            await Navigation.PushAsync(new NavigationPageViewModel(DetailsRoute));
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
            audioToolbar.OnDisappearing();
        }

        public override void OnRevealed()
        {
            base.OnRevealed();

            //Register audio again
            audioToolbar.OnRevealed();
        }

        public override void OnHidden()
        {
            base.OnHidden();

            //inform the audio toolbar to clean up
            audioToolbar.OnHidden();
        }

        #region Properties

        private string description;

        public string Description
        {
            get { return description; }
            set { SetProperty(ref description, value); }
        }

        private string distance;

        public string Distance
        {
            get { return distance; }
            set { SetProperty(ref distance, value); }
        }

        private string readOutCaption;

        public string ReadOutCaption
        {
            get { return readOutCaption; }
            set { SetProperty(ref readOutCaption, value); }
        }

        private string duration;

        public string Duration
        {
            get { return duration; }
            set { SetProperty(ref duration, value); }
        }

        private ImageSource image;

        public ImageSource Image
        {
            get { return image; }
            set { SetProperty(ref image, value); }
        }

        private ObservableCollection<RouteTag> tags;

        public ObservableCollection<RouteTag> Tags
        {
            get { return tags; }
            set { SetProperty(ref tags, value); }
        }

        private ObservableCollection<string> tabs;

        public ObservableCollection<string> Tabs
        {
            get { return tabs; }
            set { SetProperty(ref tabs, value); }
        }

        public ICommand StartRouteCommand { get; }
        public ICommand StartDescriptionPlaybackCommand { get; }

        #endregion Properties
    }
}
