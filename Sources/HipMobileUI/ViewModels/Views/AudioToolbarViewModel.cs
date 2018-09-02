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
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.UI.AudioPlayer;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages.AudioTranscript;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views
{
    /// <summary>
    /// ViewModel for the audio toolbar which controls playing the audio
    /// </summary>
    public class AudioToolbarViewModel : NavigationViewModel
    {
        private double maxAudioProgress;
        private ICommand playCommand;
        private ICommand pauseCommand;
        private ICommand playPauseCommand;
        private ICommand captionCommand;
        private ICommand manualSeekCommand;
        private double audioSliderProgress;
        private Audio currentAudio;
        private bool isAudioPlaying;

        public bool IsVisible { get; }
        public IAudioPlayer AudioPlayer { get; private set; }
        public string ExhibitTitle { get; set; }

        public AudioToolbarViewModel(string exhibitTitle, bool isVisible)
        {
            ExhibitTitle = exhibitTitle;
            IsVisible = isVisible;
            PauseCommand = new Command(PauseAudio);
            PlayCommand = new Command(PlayAudio);
            CaptionCommand = new Command(ShowCaption);
            ManualSeekCommand = new Command(ManualSeek);

            AudioPlayer = IoCManager.Resolve<IAudioPlayer>();
            AudioPlayer.AudioCompleted += AudioPlayerOnAudioCompleted;
            AudioPlayer.IsPlayingChanged += AudioPlayerOnIsPlayingChanged;
            AudioPlayer.ProgressChanged += AudioPlayerOnProgressChanged;

            AudioPlayer.AudioTitle = exhibitTitle;
        }

        private void AudioPlayerOnProgressChanged(double newProgress)
        {
            AudioSliderProgress = newProgress;
        }

        private void AudioPlayerOnIsPlayingChanged(bool value)
        {
            IsAudioPlaying = value;
        }

        private void AudioPlayerOnAudioCompleted()
        {
            IsAudioPlaying = false;
        }

        private void PlayAudio()
        {
            if (currentAudio != null) //please ensure AudioToolbar is not visible if no audio file is loaded
            {
                AudioPlayer.Play();
            }
        }

        private void PauseAudio()
        {
            if (currentAudio != null) //please ensure AudioToolbar is not visible if no audio file is loaded
            {
                AudioPlayer.Pause();
            }
        }

        private void ShowCaption()
        {
            Navigation.PushAsync(new AudioTranscriptViewModel(AudioPlayer.CurrentAudio.Caption, ExhibitTitle));
        }

        private void ManualSeek(object value)
        {
            AudioPlayer.SeekTo((double) value);
        }

        /// <summary>
        /// Sets a new audio file ready for play
        /// </summary>
        /// <param name="newAudio"></param>
        public void SetNewAudioFile(Audio newAudio)
        {
            currentAudio = newAudio;
            AudioPlayer.CurrentAudio = newAudio;
            if (newAudio != null)
                MaxAudioProgress = AudioPlayer.MaximumProgress;
            else
                MaxAudioProgress = 1;
        }

        /// <summary>
        /// Deregister events and stop audio.
        /// </summary>
        public override void OnDisappearing()
        {
            base.OnDisappearing();

            AudioPlayer.Stop();
            IsAudioPlaying = false;
            AudioPlayer.AudioCompleted -= AudioPlayerOnAudioCompleted;
            AudioPlayer.IsPlayingChanged -= AudioPlayerOnIsPlayingChanged;
        }

        /// <summary>
        /// Deregister events and stop audio.
        /// </summary>
        public override void OnHidden()
        {
            base.OnHidden();

            AudioPlayer.Stop();
            IsAudioPlaying = false;
            AudioPlayer.AudioCompleted -= AudioPlayerOnAudioCompleted;
            AudioPlayer.IsPlayingChanged -= AudioPlayerOnIsPlayingChanged;
        }

        /// <summary>
        /// Register events
        /// </summary>
        public override void OnRevealed()
        {
            base.OnRevealed();

            AudioPlayer.AudioCompleted += AudioPlayerOnAudioCompleted;
            AudioPlayer.IsPlayingChanged += AudioPlayerOnIsPlayingChanged;
        }

        #region Properties

        /// <summary>
        /// The maximum value of the audio progress
        /// </summary>
        public double MaxAudioProgress
        {
            get => maxAudioProgress;
            set => SetProperty(ref maxAudioProgress, value);
        }

        /// <summary>
        /// Unified command for audio play/pause
        /// </summary>
        public ICommand PlayPauseCommand
        {
            get => IsAudioPlaying ? PauseCommand : PlayCommand;
            set => SetProperty(ref playPauseCommand, value);
        }

        /// <summary>
        /// Command for playing audio
        /// </summary>
        public ICommand PlayCommand
        {
            get => playCommand;
            set => SetProperty(ref playCommand, value);
        }

        /// <summary>
        /// Command for pausing audio
        /// </summary>
        public ICommand PauseCommand
        {
            get => pauseCommand;
            set => SetProperty(ref pauseCommand, value);
        }

        /// <summary>
        /// Command for showing the caption
        /// </summary>
        public ICommand CaptionCommand
        {
            get => captionCommand;
            set => SetProperty(ref captionCommand, value);
        }

        public ICommand ManualSeekCommand
        {
            get => manualSeekCommand;
            set => SetProperty(ref manualSeekCommand, value);
        }

        /// <summary>
        /// Indicates whether audio is playing
        /// </summary>
        public bool IsAudioPlaying
        {
            get => isAudioPlaying;
            set
            {
                SetProperty(ref isAudioPlaying, value);
                PlayPauseCommand = value ? PauseCommand : PlayCommand;
            }
        }

        public double AudioSliderProgress
        {
            get => audioSliderProgress;
            set => SetProperty(ref audioSliderProgress, value);
        }

        #endregion
    }
}