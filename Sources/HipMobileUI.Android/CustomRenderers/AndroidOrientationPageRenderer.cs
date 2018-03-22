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

using System.ComponentModel;
using Android.App;
using Android.Content;
using Android.Content.PM;
using PaderbornUniversity.SILab.Hip.Mobile.Droid.CustomRenderers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(OrientationContentPage), typeof(AndroidOrientationPageRenderer))]

namespace PaderbornUniversity.SILab.Hip.Mobile.Droid.CustomRenderers
{
    class AndroidOrientationPageRenderer : PageRenderer
    {
        private OrientationContentPage formsOrientationPage;

        public AndroidOrientationPageRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                formsOrientationPage.PropertyChanged -= PagePropertyChanged;
            }

            if (e.NewElement != null)
            {
                formsOrientationPage = (OrientationContentPage)e.NewElement;
                formsOrientationPage.PropertyChanged += PagePropertyChanged;

                SetOrientationController(formsOrientationPage.OrientationController);
            }
        }

        private void PagePropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName.Equals("OrientationController"))
            {
                SetOrientationController(formsOrientationPage.OrientationController);
            }
        }

        private void SetOrientationController(OrientationController controller)
        {
            if (controller == OrientationController.LandscapeConstant)
            {
                ((Activity)Context).RequestedOrientation = ScreenOrientation.Landscape;
            }
            else if (controller == OrientationController.PortraitConstant)
            {
                ((Activity)Context).RequestedOrientation = ScreenOrientation.Portrait;
            }
            else
            {
                ((Activity)Context).RequestedOrientation = ScreenOrientation.Sensor;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                formsOrientationPage.PropertyChanged -= PagePropertyChanged;
            }

            base.Dispose(disposing);
        }
    }
}