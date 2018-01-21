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

using System.ComponentModel;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Controls;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Pages
{
    public partial class ExhibitDetailsPage : IViewFor<ExhibitDetailsViewModel>
    {
        private ExhibitDetailsViewModel ViewModel => (ExhibitDetailsViewModel)BindingContext;
        private OrientationController savedControllerState;
        private bool isOnDisappearingContext;

        public ExhibitDetailsPage()
        {
            InitializeComponent();

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
        /// Listens on changes in the viewmodel to the propery <see cref="ExhibitDetailsViewModel.AudioToolbarVisible"/>
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

    }
}