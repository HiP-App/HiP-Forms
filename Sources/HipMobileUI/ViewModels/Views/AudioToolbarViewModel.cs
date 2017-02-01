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

using System.Windows.Input;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using Xamarin.Forms;

namespace HipMobileUI.ViewModels.Views {
    /// <summary>
    /// ViewModel for the audio toolbar which controls playing the audio
    /// </summary>
    public class AudioToolbarViewModel : NavigationViewModel {

        private double maxAudioProgress;
        private double currentAudioProgress;
        private ICommand playCommand;
        private ICommand pauseCommand;
        private ICommand captionCommand;
        private bool isAudioPlaying;
        private readonly bool automaticallyStartNewAudio;

        /// <summary>
        /// Creates a new audio toolbar viewmodel and specifies whether a new passed audio
        /// will be played automatically
        /// </summary>
        /// <param name="automaticallyStartNewAudio"></param>
        public AudioToolbarViewModel (bool automaticallyStartNewAudio) 
        {
            PauseCommand = new Command(PauseAudio);
            PlayCommand = new Command(PlayAudio);
            CaptionCommand = new Command(ShowCaption);

            this.automaticallyStartNewAudio = automaticallyStartNewAudio;
        }

        /// <summary>
        /// Creates a new audio toolbar viewmodel which does not automatically start new audio files
        /// </summary>
        public AudioToolbarViewModel () : this(false)
        {
            
        }

        private void PlayAudio()
        {
            IsAudioPlaying = true;
        }

        private void PauseAudio()
        {
            IsAudioPlaying = false;
        }

        private void ShowCaption()
        {
            Navigation.DisplayAlert("Caption", "The caption dialog will be shown here!", "OK");
        }

        /// <summary>
        /// Sets a new audio file ready for play
        /// </summary>
        /// <param name="newAudio"></param>
        public void SetNewAudioFile (Audio newAudio)
        {
            //Stop audio
            CurrentAudioProgress = 0;
            MaxAudioProgress = 150; //TODO: Add length of audio
            if (automaticallyStartNewAudio)
            {
                //Start audio
            }
        }

        #region Properties

        /// <summary>
        /// The maximum value of the audio progress
        /// </summary>
        public double MaxAudioProgress
        {
            get { return maxAudioProgress; }
            set { SetProperty(ref maxAudioProgress, value); }
        }

        /// <summary>
        /// The current value of the audio progress
        /// </summary>
        public double CurrentAudioProgress
        {
            get { return currentAudioProgress; }
            set { SetProperty(ref currentAudioProgress, value); }
        }

        /// <summary>
        /// Command for playing audio
        /// </summary>
        public ICommand PlayCommand
        {
            get { return playCommand; }
            set { SetProperty(ref playCommand, value); }
        }

        /// <summary>
        /// Command for pausing audio
        /// </summary>
        public ICommand PauseCommand
        {
            get { return pauseCommand; }
            set { SetProperty(ref pauseCommand, value); }
        }

        /// <summary>
        /// Command for showing the caption
        /// </summary>
        public ICommand CaptionCommand
        {
            get { return captionCommand; }
            set { SetProperty(ref captionCommand, value); }
        }

        /// <summary>
        /// Indicates whether audio is playing
        /// </summary>
        public bool IsAudioPlaying
        {
            get { return isAudioPlaying; }
            set { SetProperty(ref isAudioPlaying, value); }
        }

        #endregion
    }
}