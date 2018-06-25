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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MvvmHelpers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Appearance;
using PaderbornUniversity.SILab.Hip.Mobile.UI.AudioPlayer;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views.ExhibitDetails;
using Xamarin.Forms;
using Page = PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.Page;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages
{
    public class ExhibitDetailsPageViewModel : NavigationViewModel, ExhibitDetailsViewModel.IContainer
    {
        private readonly Exhibit exhibit;

        private int position;

        public int Position
        {
            get => position;
            set
            {
                var oldVisiblePage = position < pages.Count ? pages[position] : null;
                var newVisiblePage = value < pages.Count ? pages[value] : null;

                UpdatePageVisibilityStatus(oldVisiblePage, newVisiblePage);
                SetProperty(ref position, value);
            }
        }

        private List<ExhibitDetailsViewModel> pages = new List<ExhibitDetailsViewModel>();

        public List<ExhibitDetailsViewModel> Pages
        {
            get => pages;
            set
            {
                var oldVisiblePage = position < pages.Count ? pages[position] : null;
                var newVisiblePage = position < value.Count ? value[position] : null;

                UpdatePageVisibilityStatus(oldVisiblePage, newVisiblePage);
                SetProperty(ref pages, value);
            }
        }

        private void UpdatePageVisibilityStatus([CanBeNull] ExhibitDetailsViewModel oldVisiblePage, [CanBeNull] ExhibitDetailsViewModel newVisiblePage)
        {
            if (oldVisiblePage != null)
            {
                oldVisiblePage.Visible = false;
            }

            if (newVisiblePage != null)
            {
                newVisiblePage.Visible = true;
            }

            HasAdditionalInformation = newVisiblePage?.HasAdditionalInformationPages == true;
        }

        public Command ShowAdditionalInformationCommand { get; }

        private bool hasAdditionalInformation;

        public bool HasAdditionalInformation
        {
            get => hasAdditionalInformation;
            set => SetProperty(ref hasAdditionalInformation, value);
        }

        public ExhibitDetailsPageViewModel(Exhibit exhibit) : this(exhibit, exhibit.Pages, exhibit.Name)
        {
        }

        public ExhibitDetailsPageViewModel(Exhibit exhibit, IEnumerable<Page> pages, string title, bool additionalInformation = false)
        {
            this.exhibit = exhibit;
            Position = 0;
            Pages = pages.Select((page, i) => new ExhibitDetailsViewModel(exhibit, title, page, Navigation, this, i + 1)).ToList();

            if (additionalInformation)
            {
                IoCManager.Resolve<IBarsColorsChanger>().ChangeToolbarColor(Color.FromRgb(128, 128, 128), Color.FromRgb(169, 169, 169));
            }
            else
            {
                IoCManager.Resolve<IThemeManager>().AdjustTheme();
            }

            ShowAdditionalInformationCommand = new Command(ShowAdditionalInformation);
        }

        private async void ShowAdditionalInformation()
        {
            await Pages[Position].ShowAdditionalInformation();
        }

        public async Task TryGoToNextViewAsync()
        {
            if (position + 1 < Pages.Count)
            {
                Position++;
            }
            else
            {
                var quiz = DbManager.DataAccess.Quizzes().QuizzesForExhibit(exhibit.Id);

                if (quiz.Any())
                {
                    Navigation.InsertPageBefore(new QuizStartingPageViewModel(exhibit), this);
                }
                else
                {
                    Navigation.InsertPageBefore(new UserRatingPageViewModel(exhibit), this);
                }

                await Navigation.PopAsync(false);
            }
        }
    }

    public class ExhibitDetailsViewModel : BaseViewModel
    {
        public interface IContainer
        {
            Task TryGoToNextViewAsync();
        }

        private readonly Exhibit exhibit;
        private readonly Page page;
        private readonly INavigationService navigation;
        private readonly IContainer container;
        public AudioToolbarViewModel AudioToolbar { get; }
        public int PageNumber { get; }

        private bool firstVisible = true;

        private bool visible;

        public bool Visible
        {
            get => visible;
            set
            {
                if (value)
                {
                    OnFirstVisible();
                }

                visible = value;
            }
        }

        /// <summary>
        /// The currently displayed subview.
        /// </summary>
        public ExhibitSubviewViewModel SelectedView { get; }

        public bool HasAdditionalInformationPages => page.AdditionalInformationPages?.Any() == true;

        public ExhibitDetailsViewModel(Exhibit exhibit,
                                       string title,
                                       Page page,
                                       INavigationService navigation,
                                       IContainer container,
                                       int pageNumber)
        {
            this.exhibit = exhibit;
            this.page = page;
            this.navigation = navigation;
            this.container = container;
            PageNumber = pageNumber;
            // stop audio if necessary
            var player = IoCManager.Resolve<IAudioPlayer>();
            if (player.IsPlaying)
            {
                player.Stop();
            }

            // init the audio toolbar
            AudioToolbar = new AudioToolbarViewModel(title, page.Audio != null);
            AudioToolbar.AudioPlayer.AudioCompleted += AudioPlayerOnAudioCompleted;

            PageManager.LoadPageDetails(page);
            AudioToolbar.SetNewAudioFile(page.Audio);

            switch (page)
            {
                case ImagePage imagePage:
                    SelectedView = new ImageViewModel(imagePage);
                    break;
                case TextPage textPage:
                    SelectedView = new TextViewModel(textPage);
                    break;
                case TimeSliderPage timeSliderPage:
                    SelectedView = new TimeSliderViewModel(timeSliderPage);
                    break;
            }
        }

        private async void OnFirstVisible()
        {
            if (!firstVisible) return;
            firstVisible = true;

            // TODO Heavy lifting should be done here

            await StartAutoPlay();
        }

        private async Task StartAutoPlay()
        {
            if (page.Audio == null)
                return;

            // ask if user wants automatic audio playback
            if (Settings.RepeatHintAudio)
            {
                var result = await navigation.DisplayAlert(Strings.ExhibitDetailsPage_Hinweis, Strings.ExhibitDetailsPage_AudioPlay,
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

        public async Task ShowAdditionalInformation()
        {
            if (!HasAdditionalInformationPages) throw new InvalidOperationException("Cannot show additional information as none are available!");

            await navigation.PushAsync(new ExhibitDetailsPageViewModel(exhibit, page.AdditionalInformationPages, Strings.ExhibitDetailsPage_AdditionalInformation, true));
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
                var result = await navigation.DisplayAlert(Strings.ExhibitDetailsPage_Hinweis,
                                                           Strings.ExhibitDetailsPage_PageSwitch,
                                                           Strings.ExhibitDetailsPage_AgreeFeature, Strings.ExhibitDetailsPage_DisagreeFeature).ConfigureAwait(true);
                Settings.AutoSwitchPage = result;
            }

            // aply automatic page switch if wanted and if the next page isn't the rating page
            if (Settings.AutoSwitchPage)
            {
                if (AudioToolbar.AudioPlayer.IsPlaying)
                {
                    AudioToolbar.AudioPlayer.Stop();
                }

                await container.TryGoToNextViewAsync();
            }
        }
    }
}