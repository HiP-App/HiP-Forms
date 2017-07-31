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
using System.Threading;
using AVFoundation;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using Foundation;
using MediaPlayer;
using PaderbornUniversity.SILab.Hip.Mobile.UI.AudioPlayer;
using UIKit;

namespace PaderbornUniversity.SILab.Hip.Mobile.Ios.Contracts {
    internal class IosAudioPlayer : IAudioPlayer {

        private AVAudioPlayer avAudioPlayer;
        private Audio currentAudio;
        private Timer progressUpdateTimer;

        public IosAudioPlayer ()
        {
            var session = AVAudioSession.SharedInstance ();
            session.SetActive (true);
            session.SetCategory (AVAudioSessionCategory.Playback);

            var sharedCenter = MPRemoteCommandCenter.Shared;
            sharedCenter.PlayCommand.Enabled = true;
            sharedCenter.PlayCommand.AddTarget(PlayCommand);

            sharedCenter.PauseCommand.Enabled = true;
            sharedCenter.PauseCommand.AddTarget(PauseCommand);

            UIApplication.SharedApplication.BeginReceivingRemoteControlEvents();
        }

        /// <summary>
        /// Command for the play button on lockscreen and control center
        /// </summary>
        /// <param name="commandEvent"></param>
        /// <returns></returns>
        private MPRemoteCommandHandlerStatus PlayCommand(MPRemoteCommandEvent commandEvent)
        {
            Play ();
            return MPRemoteCommandHandlerStatus.Success;
        }

        /// <summary>
        /// Command for the pause button on lockscreen and control center
        /// </summary>
        /// <param name="commandEvent"></param>
        /// <returns></returns>
        private MPRemoteCommandHandlerStatus PauseCommand(MPRemoteCommandEvent commandEvent)
        {
            Pause ();
            return MPRemoteCommandHandlerStatus.Success;
        }

        public bool IsPlaying => avAudioPlayer?.Playing ?? false;

        public double CurrentProgress => avAudioPlayer.CurrentTime*1000;

        public double MaximumProgress => avAudioPlayer.Duration * 1000;

        public Audio CurrentAudio {
            get { return currentAudio; }
            set {
                currentAudio = value;
                if (value != null)
                {
                    NSError err;
                    avAudioPlayer = new AVAudioPlayer (NSData.FromArray (value.Data), "mp3", out err);
                    avAudioPlayer.FinishedPlaying += OnAvAudioPlayerOnFinishedPlaying;
                    UpdateNowPlayingInfo(0);
                }
            }
        }

        /// <summary>
        /// Called when the AVAudioPlayer finishes playing. Stops the update timer, informs listeners and updates the isPlaying state.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="args">The event parameters</param>
        private void OnAvAudioPlayerOnFinishedPlaying (object sender, AVStatusEventArgs args)
        {
            StopUpdateTimer ();
            AudioCompleted?.Invoke ();
            IsPlayingChanged?.Invoke (IsPlaying);
        }

        public event ProgressChangedDelegate ProgressChanged;
        public event IsPlayingDelegate IsPlayingChanged;
        public event AudioCompletedDelegate AudioCompleted;

        /// <summary>
        /// Updates the information shown on lockscreen and control center
        /// Note: Setting the rate is indispensable for properly showing the information
        /// </summary>
        /// <param name="rate"></param>
        private void UpdateNowPlayingInfo (double rate)
        {
            MPNowPlayingInfoCenter.DefaultCenter.NowPlaying = new MPNowPlayingInfo
            {
                Title = AudioTitle,
                Artwork = new MPMediaItemArtwork(UIImage.FromBundle("hiphop_transparent.png")),
                MediaType = MPNowPlayingInfoMediaType.Audio,
                PlaybackDuration = avAudioPlayer.Duration,
                PlaybackRate = rate,
                ElapsedPlaybackTime = avAudioPlayer.CurrentTime
            };
        }

        public void Play ()
        {
            avAudioPlayer.Play ();
            IsPlayingChanged?.Invoke (true);
            StartUpdateTimer ();

            UpdateNowPlayingInfo (avAudioPlayer.Rate);
        }

        public void Pause ()
        {
            avAudioPlayer.Pause ();
            IsPlayingChanged?.Invoke (false);
            StopUpdateTimer ();

            UpdateNowPlayingInfo(0);
        }

        public void Stop ()
        {
			if (IsPlaying)
			{ 
				avAudioPlayer.Stop ();
			}
            
            IsPlayingChanged?.Invoke (false);
            StopUpdateTimer ();

            MPNowPlayingInfoCenter.DefaultCenter.NowPlaying = null;
        }

        public void SeekTo (double progress)
        {
            avAudioPlayer.CurrentTime = progress / 1000;
        }

        /// <summary>
        /// Starts a timer that fires an progress update event at a fixed rate.
        /// </summary>
        private void StartUpdateTimer ()
        {
            progressUpdateTimer = new Timer (UpdateProgress, null, 0, 16);
        }

        /// <summary>
        /// Stops the timer sending progress updates.
        /// </summary>
        private void StopUpdateTimer ()
        {
            progressUpdateTimer?.Dispose ();
        }

        /// <summary>
        /// Informs all listeners about an updated progress.
        /// </summary>
        /// <param name="state">Ignored.</param>
        private void UpdateProgress (object state)
        {
            ProgressChanged?.Invoke (CurrentProgress);
        }

        public string AudioTitle { private get; set; }

    }
}