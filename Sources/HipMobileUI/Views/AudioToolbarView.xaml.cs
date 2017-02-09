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
using de.upb.hip.mobile.pcl.Common;
using HipMobileUI.AudioPlayer;
using HipMobileUI.Controls;
using HipMobileUI.Navigation;
using HipMobileUI.ViewModels.Views;
using Xamarin.Forms;

namespace HipMobileUI.Views
{
    public partial class AudioToolbarView : IViewFor<AudioToolbarViewModel> {

        private bool manualInput;
        private readonly IAudioPlayer audioPlayer;
        public AudioToolbarView()
        {
            InitializeComponent();

            AudioSlider.TouchDown+=AudioSliderOnTouchDown;
            AudioSlider.TouchUp+=AudioSliderOnTouchUp;

            audioPlayer = IoCManager.Resolve<IAudioPlayer> ();
            audioPlayer.ProgressChanged += AudioPlayerOnProgressChanged;
        }

        /// <summary>
        /// User started a touch event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="eventArgs">The event arguments.</param>
        private void AudioSliderOnTouchDown (object sender, EventArgs eventArgs)
        {
            manualInput = true;
        }

        /// <summary>
        /// User finished the touch event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="eventArgs">The event args containing the new value of the slider.</param>
        private void AudioSliderOnTouchUp (object sender, EventArgs eventArgs)
        {
            var args = (ValueEventArgs) eventArgs;
            audioPlayer.SeekTo (args.Value);
            manualInput = false;
        }

        private void AudioPlayerOnProgressChanged (double newProgress)
        {
            if (!manualInput)
            {
                Device.BeginInvokeOnMainThread(() =>AudioSlider.Value = newProgress);
            }
        }

    }
}
