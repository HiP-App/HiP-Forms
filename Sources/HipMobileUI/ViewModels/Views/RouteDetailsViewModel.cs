// Copyright (C) 2016 History in Paderborn App - Universität Paderborn
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

using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using HipMobileUI.ViewModels.Pages;
using Xamarin.Forms;

namespace HipMobileUI.ViewModels.Views {
    public class RouteDetailsViewModel : NavigationViewModel {

        public RouteDetailsViewModel (Route route)
        {
            Title = route.Title;
            Description = route.Description;
            Distance = $"{route.Distance} km";
            Duration = $"{route.Duration/60} min";
            var data = route.Image.Data;
            Image = ImageSource.FromStream (() => new MemoryStream(data));  
            
            StartRouteCommand = new Command (StartRoute);          
            StartDescriptionPlaybackCommand = new Command (StartDescriptionPlayback);
        }

        private void StartDescriptionPlayback(object s)
        {
            Navigation.DisplayAlert (
                "Audio Playback", 
                "Audio playback is currently not supported!", 
                "Ok"
            );
        }

        private void StartRoute ()
        {
            Navigation.DisplayAlert (
                "Currently not supported!",
                "Starting a route is currently not supported!",
                "Ok"
            );
        }

        #region Properties

        private string description;
        public string Description
        {
            get { return description; }
            set { SetProperty(ref description, value); }
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

        public ICommand StartRouteCommand { get; }
        public ICommand StartDescriptionPlaybackCommand { get; }

        #endregion Properties

    }
}