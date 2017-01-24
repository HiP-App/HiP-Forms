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

using System.IO;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using de.upb.hip.mobile.pcl.Helpers;
using MvvmHelpers;
using Plugin.Geolocator.Abstractions;
using Xamarin.Forms;

namespace HipMobileUI.ViewModels.Views
{
    class ExhibitsOverviewListItemViewModel : BaseViewModel
    {

        public ExhibitsOverviewListItemViewModel (Exhibit exhibit, string distance = "- m")
        {
            ExhibitName = exhibit.Name;
            Distance = distance;
            var data = exhibit.Image.Data;
            Image = ImageSource.FromStream (() => new MemoryStream (data));
            Exhibit = exhibit;
        }

        private Exhibit exhibit;
        private string exhibitName;
        private string distance;
        private ImageSource image;

        /// <summary>
        /// The name of the exhibit.
        /// </summary>
        public string ExhibitName {
            get { return exhibitName; }
            set { SetProperty (ref exhibitName, value); }
        }

        /// <summary>
        /// The distance to the exhibit.
        /// </summary>
        public string Distance {
            get { return distance; }
            set { SetProperty (ref distance, value); }
        }

        /// <summary>
        /// The appetizer image for teh exhibit.
        /// </summary>
        public ImageSource Image {
            get { return image; }
            set { SetProperty (ref image, value); }
        }

        /// <summary>
        /// The id of the exhibit.
        /// </summary>
        public Exhibit Exhibit {
            get { return exhibit; }
            set { SetProperty (ref exhibit, value); }
        }

        /// <summary>
        /// Update the displayed distance according to the position.
        /// </summary>
        /// <param name="position">The new position from which the distance is measured.</param>
        public void UpdateDistance (Position position)
        {
            double distance = MathUtil.CalculateDistance (exhibit.Location.Latitude, exhibit.Location.Longitude, position.Latitude, position.Longitude);
            if (distance < 1)
            {
                Distance = $"{distance*1000:F0} m";
            }
            else
            {
                Distance = $"{(distance):0.##} km";
            }
        }

    }
}
