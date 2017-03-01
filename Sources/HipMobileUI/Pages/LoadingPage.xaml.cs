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

using HipMobileUI.Helpers;
using HipMobileUI.Navigation;
using HipMobileUI.ViewModels.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HipMobileUI.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadingPage : ContentPage, IViewFor<LoadingPageViewModel> {

        private DeviceOrientation deviceOrientation;

        public LoadingPage()
        {
            InitializeComponent();
            deviceOrientation = DeviceOrientation.Undefined;
        }

        protected override void OnAppearing ()
        {
            base.OnAppearing ();

            ((LoadingPageViewModel)BindingContext).StartLoading.Execute (null);
        }

        protected override void OnSizeAllocated (double width, double height)
        {
            base.OnSizeAllocated (width, height);

            if (width <= height)
            {
                if (deviceOrientation != DeviceOrientation.Portrait)
                {
                    // portrait mode
                    OuterStack.Orientation = StackOrientation.Vertical;
                    deviceOrientation = DeviceOrientation.Portrait;
                }
            }
            else if (width > height)
            {
                if (deviceOrientation != DeviceOrientation.Landscape)
                {
                    // landscape mode
                    OuterStack.Orientation = StackOrientation.Horizontal;
                    deviceOrientation = DeviceOrientation.Landscape;
                }
            }
        }

    }
}
