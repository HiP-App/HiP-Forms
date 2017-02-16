﻿// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
            texts = new List<string> ();
            Headline = timesliderPage.Title;
            Description = timesliderPage.Text;
            foreach (Image timesliderPageImage in timesliderPage.Images)
            {
                Images.Add (timesliderPageImage.GetImageSource ());
                texts.Add (timesliderPageImage.Description);
            }
            if(texts.Count>0)DisplayedText = texts [0];
            PropertyChanged+=OnPropertyChanged;

            // TODO add year numbers once timeslider should be used with years
        }

        private void OnPropertyChanged (object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName.Equals (nameof (SelectedValue)))
            {
                DisplayedText = texts [Convert.ToInt32 (Math.Round(SelectedValue))];
            }
        }

        private ObservableCollection<ImageSource> images;
        private ObservableCollection<string> years;
        private double selectedValue;
        private string displayedText;

        private readonly List<string> texts;
        private string headline;
        private string description;

        public ObservableCollection<ImageSource> Images {
            get { return images; }
            set { SetProperty (ref images, value); }
        }

        public ObservableCollection<string> Years {
            get { return years; }
            set { SetProperty (ref years, value); }
        }

        public double SelectedValue {
            get { return selectedValue; }
            set { SetProperty (ref selectedValue, value); }
        }

        public string DisplayedText {
            get { return displayedText; }
            set { SetProperty (ref displayedText, value); }
        }

        public string Headline {
            get { return headline; }
            set { SetProperty (ref headline, value); }
        }

        public string Description {
            get { return description; }
            set { SetProperty (ref description, value); }
        }

    }
}
