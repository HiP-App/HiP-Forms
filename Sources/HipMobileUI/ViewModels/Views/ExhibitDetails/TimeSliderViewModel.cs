// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
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
// limitations under the License.

using System.Collections.ObjectModel;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using HipMobileUI.Helpers;
using Xamarin.Forms;
using Image = de.upb.hip.mobile.pcl.BusinessLayer.Models.Image;

namespace HipMobileUI.ViewModels.Views.ExhibitDetails
{
    public class TimeSliderViewModel : ExhibitSubviewViewModel
    {

        public TimeSliderViewModel (TimeSliderPage timesliderPage)
        {
            Images = new ObservableCollection<ImageSource> ();
            Years = new ObservableCollection<string> ();
            int i = 1950;
            foreach (Image timesliderPageImage in timesliderPage.Images)
            {
                Images.Add (timesliderPageImage.GetImageSource ());
                Years.Add (i.ToString());
                i += 10;
            }
        }

        private ObservableCollection<ImageSource> images;
        private ObservableCollection<string> years;

        public ObservableCollection<ImageSource> Images {
            get { return images; }
            set { SetProperty (ref images, value); }
        }

        public ObservableCollection<string> Years {
            get { return years; }
            set { SetProperty (ref years, value); }
        }

    }
}
