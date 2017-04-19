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
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.UI.AudioPlayer;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Location;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages {
    /// <summary>
    /// ViewModel for the RouteDetailsPage.
    /// </summary>
    public class RouteDetailsPageViewModel : NavigationViewModel {

        private GeoLocation gpsLocation;
        private Route detailsRoute;
        private bool showDetailsRoute;
        private IAudioPlayer audioPlayer;

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
        public RouteDetailsPageViewModel (Route route)
        {
            Title = route.Title;
            Description = route.Audio?.Caption;
            Distance = string.Format (Strings.RouteDetailsPageViewModel_Distance, route.Distance);
            Duration = string.Format (Strings.RouteDetailsPageViewModel_Duration, route.Duration/60);
            ReadOutCaption = Strings.RouteDetailsPage_PlayAudio;
            Tags = new ObservableCollection<RouteTag> (route.RouteTags);
            var data = route.Image.Data;
            Image = ImageSource.FromStream (() => new MemoryStream (data));
            StartRouteCommand = new Command (StartRoute);
            StartDescriptionPlaybackCommand = new Command (StartDescriptionPlayback);

            Tabs = new ObservableCollection<string> {Strings.RouteDetailsPageViewModel_Description, Strings.RouteDetailsPageViewModel_Map};
            GpsLocation = IoCManager.Resolve<ILocationManager> ().LastKnownLocation.ToGeoLocation ();
            DetailsRoute = route;
            ShowDetailsRoute = true;

            // init the audio button
            audioPlayer = IoCManager.Resolve<IAudioPlayer>();
            audioPlayer.CurrentAudio = route.Audio;
            audioPlayer.AudioTitle = route.Title;
            audioPlayer.IsPlayingChanged += AudioPlayerOnIsPlayingChanged;
        }

        private void AudioPlayerOnIsPlayingChanged (bool newvalue)
        {
            ReadOutCaption = audioPlayer.IsPlaying ? Strings.RouteDetailsPage_PauseAudio : Strings.RouteDetailsPage_PlayAudio;
        }

        /// <summary>
        /// Starts audio playback for the route's description.
        /// </summary>
        private void StartDescriptionPlayback ()
        {
            if (!audioPlayer.IsPlaying)
            {
                audioPlayer.Play ();
            }
            else
            {
                audioPlayer.Pause ();
            }
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

        public override void OnDisappearing ()
        {
            base.OnDisappearing ();

            audioPlayer.Stop ();
        }

        public override void OnRevealed ()
        {
            base.OnRevealed ();

            if (audioPlayer.CurrentAudio==null || !audioPlayer.CurrentAudio.Id.Equals (DetailsRoute.Audio.Id))
            {
                // audio has been changed by exhibit details, reset it
                audioPlayer.CurrentAudio = DetailsRoute.Audio;
            }
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

        private string readOutCaption;

        public string ReadOutCaption
        {
            get { return readOutCaption; }
            set { SetProperty(ref readOutCaption, value); }
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