// Copyright (C) 2017 History in Paderborn App - Universit�t Paderborn
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

using Android.Widget;
using de.upb.hip.mobile.droid.CustomRenderers;
using HipMobileUI.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(TouchEventSlider), typeof(DroidTouchEventSlider))]
namespace de.upb.hip.mobile.droid.CustomRenderers
{
    class DroidTouchEventSlider : SliderRenderer {

        private TouchEventSlider formsSlider;
        private bool areListenersAdded;

        protected override void OnElementChanged (ElementChangedEventArgs<Slider> e)
        {
            base.OnElementChanged (e);

            formsSlider = (TouchEventSlider)e.NewElement;

            if (Control != null && !areListenersAdded)
            {
                Control.StartTrackingTouch += SeekbarOnStartTrackingTouch;
                Control.StopTrackingTouch += SeekbarOnStopTrackingTouch;
                areListenersAdded = true;
            }
        }

        private void SeekbarOnStopTrackingTouch (object sender, SeekBar.StopTrackingTouchEventArgs stopTrackingTouchEventArgs)
        {
            double newProgress= stopTrackingTouchEventArgs.SeekBar.Progress*formsSlider.Maximum/1000;
            formsSlider?.TouchUpEvent?.Invoke(sender, new ValueEventArgs (newProgress));
        }

        private void SeekbarOnStartTrackingTouch (object sender, SeekBar.StartTrackingTouchEventArgs startTrackingTouchEventArgs)
        {
            formsSlider?.TouchDownEvent?.Invoke (sender, null);
        }

    }
}