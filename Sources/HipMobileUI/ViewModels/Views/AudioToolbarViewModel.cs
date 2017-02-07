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
using System.ComponentModel;
using System.Windows.Input;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using de.upb.hip.mobile.pcl.Common;
using HipMobileUI.AudioPlayer;
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

        private IAudioPlayer audioPlayer;

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

            audioPlayer = IoCManager.Resolve<IAudioPlayer> ();
            audioPlayer.IsPlayingChanged += value => {
                IsAudioPlaying = value;
            };
            audioPlayer.ProgressChanged += (oldProgress, newProgress) => {
                if(!disableAutomaticUpdate)CurrentAudioProgress = newProgress;
            };

            this.automaticallyStartNewAudio = automaticallyStartNewAudio;
        }

        private bool disableAutomaticUpdate;

        /// <summary>
        /// Creates a new audio toolbar viewmodel which does not automatically start new audio files
        /// </summary>
        public AudioToolbarViewModel () : this(false)
        {
            
        }

        private void PlayAudio()
        {
            audioPlayer.Play ();
            IsAudioPlaying = true;
        }

        private void PauseAudio()
        {
            audioPlayer.Pause ();
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
            audioPlayer.CurrentAudio = newAudio;
            if (newAudio != null)
                MaxAudioProgress = audioPlayer.MaximumProgress;
            else
                MaxAudioProgress = 1;//TODO: Add length of audio
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
            set {
                var diff = (CurrentAudioProgress - value);
                
                if ( Math.Abs(diff) > 2000)
                {
                    // user changed value of slider manually
                    disableAutomaticUpdate = true;
                    audioPlayer.CurrentProgress = value;
                    disableAutomaticUpdate = false;
                }
                if (Math.Abs(diff) > 1000 || diff > 0)
                {
                    SetProperty(ref currentAudioProgress, value);
                }

            }
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