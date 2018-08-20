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
using PaderbornUniversity.SILab.Hip.Mobile.UI.Controls;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Views
{
    public partial class AudioToolbarView : IViewFor<AudioToolbarViewModel>
    {
        public static readonly BindableProperty ManualSeekCommandProperty = BindableProperty.Create(
            nameof(ManualSeekCommand),
            typeof(Command),
            typeof(AudioToolbarView)
        );

        public static readonly BindableProperty AudioSliderProgressProperty = BindableProperty.Create(
            nameof(AudioSliderProgress),
            typeof(double),
            typeof(AudioToolbarView),
            0.0
        );

        public Command ManualSeekCommand
        {
            get => (Command) GetValue(ManualSeekCommandProperty);
            set => SetValue(ManualSeekCommandProperty, value);
        }

        public double AudioSliderProgress
        {
            get => (double) GetValue(AudioSliderProgressProperty);
            set => SetValue(AudioSliderProgressProperty, value);
        }

        private bool manualInput;

        public AudioToolbarView()
        {
            InitializeComponent();

            AudioSlider.TouchDown += AudioSliderOnTouchDown;
            AudioSlider.TouchUp += AudioSliderOnTouchUp;
        }

        /// <summary>
        /// User started a touch event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="eventArgs">The event arguments.</param>
        private void AudioSliderOnTouchDown(object sender, EventArgs eventArgs)
        {
            manualInput = true;
        }

        /// <summary>
        /// User finished the touch event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="eventArgs">The event args containing the new value of the slider.</param>
        private void AudioSliderOnTouchUp(object sender, EventArgs eventArgs)
        {
            var args = (ValueEventArgs) eventArgs;
            manualInput = false;
            ManualSeekCommand?.Execute(args.Value);
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(AudioSliderProgress):
                    AudioPlayerOnProgressChanged(AudioSliderProgress);
                    break;
            }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            var manualSeekCommandBinding = new Binding(nameof(AudioToolbarViewModel.ManualSeekCommand)) { Source = this };
            SetBinding(ManualSeekCommandProperty, manualSeekCommandBinding);
            
            var audioSliderProgressBinding = new Binding(nameof(AudioToolbarViewModel.AudioSliderProgress)) { Source = this };
            SetBinding(AudioSliderProgressProperty, audioSliderProgressBinding);
        }

        /// <summary>
        /// New progress value of the audio player available. Update the slider if no manual input is currently pending.
        /// </summary>
        /// <param name="newProgress">The new audio progress</param>
        private void AudioPlayerOnProgressChanged(double newProgress)
        {
            if (!manualInput)
            {
                Device.BeginInvokeOnMainThread(() => AudioSlider.Value = newProgress);
            }
        }
    }
}