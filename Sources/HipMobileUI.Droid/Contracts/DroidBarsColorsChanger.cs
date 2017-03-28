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

using PaderbornUniversity.SILab.Hip.Mobile.UI.Contracts;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using MainPage = PaderbornUniversity.SILab.Hip.Mobile.UI.Pages.MainPage;
using NavigationPage = Xamarin.Forms.NavigationPage;

namespace PaderbornUniversity.SILab.Hip.Mobile.Droid.Contracts {
    public class DroidBarsColorsChanger : IBarsColorsChanger {

        public DroidBarsColorsChanger (MainActivity activity)
        {
            this.activity = activity;
        }

        private readonly MainActivity activity;

        private Page FormsPage => Application.Current.MainPage;

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
            activity.SetStatusBarColor(statusBarColor.ToAndroid());
        }

    }
}