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

using Acr.UserDialogs;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Appearance;
using PaderbornUniversity.SILab.Hip.Mobile.UI.AudioPlayer;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views.ExhibitDetails;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Page = PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.Page;
using Settings = PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers.Settings;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages
{
    public class ExhibitDetailsPageViewModel : NavigationViewModel, IDbChangedObserver
    {
        private ExhibitSubviewViewModel selectedView;
        private Exhibit exhibit;
        private readonly IList<Page> pages;
        private int currentViewIndex;
        private bool audioToolbarVisible;
        private bool hasAdditionalInformation;
        private bool buttonsVisible = true;
        private readonly bool additionalInformation;

        public ExhibitDetailsPageViewModel(string exhibitId) : this(DbManager.DataAccess.Exhibits().GetExhibit(exhibitId)) { }

        public ExhibitDetailsPageViewModel(Exhibit exhibit) : this(exhibit, exhibit.Pages, exhibit.Name) { }

        public ExhibitDetailsPageViewModel(Exhibit exhibit, ICollection<Page> pages, string title, bool additionalInformation = false)
        {
            Exhibit = exhibit;
            this.additionalInformation = additionalInformation;
            AdjustToolbarColor();
            AudioPlayerOnAudioCompleted();

            // stop audio if necessary
            var player = IoCManager.Resolve<IAudioPlayer>();
            if (player.IsPlaying)
            {
                player.Stop();
            }

            // init the audio toolbar
            AudioToolbar = new AudioToolbarViewModel(title);            
            AudioToolbar.AudioPlayer.AudioCompleted += AudioPlayerOnAudioCompleted;
            //AudioPlayerOnAudioCompleted();
            
            // init the current view
            currentViewIndex = 0;
            this.pages = pages.ToList();
            SetCurrentView().ConfigureAwait(true);
            Title = title;

            // init commands     
            NextViewCommand = new Command(async () => await GotoNextView());
            PreviousViewCommand = new Command(GotoPreviousView);
            ShowAudioToolbarCommand = new Command(SwitchAudioToolbarVisibleState);
            ShowAdditionalInformationCommand = new Command(ShowAdditionalInformation);
            var dbChangedHandler = IoCManager.Resolve<IDbChangedHandler>();
            dbChangedHandler.AddObserver(this);
        }

        private void AdjustToolbarColor()
        {
            if (additionalInformation)
            {
                IoCManager.Resolve<IBarsColorsChanger>().ChangeToolbarColor(Color.FromRgb(128, 128, 128), Color.FromRgb(169, 169, 169));
            }
            else
            {
                IoCManager.Resolve<IThemeManager>().AdjustTopBarTheme();
            }
        }

        private void ShowAdditionalInformation()
        {
            var currentPage = pages[currentViewIndex];
            if (currentPage.AdditionalInformationPages == null || !currentPage.AdditionalInformationPages.Any())
                return;

            Navigation.PushAsync(new ExhibitDetailsPageViewModel(Exhibit, currentPage.AdditionalInformationPages, Strings.ExhibitDetailsPage_AdditionalInformation, true));
        }

        private CancellationTokenSource tokenSource;
        private bool willDisappear;

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
        private void ToggleVisibilityOfNavigationButtons()
        {
            buttonsVisible = !buttonsVisible;
            OnPropertyChanged(nameof(NextVisible));
            OnPropertyChanged(nameof(PreviousVisible));
            tokenSource?.Cancel();
        }

        /// <summary>
        /// Audio finished playing.
        /// </summary>
        private async void AudioPlayerOnAudioCompleted()
        {
            if (Settings.RepeatHintAutoPageSwitch)
            {
                // ask for preferred setting regarind automatic page switch
                Settings.RepeatHintAutoPageSwitch = false;
                var result = await Navigation.DisplayAlert(Strings.ExhibitDetailsPage_Hinweis,
                                                           Strings.ExhibitDetailsPage_PageSwitch,
                                                           Strings.ExhibitDetailsPage_AgreeFeature, Strings.ExhibitDetailsPage_DisagreeFeature).ConfigureAwait(true);
                Settings.AutoSwitchPage = result;
 
            }

            // aply automatic page switch if wanted and if the next page isn't the rating page
            if (Settings.AutoSwitchPage && NextViewAvailable && currentViewIndex < pages.Count - 1)
            {
                await GotoNextView();
            }
        }

        /// <summary>
        /// Go to the next available view.
        /// </summary>
        /// <returns></returns>
        private async Task GotoNextView()
        {
            if (currentViewIndex < pages.Count - 1)
            {
                // stop audio
                if (AudioToolbar.AudioPlayer.IsPlaying)
                {
                    AudioToolbar.AudioPlayer.Stop();
                }
                // update the UI
                currentViewIndex++;
                await SetCurrentView();
            }
            else if (currentViewIndex == pages.Count - 1)
            {
                // stop audio
                if (IoCManager.Resolve<INetworkAccessChecker>().GetNetworkAccessStatus() != NetworkAccessStatus.NoAccess)
                {
                    if (AudioToolbar.AudioPlayer.IsPlaying)
                    {
                        AudioToolbar.AudioPlayer.Stop();
                    }
                    Navigation.InsertPageBefore(new UserRatingPageViewModel(Exhibit), this);
                    await Navigation.PopAsync(false);
                }
                else
                {
                    UserDialogs.Instance.Alert(new AlertConfig()
                    {
                        Title = Strings.UserRating_Dialog_Title_No_Internet,
                        Message = Strings.UserRating_Dialog_Message_No_Internet,
                        OkText = Strings.Ok
                    });
                }
            }
        }

        private void SwitchAudioToolbarVisibleState()
        {
            AudioToolbarVisible = !AudioToolbarVisible;
        }

        /// <summary>
        /// Switch to the previous view.
        /// </summary>
        private async void GotoPreviousView()
        {
            if (currentViewIndex > 0)
            {
                // stop audio
                if (AudioToolbar.AudioPlayer.IsPlaying)
                {
                    AudioToolbar.AudioPlayer.Stop();
                }
                // update UI
                currentViewIndex--;
                await SetCurrentView();
            }
            // Go back to appetizer page
            else
            {
                await Navigation.PopAsync();
            }
        }

        /// <summary>
        /// Set the current view.
        /// </summary>
        /// <returns></returns>
        private async Task SetCurrentView()
        {
            var currentPage = pages[currentViewIndex];
            PageManager.LoadPageDetails(currentPage);
            OnPropertyChanged(nameof(AudioAvailable));
            AudioToolbarVisible = AudioAvailable;
            OnPropertyChanged(nameof(Pagenumber));
            OnPropertyChanged(nameof(NextViewAvailable));
            OnPropertyChanged(nameof(PreviousViewAvailable));
            OnPropertyChanged(nameof(NextVisible));
            OnPropertyChanged(nameof(PreviousVisible));
            
            // It's possible to get no audio data even if it should exist
            try
            {
                AudioToolbar.SetNewAudioFile(currentPage.Audio);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.StackTrace);
                AudioToolbar.SetNewAudioFile(null);
                AudioToolbarVisible = false;
            }

            switch (currentPage)
            {
                case ImagePage imagePage:
                    SelectedView = new ImageViewModel(imagePage, ToggleVisibilityOfNavigationButtons);
                    break;
                case TextPage textPage:
                    SelectedView = new TextViewModel(textPage, ToggleVisibilityOfNavigationButtons);
                    break;
                case TimeSliderPage timeSliderPage:
                    SelectedView = new TimeSliderViewModel(timeSliderPage, ToggleVisibilityOfNavigationButtons);
                    break;
            }

            HasAdditionalInformation = currentPage.AdditionalInformationPages?.Any() == true;

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
                // ask if user wants automatic audio playback
                if (Settings.RepeatHintAudio)
                {
                    var result = await Navigation.DisplayAlert(Strings.ExhibitDetailsPage_Hinweis, Strings.ExhibitDetailsPage_AudioPlay,
                                                               Strings.ExhibitDetailsPage_AgreeFeature, Strings.ExhibitDetailsPage_DisagreeFeature);
                    Settings.AutoStartAudio = result;
                    Settings.RepeatHintAudio = false;
                }

                //play automatic audio, if wanted
                if (Settings.AutoStartAudio)
                {
                    AudioToolbar.AudioPlayer.Play();
                }
            }
        }

        /// <summary>
        /// Refreshs the availability of the pages depending on the changed database
        /// </summary>
        public async void DbChanged()
        {
            var exhibitId = Exhibit.Id;
            Exhibit = DbManager.DataAccess.Exhibits().GetExhibit(exhibitId);
            if (!Exhibit.DetailsDataLoaded)
                return;
            await SetCurrentView();
        }

        /// <summary>
        /// Called when the page disappears.
        /// </summary>
        public override void OnDisappearing()
        {
            WillDisappear = true;

            base.OnDisappearing();

            AudioToolbar.AudioPlayer.AudioCompleted -= AudioPlayerOnAudioCompleted;

            //inform the audio toolbar to clean up
            AudioToolbar.OnDisappearing();
        }

        public override void OnHidden()
        {
            base.OnHidden();

            AudioToolbar.AudioPlayer.AudioCompleted -= AudioPlayerOnAudioCompleted;

            //inform the audio toolbar to clean up
            AudioToolbar.OnHidden();
        }

        public override void OnRevealed()
        {
            base.OnRevealed();

            AdjustToolbarColor();
            AudioToolbar.AudioPlayer.AudioCompleted += AudioPlayerOnAudioCompleted;

            //Register audio again
            AudioToolbar.OnRevealed();
        }

        #region properties

        /// <summary>
        /// The exhibit for the details page.
        /// </summary>
        public Exhibit Exhibit
        {
            get => exhibit;
            set => SetProperty(ref exhibit, value);
        }

        /// <summary>
        /// The currently displayed subview.
        /// </summary>
        public ExhibitSubviewViewModel SelectedView
        {
            get => selectedView;
            set => SetProperty(ref selectedView, value);
        }

        /// <summary>
        /// The command for switching to the next view, if available.
        /// </summary>
        public ICommand NextViewCommand { get; }

        /// <summary>
        /// The command for switching to the previous view, if available.
        /// </summary>
        public ICommand PreviousViewCommand { get; }

        /// <summary>
        /// Indicator if a previous view is available.
        /// </summary>
        public bool PreviousViewAvailable => currentViewIndex > 0;

        /// <summary>
        /// Indicator if a next view is available.
        /// </summary>
        public bool NextViewAvailable => currentViewIndex < pages.Count;

        /// <summary>
        /// Indicator if navigation to previous is visible
        /// </summary>
        public bool PreviousVisible => buttonsVisible && PreviousViewAvailable;

        /// <summary>
        /// Indicator if navigation to next is visible
        /// </summary>
        public bool NextVisible => buttonsVisible && NextViewAvailable;

        /// <summary>
        /// Shows the audio toolbar
        /// </summary>
        public ICommand ShowAudioToolbarCommand { get; }

        /// <summary>
        /// Indicates whether the current page has audio available
        /// </summary>
        public bool AudioAvailable => pages[currentViewIndex].Audio != null;

        /// <summary>
        /// Indicates whether the audio toolbar is visible
        /// </summary>
        public bool AudioToolbarVisible
        {
            get => audioToolbarVisible;
            set => SetProperty(ref audioToolbarVisible, value);
        }

        /// <summary>
        /// Viewmodel of audio toolbar which can be shown on the details page
        /// </summary>
        public AudioToolbarViewModel AudioToolbar { get; }

        /// <summary>
        /// The page number
        /// </summary>
        public string Pagenumber => (currentViewIndex + 1) + " / " + pages.Count;

        /// <summary>
        /// Indicates whether there are additional informations for this page
        /// </summary>
        public bool HasAdditionalInformation
        {
            get => hasAdditionalInformation;
            set => SetProperty(ref hasAdditionalInformation, value);
        }

        /// <summary>
        /// Navigates to the additional Information
        /// </summary>
        public ICommand ShowAdditionalInformationCommand { get; }

        /// <summary>
        /// Value indicating that the view of this viewmodel will disappear.
        /// </summary>
        public bool WillDisappear
        {
            get => willDisappear;
            set => SetProperty(ref willDisappear, value);
        }

        #endregion
    }
}
