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
using Foundation;
using HipMobileUI.Helpers;
using HipMobileUI.iOS.CustomRenderers;
using HipMobileUI.iOS.ViewControllers;
using HipMobileUI.Pages;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(OrientationContentPage), typeof(IosOrientationContentPageRenderer))]
namespace HipMobileUI.iOS.CustomRenderers
{
    class IosOrientationContentPageRenderer : PageRenderer
    {

        private OrientationContentPage formsOrientationPage;
        private UIViewController normalController;

        protected override void OnElementChanged (VisualElementChangedEventArgs e)
        {
            base.OnElementChanged (e);

            if (e.OldElement != null)
            {
                formsOrientationPage.PropertyChanged -= PagePropertyChanged;
            }

            if (e.NewElement != null)
            {
                formsOrientationPage = (OrientationContentPage)e.NewElement;   
                formsOrientationPage.PropertyChanged+=PagePropertyChanged;

                normalController = UIApplication.SharedApplication.KeyWindow.RootViewController;
                SetController (formsOrientationPage.OrientationController);
            }
        }

        private void SetController (OrientationController controller)
        {
            if (controller == OrientationController.Sensor)
            {
                // restore the normal controler whioch supports rotation
                foreach (UIViewController viewController in UIApplication.SharedApplication.KeyWindow.RootViewController.ChildViewControllers)
                {
                    normalController.AddChildViewController (viewController);
                    normalController.View.Add (viewController.View);
                }
                UIApplication.SharedApplication.KeyWindow.RootViewController = normalController;
            }
            else
            {
                if (controller == OrientationController.PortraitConstant)
                {
                    UIDevice.CurrentDevice.SetValueForKey(new NSNumber((int)UIInterfaceOrientation.Portrait), new NSString("orientation"));
                }
                // disable rotation
                UIApplication.SharedApplication.KeyWindow.RootViewController = new OrientationViewController (controller,
                                                                                                              UIApplication.SharedApplication.KeyWindow.RootViewController
                                                                                                                           .ChildViewControllers);
            }
        }

        private void PagePropertyChanged (object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName.Equals ("OrientationController"))
            {
                SetController(formsOrientationPage.OrientationController);
            }
        }

        protected override void Dispose (bool disposing)
        {
            if (disposing)
            {
                formsOrientationPage.PropertyChanged -= PagePropertyChanged;
            }

            base.Dispose (disposing);
        }

    }
}
