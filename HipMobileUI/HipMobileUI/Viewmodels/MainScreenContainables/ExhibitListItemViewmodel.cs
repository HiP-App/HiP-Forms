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

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using HipMobileUI.Annotations;
using HipMobileUI.Helpers;
using Xamarin.Forms;

namespace HipMobileUI.Viewmodels.MainScreenContainables {
    public class ExhibitListItemViewmodel : INotifyPropertyChanged{

        public ExhibitListItemViewmodel (string exhibitId)
        {
            this.ExhibitId = exhibitId;

            var exhibit = ExhibitManager.GetExhibit (exhibitId);
            ExhibitName = exhibit.Name;
            Distance = 4.2;
            Image = exhibit.Image.GetImageSource ();
        }

        public string ExhibitId { get; set; }

        private string exhibitName;
        private double distance;
        private ImageSource image;

        public string ExhibitName {
            get { return exhibitName; }
            set {
                exhibitName = value;
                OnPropertyChanged ();
            }
        }

        public double Distance {
            get { return distance; }
            set {
                distance = value;
                OnPropertyChanged ();
            }
        }

        public ImageSource Image {
            get { return image; }
            set {
                image = value;
                OnPropertyChanged ();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged ([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke (this, new PropertyChangedEventArgs (propertyName));
        }

    }
}