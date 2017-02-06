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
using HipMobileUI.Controls;
using HipMobileUI.Helpers;
using HipMobileUI.Navigation;
using HipMobileUI.Resources;
using HipMobileUI.ViewModels.Pages;
using Xamarin.Forms;

namespace HipMobileUI.Pages {
    public partial class ExhibitDetailsPage : OrientationContentPage, IViewFor<ExhibitDetailsViewModel> {

        private ExhibitDetailsViewModel ViewModel => (ExhibitDetailsViewModel) BindingContext;
        private HideableToolbarItem audioToolbarButton;

        public ExhibitDetailsPage ()
        {
            InitializeComponent ();
        }

        /// <summary>
        /// Registers the listening method on the ViewModel property changed event
        /// </summary>
        protected override void OnBindingContextChanged ()
        {
            base.OnBindingContextChanged ();

            audioToolbarButton = new HideableToolbarItem
            {
                Icon = "ic_headset_white",
                Text = Strings.ExhibitDetailsPage_AudioToolbar,
                Parent = this
            };
            audioToolbarButton.SetBinding (MenuItem.CommandProperty, "ShowAudioToolbarCommand");
            audioToolbarButton.SetBinding (HideableToolbarItem.IsVisibleProperty, "AudioAvailable");

            ViewModel.PropertyChanged += ViewModelOnPropertyChanged;
        }

        /// <summary>
        /// Listens on changes in the viewmodel to the propery <see cref="ExhibitDetailsViewModel.AudioToolbarVisible"/>
        /// Toggles the visibility of the audio bar accordingly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="propertyChangedEventArgs"></param>
        private void ViewModelOnPropertyChanged (object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == nameof (ViewModel.AudioToolbarVisible))
            {
                ToggleAudioBarVisibility ();
            }
        }

        protected override void OnDisappearing ()
        {
            base.OnDisappearing ();

            OrientationController = OrientationController.Sensor;
        }

        /// <summary>
        /// Translates the audio toolbar on top of the screen or outside of it dependent on whether the toolbar
        /// should be visible or not
        /// </summary>
        private void ToggleAudioBarVisibility ()
        {
            if (ViewModel.AudioToolbarVisible)
            {
                AudioToolbar.TranslateTo (0, 0);
            }
            else
            {
                AudioToolbar.TranslateTo (0, -100);
            }
        }

    }
}