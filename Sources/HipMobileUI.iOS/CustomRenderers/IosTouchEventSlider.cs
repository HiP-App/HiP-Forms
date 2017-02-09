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

using System;
using HipMobileUI.Controls;
using HipMobileUI.iOS.CustomRenderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(TouchEventSlider), typeof(IosTouchEventSlider))]
namespace HipMobileUI.iOS.CustomRenderers
{
    class IosTouchEventSlider : SliderRenderer
    {
        private TouchEventSlider formsSlider;

        protected override void OnElementChanged(ElementChangedEventArgs<Slider> e)
        {
            base.OnElementChanged(e);

            formsSlider = (TouchEventSlider)e.NewElement;

            if (Control != null)
            {
                Control.TouchDown+=ControlOnTouchDown;
                Control.TouchUpInside+=ControlOnTouchUp;
                Control.TouchUpOutside += ControlOnTouchUp;
            }
        }

        private void ControlOnTouchUp (object sender, EventArgs eventArgs)
        {
            var args = new ValueEventArgs (Control.Value);
            formsSlider?.TouchUpEvent?.Invoke(sender, args);
        }

        private void ControlOnTouchDown (object sender, EventArgs eventArgs)
        {
            formsSlider?.TouchDownEvent?.Invoke (sender, null);
        }

    }
}
