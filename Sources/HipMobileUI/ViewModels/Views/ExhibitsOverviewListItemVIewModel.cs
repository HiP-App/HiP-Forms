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
    class ExhibitsOverviewListItemVIewModel : BaseViewModel
    {

        public ExhibitsOverviewListItemVIewModel (Exhibit exhibit, string distance = "0")
        {
            ExhibitName = exhibit.Name;
            Distance = distance;
            var data = exhibit.Image.Data;
            Image = ImageSource.FromStream (() => new MemoryStream (data));
            ExhibitId = exhibit.Id;
            this.exhibit = exhibit;
        }

        private Exhibit exhibit;
        private string exhibitName;
        private string distance;
        private ImageSource image;

        public string ExhibitName {
            get { return exhibitName; }
            set { SetProperty (ref exhibitName, value); }
        }

        public string Distance {
            get { return distance; }
            set { SetProperty (ref distance, value); }
        }

        public ImageSource Image {
            get { return image; }
            set { SetProperty (ref image, value); }
        }

        public string ExhibitId { get; private set; }

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
