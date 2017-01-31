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
using System.Windows.Input;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using HipMobileUI.ViewModels.Views.ExhibitDetails;
using Xamarin.Forms;
using Page = de.upb.hip.mobile.pcl.BusinessLayer.Models.Page;

namespace HipMobileUI.ViewModels.Pages
{
    public class ExhibitDetailsViewModel : NavigationViewModel
    {

        private ExhibitSubviewViewModel selectedView;
        private readonly Exhibit exhibit;
        private ICommand nextViewCommand;
        private ICommand previousViewCommand;
        private ICommand audioToolbarCommand;
        private bool previousViewAvailable;
        private bool nextViewAvailable;
        private int currentViewIndex;
        private bool audioAvailabe;
        private bool audioToolbarVisible;
        private double maxAudioProgress;
        private double currentAudioProgress;
        private ICommand playCommand;
        private ICommand pauseCommand;
        private ICommand captionCommand;
        private bool isAudioPlaying;

        public ExhibitDetailsViewModel (string exhibitId) : this(ExhibitManager.GetExhibit(exhibitId))
        {
        }

        public ExhibitDetailsViewModel (Exhibit exhibit)
        {
            currentViewIndex = 0;
            if (exhibit != null)
            {
                this.exhibit = exhibit;
                SetCurrentView ();
                Title = exhibit.Name;
                if(exhibit.Pages.Count>1)
                    NextViewAvailable = true;
            }
            NextViewCommand = new Command (GotoNextView);
            PreviousViewCommand = new Command (GotoPreviousView);

            ShowAudioToolbarCommand = new Command (SwitchAudioToolbarVisibleState);
            PauseCommand = new Command (PauseAudio);
            PlayCommand = new Command(PlayAudio);
            CaptionCommand = new Command(ShowCaption);
        }

        private void GotoNextView ()
        {
            if (currentViewIndex < exhibit.Pages.Count - 1)
            {
                currentViewIndex++;
                SetCurrentView ();
                NextViewAvailable = currentViewIndex < exhibit.Pages.Count - 1;
                PreviousViewAvailable = true;
            }
        }

        private void PlayAudio ()
        {
            IsAudioPlaying = true;
        }

        private void PauseAudio ()
        {
            IsAudioPlaying = false;
        }

        private void ShowCaption ()
        {
            Navigation.DisplayAlert ("Caption", "The caption dialog will be shown here!", "OK");
        }

        private void SwitchAudioToolbarVisibleState()
        {
            AudioToolbarVisible = !AudioToolbarVisible;
        }

        private void GotoPreviousView ()
        {
            if (currentViewIndex > 0)
            {
                currentViewIndex--;
                SetCurrentView();
                PreviousViewAvailable = currentViewIndex > 0;
                NextViewAvailable = true;
            }
        }

        private void SetCurrentView ()
        {
            Page currentPage = exhibit.Pages [currentViewIndex];

            AudioAvailable = currentPage.Audio != null;
            if (!AudioAvailable)
            {
                AudioToolbarVisible = false;
            }
            MaxAudioProgress = 150; //TODO: Add length of audio

            if (currentPage.IsAppetizerPage ())
            {
                SelectedView = new AppetizerViewModel (exhibit.Name, currentPage.AppetizerPage);
            }
            else if (currentPage.IsImagePage())
            {
                SelectedView = new ImageViewModel(currentPage.ImagePage);
            }
            else if (currentPage.IsTextPage())
            {

            }
            else if (currentPage.IsTimeSliderPage())
            {
                SelectedView = new TimeSliderViewModel();
            }
            else
            {
                throw new Exception("Unknown page found: " + currentPage);
            }
        }


        #region propeties
        /// <summary>
        /// The currently displayed subview.
        /// </summary>
        public ExhibitSubviewViewModel SelectedView
        {
            get { return selectedView; }
            set { SetProperty(ref selectedView, value); }
        }

        /// <summary>
        /// The command for switching to the next view, if available.
        /// </summary>
        public ICommand NextViewCommand
        {
            get { return nextViewCommand; }
            set { SetProperty(ref nextViewCommand, value); }
        }

        /// <summary>
        /// The command for switching to the previous view, if available.
        /// </summary>
        public ICommand PreviousViewCommand
        {
            get { return previousViewCommand; }
            set { SetProperty(ref previousViewCommand, value); }
        }

        /// <summary>
        /// Indicator if a previous view is available.
        /// </summary>
        public bool PreviousViewAvailable
        {
            get { return previousViewAvailable; }
            set { SetProperty(ref previousViewAvailable, value); }
        }

        /// <summary>
        /// Indicator if a next view is available.
        /// </summary>
        public bool NextViewAvailable
        {
            get { return nextViewAvailable; }
            set { SetProperty(ref nextViewAvailable, value); }
        }

        /// <summary>
        /// Shows the audio toolbar
        /// </summary>
        public ICommand ShowAudioToolbarCommand
        {
            get { return audioToolbarCommand; }
            set { SetProperty (ref audioToolbarCommand, value); }
        }

        /// <summary>
        /// Indicates whether the current page has audio available
        /// </summary>
        public bool AudioAvailable
        {
            get { return audioAvailabe; }
            set { SetProperty(ref audioAvailabe, value); }
        }

        /// <summary>
        /// Indicates whether the audio toolbar is visible
        /// </summary>
        public bool AudioToolbarVisible
        {
            get { return audioToolbarVisible; }
            set { SetProperty(ref audioToolbarVisible, value); }
        }

        /// <summary>
        /// The maximum value of the audio progress
        /// </summary>
        public double MaxAudioProgress
        {
            get { return maxAudioProgress;}
            set { SetProperty (ref maxAudioProgress, value); }
        }

        /// <summary>
        /// The current value of the audio progress
        /// </summary>
        public double CurrentAudioProgress
        {
            get { return currentAudioProgress; }
            set { SetProperty (ref currentAudioProgress, value); }
        }

        public ICommand PlayCommand
        {
            get { return playCommand; }
            set { SetProperty (ref playCommand, value); }
        }

        public ICommand PauseCommand
        {
            get { return pauseCommand; }
            set { SetProperty(ref pauseCommand, value); }
        }

        public ICommand CaptionCommand
        {
            get { return captionCommand; }
            set { SetProperty (ref captionCommand, value); }
        }

        public bool IsAudioPlaying
        {
            get { return isAudioPlaying; }
            set { SetProperty(ref isAudioPlaying, value); }
        }
        
        #endregion


    }
}
