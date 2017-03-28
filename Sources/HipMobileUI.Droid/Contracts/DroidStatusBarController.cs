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

using Android.Views;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Contracts;
using Plugin.CurrentActivity;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace PaderbornUniversity.SILab.Hip.Mobile.Droid.Contracts {
    public class DroidStatusBarController : IStatusBarController{

        public void HideStatusBar ()
        {
            MainActivity main = ((MainActivity) CrossCurrentActivity.Current.Activity);
            main.Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
            main.SetStatusBarColor (Color.Black.ToAndroid());
        }

        public void ShowStatusBar ()
        {
            Color color = (Color) Application.Current.Resources ["PrimaryDarkColor"];
            MainActivity main = ((MainActivity)CrossCurrentActivity.Current.Activity);
            main.Window.ClearFlags(WindowManagerFlags.Fullscreen);
            main.SetStatusBarColor(color.ToAndroid());
        }

    }
}