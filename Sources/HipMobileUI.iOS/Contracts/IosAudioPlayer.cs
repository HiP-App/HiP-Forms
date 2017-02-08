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

using System.Threading;
using AVFoundation;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using Foundation;
using HipMobileUI.AudioPlayer;
using UIKit;

namespace HipMobileUI.iOS.Contracts {
    internal class IosAudioPlayer : IAudioPlayer {

        private bool automaticUpdate;

        private AVAudioPlayer avAudioPlayer;
        private Audio currentAudio;
        private double currentProgress;
        private Timer progressUpdateTimer;

        public IosAudioPlayer ()
        {
            var session = AVAudioSession.SharedInstance ();
            session.SetActive (true);
            session.SetCategory (AVAudioSessionCategory.Playback);
            UIApplication.SharedApplication.BeginReceivingRemoteControlEvents ();
        }

        public bool IsPlaying => avAudioPlayer?.Playing ?? false;

        public double CurrentProgress {
            get { return currentProgress; }
            set {
                currentProgress = value;
                if (!automaticUpdate)
                    avAudioPlayer.CurrentTime = value / 1000;
            }
        }

        public double MaximumProgress { get; private set; }

        public Audio CurrentAudio {
            get { return currentAudio; }
            set {
                currentAudio = value;
                if (value != null)
                {
                    NSError err;
                    avAudioPlayer = new AVAudioPlayer (NSData.FromArray (value.Data), "mp3", out err);
                    CurrentProgress = 0;
                    MaximumProgress = avAudioPlayer.Duration * 1000;
                    avAudioPlayer.FinishedPlaying += OnAvAudioPlayerOnFinishedPlaying;
                }
            }
        }

        private void OnAvAudioPlayerOnFinishedPlaying (object sender, AVStatusEventArgs args)
        {
            StopUpdateTimer ();
            AudioCompleted?.Invoke ();
            IsPlayingChanged?.Invoke (IsPlaying);
        }

        public event ProgressChangedDelegate ProgressChanged;
        public event IsPlayingDelegate IsPlayingChanged;
        public event AudioCompletedDelegate AudioCompleted;

        public void Play ()
        {
            avAudioPlayer.Play ();
            IsPlayingChanged?.Invoke (true);
            StartUpdateTimer ();
        }

        public void Pause ()
        {
            avAudioPlayer.Pause ();
            IsPlayingChanged?.Invoke (false);
            StopUpdateTimer ();
        }

        public void Stop ()
        {
            avAudioPlayer.Stop ();
            IsPlayingChanged?.Invoke (false);
            StopUpdateTimer ();
        }

        private void StartUpdateTimer ()
        {
            progressUpdateTimer = new Timer (UpdateProgress, null, 0, 200);
        }

        private void StopUpdateTimer ()
        {
            progressUpdateTimer?.Dispose ();
        }

        private void UpdateProgress (object state)
        {
            automaticUpdate = true;
            var oldProgress = CurrentProgress;
            CurrentProgress = avAudioPlayer.CurrentTime * 1000;
            ProgressChanged?.Invoke (oldProgress, CurrentProgress);
            automaticUpdate = false;
        }

    }
}