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

using System.ComponentModel;
using System.Runtime.CompilerServices;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using HipMobileUI.Annotations;
using HipMobileUI.Helpers;
using Xamarin.Forms;

namespace HiPMobileUI.Viewmodels {
    public class AppetizerViewmodel : INotifyPropertyChanged{

        public void Init (AppetizerPage page)
        {
            BottomText = page.Text;
            Image = page.Image.GetImageSource ();
        }

        private ImageSource image;
        private string bottomText;

        public ImageSource Image {
            get { return image; }
            set {
                image = value;
                OnPropertyChanged ();
            }
        }

        public string BottomText {
            get { return bottomText; }
            set {
                bottomText = value;
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