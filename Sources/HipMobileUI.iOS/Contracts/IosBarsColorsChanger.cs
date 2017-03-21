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

using HipMobileUI.Contracts;
using HipMobileUI.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using NavigationPage = Xamarin.Forms.NavigationPage;

namespace HipMobileUI.iOS.Contracts {
    public class IosBarsColorsChanger : IBarsColorsChanger {
        private Page FormsPage => Xamarin.Forms.Application.Current.MainPage;

        private NavigationPage NavigationPage
        {
            get
            {
                var mainPage = FormsPage as MainPage;
                return mainPage?.Navigationpage;
            }
        }

        public void ChangeToolbarColor(Color statusBarColor, Color actionBarColor)
        {
            NavigationPage.BarBackgroundColor = actionBarColor;
            //activity.SetStatusBarColor(statusBarColor.ToUIColor());
        }

    }
}