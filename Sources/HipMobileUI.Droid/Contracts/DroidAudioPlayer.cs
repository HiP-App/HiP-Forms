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
using System.IO;
using System.Threading;
using Android.Media;
using Android.Util;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using HipMobileUI.AudioPlayer;
using Java.IO;
using File = Java.IO.File;
using IOException = Java.IO.IOException;
using Stream = Android.Media.Stream;

namespace de.upb.hip.mobile.droid.Contracts {
    internal class DroidAudioPlayer : IAudioPlayer {

        private readonly MediaPlayer mediaPlayer;
        private Audio currentAudio;
        private Timer progressUpdateTimer;

        public DroidAudioPlayer ()
        {
            mediaPlayer = new MediaPlayer ();
            mediaPlayer.SetAudioStreamType (Stream.Music);
            mediaPlayer.Completion += MediaPlayerOnCompletion;
        }

        public bool IsPlaying => mediaPlayer.IsPlaying;

        public double CurrentProgress => mediaPlayer.CurrentPosition;

        public double MaximumProgress => mediaPlayer.Duration;

        public Audio CurrentAudio {
            get { return currentAudio; }
            set {
                currentAudio = value;
                mediaPlayer.Stop ();
                mediaPlayer.Reset ();
                if (value != null)
                {
                    var path = CopyAudioToTemp (value);
                    mediaPlayer.SetDataSource (path);
                    mediaPlayer.Prepare ();
                }
            }
        }

        public event ProgressChangedDelegate ProgressChanged;
        public event IsPlayingDelegate IsPlayingChanged;
        public event AudioCompletedDelegate AudioCompleted;

        public void Play ()
        {
            mediaPlayer.Start ();
            IsPlayingChanged?.Invoke (true);
            StartUpdateTimer ();
        }

        public void Pause ()
        {
            mediaPlayer.Pause ();
            IsPlayingChanged?.Invoke(false);
            StopUpdateTimer ();
        }

        public void Stop ()
        {
            mediaPlayer.Stop ();
            IsPlayingChanged?.Invoke(false);
            StopUpdateTimer ();
        }

        public void SeekTo (double progress)
        {
            mediaPlayer.SeekTo (Convert.ToInt32 (progress));
        }

        private void MediaPlayerOnCompletion (object sender, EventArgs eventArgs)
        {
            StopUpdateTimer ();
            AudioCompleted?.Invoke ();
            IsPlayingChanged?.Invoke(IsPlaying);
        }

        private void UpdateProgress (object state)
        {
            ProgressChanged?.Invoke (CurrentProgress);
        }

        private string CopyAudioToTemp (Audio audio)
        {
            var filepath = "";
            try
            {
                var tempMp3 = File.CreateTempFile ("temp", ".mp3", new File (Path.GetTempPath ()));
                tempMp3.DeleteOnExit ();
                var fos = new FileOutputStream (tempMp3);
                fos.Write (audio.Data);
                fos.Close ();
                filepath = tempMp3.AbsolutePath;
            }
            catch (IOException ioe)
            {
                Log.Warn ("AndroidMediaPlayer", "Could not write audio to temp file, exception message:" + ioe.Message);
            }
            return filepath;
        }

        private void StartUpdateTimer ()
        {
            progressUpdateTimer = new Timer (UpdateProgress, null, 0, 16);
        }

        private void StopUpdateTimer ()
        {
            progressUpdateTimer?.Dispose ();
        }

    }
}