﻿// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
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

using PaderbornUniversity.SILab.Hip.Mobile.UI.DesignTime;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Views.ExhibitDetails;
using System.ComponentModel;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Pages
{
    public partial class ExhibitDetailsPage : IViewFor<ExhibitDetailsPageViewModel>
    {
        private ExhibitDetailsPageViewModel ViewModel => (ExhibitDetailsPageViewModel) BindingContext;
        private OrientationController savedControllerState;
        private bool isOnDisappearingContext;
        private DeviceOrientation orientation;

        public ExhibitDetailsPage()
        {
            orientation = DeviceOrientation.Undefined;
            InitializeComponent();
            DesignMode.Initialize(this);

            // Workaround because OnDisappearing is called when the app starts sleeping(on Android) and the OrientationController is reset. Therefore, we need to safe the controller and reapply it after the app wakes up.
            //savedControllerState = OrientationController;
            PropertyChanged += OnPropertyChanged;
            MessagingCenter.Subscribe<App>(this, AppSharedData.WillWakeUpMessage, WillWakeUp);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName.Equals(nameof(OrientationController)) && !isOnDisappearingContext)
            {
                //save the new controller
                //savedControllerState = OrientationController;
            }
        }

        /// <summary>
        /// Called when the app is about to wake up.
        /// </summary>
        /// <param name="app">The instance of the app.</param>
        private void WillWakeUp(App app)
        {
            //restore the old controller as it was changed by OnDisappearing(cannot distinguish between sleep and page popped on Android)
            //OrientationController = savedControllerState;
        }

        /// <summary>
        /// Registers the listening method on the ViewModel property changed event
        /// </summary>
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            UpdateAudioBarVisibility();
            ViewModel.PropertyChanged += ViewModelOnPropertyChanged;
        }

        /// <summary>
        /// Listens on changes in the viewmodel to the propery <see cref="ExhibitDetailsPageViewModel.AudioToolbarVisible"/>
        /// Toggles the visibility of the audio bar accordingly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="propertyChangedEventArgs"></param>
        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName.Equals(nameof(ViewModel.AudioToolbarVisible)))
            {
                UpdateAudioBarVisibility();
            }

            if (propertyChangedEventArgs.PropertyName.Equals(nameof(ViewModel.WillDisappear)) && ViewModel.WillDisappear)
            {
                MessagingCenter.Unsubscribe<App>(this, AppSharedData.WillWakeUpMessage);
            }
        }

        /// <summary>
        /// Check if AudioBar should be visible on the page
        /// 
        /// </summary>
        private void UpdateAudioBarVisibility()
        {
            AudioToolbar.IsVisible = ViewModel.AudioToolbarVisible;
        }

        /// <summary>
        /// Called when the page is popped from the navigation stack(Android& iOS) or the app is about to start sleeping(display turned off, app went to background9(only Android)
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            // reset the controller, cannot be called in OnDisappearing of viewmodel as it is too late
            // in case it was called on app sleep, the state will be restored, when the app wakes up
            isOnDisappearingContext = true;
            //OrientationController = OrientationController.Sensor;
            isOnDisappearingContext = false;
        }

        /// <summary>
        /// Size changed, determine if we need to update the layout.
        /// </summary>
        /// <param name="width">The new width.</param>
        /// <param name="height">The new height.</param>
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (width > height && orientation != DeviceOrientation.Landscape)
            {
                if (ContentView.Content.GetType() == typeof(ImageView) || ContentView.Content.GetType() == typeof(TimeSliderView))
                {
                    AudioContainer.IsVisible = false;
                    ContentView.Margin = 0;
                }
                orientation = DeviceOrientation.Landscape;

            }
            else if (width < height && orientation != DeviceOrientation.Portrait)
            {
                orientation = DeviceOrientation.Portrait;
                AudioContainer.IsVisible = true;
                ContentView.Margin = 5;
            }
        }
    }
}