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

using Android.Content;
using Android.Widget;
using PaderbornUniversity.SILab.Hip.Mobile.Droid.CustomRenderers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(TouchEventSlider), typeof(DroidTouchEventSlider))]

namespace PaderbornUniversity.SILab.Hip.Mobile.Droid.CustomRenderers
{
    class DroidTouchEventSlider : SliderRenderer
    {
        private TouchEventSlider formsSlider;
        private bool areListenersAdded;

        public DroidTouchEventSlider(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Slider> e)
        {
            base.OnElementChanged(e);

            formsSlider = (TouchEventSlider) e.NewElement;

            if (Control != null && !areListenersAdded)
            {
                Control.StartTrackingTouch += SeekbarOnStartTrackingTouch;
                Control.StopTrackingTouch += SeekbarOnStopTrackingTouch;
                areListenersAdded = true;
            }
        }

        /// <summary>
        /// User started doing a drag of the slider.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="stopTrackingTouchEventArgs">The event parameters.</param>
        private void SeekbarOnStopTrackingTouch(object sender, SeekBar.StopTrackingTouchEventArgs stopTrackingTouchEventArgs)
        {
            double newProgress = stopTrackingTouchEventArgs.SeekBar.Progress * formsSlider.Maximum / 1000;
            formsSlider?.TouchUpEvent?.Invoke(sender, new ValueEventArgs(newProgress));
        }

        /// <summary>
        /// User finished doing a drag of the slider.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="startTrackingTouchEventArgs">The event parameters</param>
        private void SeekbarOnStartTrackingTouch(object sender, SeekBar.StartTrackingTouchEventArgs startTrackingTouchEventArgs)
        {
            formsSlider?.TouchDownEvent?.Invoke(sender, null);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Control.StartTrackingTouch -= SeekbarOnStartTrackingTouch;
                Control.StopTrackingTouch -= SeekbarOnStopTrackingTouch;
            }

            base.Dispose(disposing);
        }
    }
}