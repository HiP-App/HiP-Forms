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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using de.upb.hip.mobile.pcl.Helpers;
using HipMobileUI.Resources;
using HipMobileUI.ViewModels.Views;
using HipMobileUI.ViewModels.Views.ExhibitDetails;
using Xamarin.Forms;
using Page = de.upb.hip.mobile.pcl.BusinessLayer.Models.Page;

namespace HipMobileUI.ViewModels.Pages {
    public class ExhibitDetailsViewModel : NavigationViewModel {

        private ExhibitSubviewViewModel selectedView;
        private AudioToolbarViewModel audioToolbar;

        private readonly Exhibit exhibit;
        private ICommand nextViewCommand;
        private ICommand previousViewCommand;
        private ICommand audioToolbarCommand;
        private bool previousViewAvailable;
        private bool nextViewAvailable;
        private bool previousVisible;
        private bool nextVisible;
        private int currentViewIndex;
        private bool audioAvailabe;
        private bool audioToolbarVisible;

        public ExhibitDetailsViewModel (string exhibitId) : this (ExhibitManager.GetExhibit (exhibitId))
        {
        }

        public ExhibitDetailsViewModel (Exhibit exhibit)
        {
            // init the audio toolbar
            AudioToolbar = new AudioToolbarViewModel (exhibit.Name);
            AudioToolbar.AudioPlayer.AudioCompleted += AudioPlayerOnAudioCompleted;

            // init the current view
            currentViewIndex = 0;
            if (exhibit != null)
            {
                this.exhibit = exhibit;
                SetCurrentView ().ConfigureAwait (true);
                Title = exhibit.Name;

                if (exhibit.Pages.Count > 1)
                    NextViewAvailable = true;
            }

            // init commands
            NextViewCommand = new Command (async () => await GotoNextView ());
            PreviousViewCommand = new Command (GotoPreviousView);
            ShowAudioToolbarCommand = new Command (SwitchAudioToolbarVisibleState);
        }

        private CancellationTokenSource tokenSource;

        /// <summary>
        /// Initializes the delayed toggling with possibility to cancel it using a tokenSource
        /// </summary>
        private void StartDelayedToggling()
        {
            tokenSource = new CancellationTokenSource();

            var token = tokenSource.Token;
            Task.Run(() => ToggleVisibilityDelayed(token), token);
        }

        private const int NavigationButtonsToggleDelay = 2000;
        /// <summary>
        /// Toggles the visibility of then navigation buttons after <see cref="NavigationButtonsToggleDelay"/> milliseconds
        /// unless the task has been canceled using the token
        /// </summary>
        /// <param name="token">Token for canceling the task</param>
        private async Task ToggleVisibilityDelayed(CancellationToken token)
        {
            await Task.Delay(NavigationButtonsToggleDelay, token);
            if (!token.IsCancellationRequested)
            {
                ToggleVisibilityOfNavigationButtons();
            }
        }

        /// <summary>
        /// Toggles the visibility of the navigation buttons if the next/previous page is available
        /// Cancels the delayed task for toggling
        /// </summary>
        private void ToggleVisibilityOfNavigationButtons ()
        {
            if (NextViewAvailable)
            {
                NextVisible = !NextVisible;
            }
            if (PreviousViewAvailable)
            {
                PreviousVisible = !PreviousVisible;
            }

            tokenSource?.Cancel();
        }

        /// <summary>
        /// Audio finished playing.
        /// </summary>
        private async void AudioPlayerOnAudioCompleted ()
        {
            if (Settings.RepeatHintAutoPageSwitch)
            {
                // ask for preferred setting regarind automatic page switch
                Settings.RepeatHintAutoPageSwitch = false;
                var result = await Navigation.DisplayAlert (Strings.ExhibitDetailsPage_Hinweis,
                                                            Strings.ExhibitDetailsPage_PageSwitch,
                                                            Strings.ExhibitDetailsPage_AgreeFeature, Strings.ExhibitDetailsPage_DisagreeFeature).ConfigureAwait (true);
                Settings.AutoSwitchPage = result;
            }

            // aply automatic page switch if wanted
            if (Settings.AutoSwitchPage && NextViewAvailable)
            {
                await GotoNextView ();
            }
        }

        /// <summary>
        /// Go to the next available view.
        /// </summary>
        /// <returns></returns>
        private async Task GotoNextView ()
        {
            if (currentViewIndex < exhibit.Pages.Count - 1)
            {
                // stop audio
                if (AudioToolbar.AudioPlayer.IsPlaying)
                {
                    AudioToolbar.AudioPlayer.Stop ();
                }

                // update the UI
                currentViewIndex++;
                NextViewAvailable = currentViewIndex < exhibit.Pages.Count - 1;
                PreviousViewAvailable = true;
                await SetCurrentView ();
            }
        }

        private void SwitchAudioToolbarVisibleState ()
        {
            AudioToolbarVisible = !AudioToolbarVisible;
        }

        /// <summary>
        /// Switch to the previous view.
        /// </summary>
        private async void GotoPreviousView ()
        {
            if (currentViewIndex > 0)
            {
                // stop audio
                if (AudioToolbar.AudioPlayer.IsPlaying)
                {
                    AudioToolbar.AudioPlayer.Stop ();
                }

                // update UI
                currentViewIndex--;
                await SetCurrentView ();
                PreviousViewAvailable = currentViewIndex > 0;
                NextViewAvailable = true;
            }
        }

        /// <summary>
        /// Set the current view.
        /// </summary>
        /// <returns></returns>
        private async Task SetCurrentView ()
        {
            // update UI
            Page currentPage = exhibit.Pages [currentViewIndex];
            AudioAvailable = currentPage.Audio != null;
            if (!AudioAvailable)
            {
                AudioToolbarVisible = false;
            }

            AudioToolbar.SetNewAudioFile (currentPage.Audio);

            switch (currentPage.PageType)
            {
                case PageType.AppetizerPage:
                    SelectedView = new AppetizerViewModel(exhibit.Name, currentPage.AppetizerPage);
                    break;
                case PageType.ImagePage:
                    SelectedView = new ImageViewModel(currentPage.ImagePage, ToggleVisibilityOfNavigationButtons);
                    break;
                case PageType.TextPage:
                    SelectedView = new TextViewModel(currentPage.TextPage, ToggleVisibilityOfNavigationButtons);
                    break;
                case PageType.TimeSliderPage:
                    SelectedView = new TimeSliderViewModel(currentPage.TimeSliderPage, ToggleVisibilityOfNavigationButtons);
                    break;
            }

            //Cancel disabling navigation buttons caused by page selected before
            tokenSource?.Cancel();
            //Toggle navigation buttons visibility for specific pages
            switch (currentPage.PageType)
            {
                case PageType.ImagePage:
                case PageType.TextPage:
                case PageType.TimeSliderPage:
                    StartDelayedToggling();
                    break;
            }

            if (currentPage.Audio != null)
            {
                // ask if user wants autoamtic audio playback
                if (Settings.RepeatHintAudio)
                {
                    var result = await Navigation.DisplayAlert (Strings.ExhibitDetailsPage_Hinweis, Strings.ExhibitDetailsPage_AudioPlay,
                                                                Strings.ExhibitDetailsPage_AgreeFeature, Strings.ExhibitDetailsPage_DisagreeFeature);
                    Settings.AutoStartAudio = result;
                    Settings.RepeatHintAudio = false;
                }

                //play automatic audio, if wanted
                if (Settings.AutoStartAudio)
                {
                    AudioToolbar.AudioPlayer.Play ();
                }
            }
        }

        /// <summary>
        /// Called when the page disappears.
        /// </summary>
        public override void OnDisappearing ()
        {
            base.OnDisappearing ();

            //inform the audio toolbar to clean up
            AudioToolbar.OnDisappearing ();
        }

        #region propeties

        /// <summary>
        /// The currently displayed subview.
        /// </summary>
        public ExhibitSubviewViewModel SelectedView {
            get { return selectedView; }
            set { SetProperty (ref selectedView, value); }
        }

        /// <summary>
        /// The command for switching to the next view, if available.
        /// </summary>
        public ICommand NextViewCommand {
            get { return nextViewCommand; }
            set { SetProperty (ref nextViewCommand, value); }
        }

        /// <summary>
        /// The command for switching to the previous view, if available.
        /// </summary>
        public ICommand PreviousViewCommand {
            get { return previousViewCommand; }
            set { SetProperty (ref previousViewCommand, value); }
        }

        /// <summary>
        /// Indicator if a previous view is available.
        /// </summary>
        public bool PreviousViewAvailable {
            get { return previousViewAvailable; }
            set
            {
                PreviousVisible = value;
                SetProperty (ref previousViewAvailable, value);
            }
        }

        /// <summary>
        /// Indicator if a next view is available.
        /// </summary>
        public bool NextViewAvailable {
            get { return nextViewAvailable; }
            set
            {
                NextVisible = value;
                SetProperty (ref nextViewAvailable, value);
            }
        }

        /// <summary>
        /// Indicator if navigation to previous is visible
        /// </summary>
        public bool PreviousVisible
        {
            get { return previousVisible; }
            set { SetProperty(ref previousVisible, value); }
        }

        /// <summary>
        /// Indicator if navigation to next is visible
        /// </summary>
        public bool NextVisible
        {
            get { return nextVisible; }
            set { SetProperty(ref nextVisible, value); }
        }

        /// <summary>
        /// Shows the audio toolbar
        /// </summary>
        public ICommand ShowAudioToolbarCommand {
            get { return audioToolbarCommand; }
            set { SetProperty (ref audioToolbarCommand, value); }
        }

        /// <summary>
        /// Indicates whether the current page has audio available
        /// </summary>
        public bool AudioAvailable {
            get { return audioAvailabe; }
            set { SetProperty (ref audioAvailabe, value); }
        }

        /// <summary>
        /// Indicates whether the audio toolbar is visible
        /// </summary>
        public bool AudioToolbarVisible {
            get { return audioToolbarVisible; }
            set { SetProperty (ref audioToolbarVisible, value); }
        }

        /// <summary>
        /// Viewmodel of audio toolbar which can be shown on the details page
        /// </summary>
        public AudioToolbarViewModel AudioToolbar {
            get { return audioToolbar; }
            set { SetProperty (ref audioToolbar, value); }
        }

        #endregion
    }
}