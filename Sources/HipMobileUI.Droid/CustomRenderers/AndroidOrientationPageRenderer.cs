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
using Android.Content.PM;
using de.upb.hip.mobile.droid.CustomRenderers;
using HipMobileUI.Helpers;
using HipMobileUI.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(OrientationContentPage), typeof(AndroidOrientationPageRenderer))]
namespace de.upb.hip.mobile.droid.CustomRenderers
{
    class AndroidOrientationPageRenderer : PageRenderer {

        private OrientationContentPage formsOrientationPage;

        protected override void OnElementChanged (ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged (e);

            if (e.OldElement != null)
            {
                formsOrientationPage.PropertyChanged -= PagePropertyChanged;
            }

            if (e.NewElement != null)
            {
                formsOrientationPage = (OrientationContentPage) e.NewElement;
                formsOrientationPage.PropertyChanged+=PagePropertyChanged;

                SetOrientationController (formsOrientationPage.OrientationController);
            }
        }

        private void PagePropertyChanged (object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName.Equals ("OrientationController"))
            {
                SetOrientationController (formsOrientationPage.OrientationController);
            }
        }

        private void SetOrientationController (OrientationController controller)
        {
            if (controller == OrientationController.LandscapeConstant)
            {
                ((MainActivity) Context).RequestedOrientation = ScreenOrientation.Landscape;
            }
            else if (controller == OrientationController.PortraitConstant)
            {
                ((MainActivity) Context).RequestedOrientation = ScreenOrientation.Portrait;
            }
            else
            {
                ((MainActivity) Context).RequestedOrientation = ScreenOrientation.Sensor;
            }
        }

    }

}
