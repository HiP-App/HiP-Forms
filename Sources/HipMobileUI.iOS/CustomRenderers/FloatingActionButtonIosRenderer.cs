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
using HipMobileUI.Controls;
using HipMobileUI.iOS.CustomRenderers;
using MaterialControls;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(FloatingActionButton), typeof(FloatingActionButtonIosRenderer))]
namespace HipMobileUI.iOS.CustomRenderers
{
    class FloatingActionButtonIosRenderer : ViewRenderer<FloatingActionButton, UIView> {

        private MDButton fab;
        private FloatingActionButton formsButton;

        protected override void OnElementChanged (ElementChangedEventArgs<FloatingActionButton> e)
        {
            base.OnElementChanged (e);

            if (Control == null)
            {
                fab = new MDButton();
                fab.Layer.CornerRadius = FloatingActionButton.IosSize / 2;
                fab.TouchUpInside += FabPressed;
                SetNativeControl(fab);
            }
            if (e.OldElement != null)
            {
                // Unsubscribe
                e.OldElement.NormalColorChanged -= SetNormalColor;
                e.OldElement.RippleColorChanged -= SetRippleColor;
                e.OldElement.IconChanged -= SetIcon;
            }
            if (e.NewElement != null)
            {
                // set init values
                formsButton = e.NewElement;
                SetNormalColor(formsButton.NormalColor);
                SetRippleColor (formsButton.RippleColor);
                SetIcon (formsButton.Icon);

                // Subscribe
                formsButton.NormalColorChanged+=SetNormalColor;
                formsButton.RippleColorChanged+=SetRippleColor;
                formsButton.IconChanged+=SetIcon;
            }
        }

        private void SetIcon (string newIcon)
        {
            fab.SetImage(UIImage.FromBundle(newIcon), UIControlState.Normal);
        }

        private void SetRippleColor (Color newColor)
        {
            fab.RippleColor = newColor.ToUIColor ();
        }

        private void SetNormalColor (Color newColor)
        {
            fab.BackgroundColor = newColor.ToUIColor ();
        }

        private void FabPressed (object sender, EventArgs eventArgs)
        {
            if (formsButton != null && formsButton.Command.CanExecute (null))
            {
                formsButton.Command.Execute (null);
            }
        }

        protected override void Dispose (bool disposing)
        {
            if (disposing)
            {
                fab.TouchUpInside -= FabPressed;
                formsButton.NormalColorChanged -= SetNormalColor;
                formsButton.RippleColorChanged -= SetRippleColor;
                formsButton.IconChanged -= SetIcon;
            }

            base.Dispose (disposing);
        }

    }
}
