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

        public IAudioPlayer AudioPlayer { get; private set; }

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

            AudioPlayer = IoCManager.Resolve<IAudioPlayer> ();
            AudioPlayer.IsPlayingChanged += value => {
                IsAudioPlaying = value;
            };
            AudioPlayer.ProgressChanged += AudioPlayerOnProgressChanged;
            AudioPlayer.AudioCompleted += AudioPlayerOnAudioCompleted;
            AudioPlayer.IsPlayingChanged += AudioPlayerOnIsPlayingChanged;

            this.automaticallyStartNewAudio = automaticallyStartNewAudio;
        }

        private void AudioPlayerOnIsPlayingChanged (bool value)
        {
            IsAudioPlaying = value;
        }

        private void AudioPlayerOnAudioCompleted ()
        {
            CurrentAudioProgress = 0;
            IsAudioPlaying = false;
        }

        private void AudioPlayerOnProgressChanged (double oldProgress, double newProgress)
        {
            if (!disableAutomaticUpdate)
                CurrentAudioProgress = newProgress;
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
            AudioPlayer.Play ();
        }

        private void PauseAudio()
        {
            AudioPlayer.Pause ();
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
            AudioPlayer.CurrentAudio = newAudio;
            if (newAudio != null)
                MaxAudioProgress = AudioPlayer.MaximumProgress;
            else
                MaxAudioProgress = 1;
            if (automaticallyStartNewAudio)
            {
                //Start audio
                AudioPlayer.Play ();
            }
        }

        public override void OnDisappearing ()
        {
            base.OnDisappearing ();

            AudioPlayer.Stop ();
            AudioPlayer.ProgressChanged -= AudioPlayerOnProgressChanged;
            AudioPlayer.AudioCompleted -= AudioPlayerOnAudioCompleted;
            AudioPlayer.IsPlayingChanged -= AudioPlayerOnIsPlayingChanged;
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
                    AudioPlayer.CurrentProgress = value;
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