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

using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace HipMobileUI.Controls
{
    class FadeInImage : Image {

        public FadeInImage ()
        {
            PropertyChanged+=OnPropertyChanged;
        }

        private async void OnPropertyChanged (object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName.Equals ("IsLoading"))
            {
                if (IsLoading)
                {
                    Opacity = 0;
                }
                else
                {
                    await this.FadeTo (1, FadeInTime);
                }
            }
        }

        public static readonly BindableProperty FadeInTimeProperty =
            BindableProperty.Create ("FadeInTime", typeof (uint), typeof (FadeInImage), 200U);

        public uint FadeInTime
        {
            get { return (uint)GetValue(FadeInTimeProperty); }
            set { SetValue(FadeInTimeProperty, value); }
        }
    }
}
