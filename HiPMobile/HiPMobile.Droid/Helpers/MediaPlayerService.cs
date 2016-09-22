// Copyright (C) 2016 History in Paderborn App - Universität Paderborn
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
using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Util;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using Java.IO;

namespace de.upb.hip.mobile.droid.Helpers {
    /// <summary>
    ///     This class controls playing audio files. As the audio needs to be playable regardless of the screen activity
    ///     and also while the screen is off, this needs to be a service.The service should be started from one activity
    ///     and then be given to the other activities in which it is used, otherwise a good control is impossible.
    /// </summary>
    [Service]
    public class MediaPlayerService : Service, MediaPlayer.IOnPreparedListener, MediaPlayer.IOnErrorListener {

        private readonly IBinder binder;
        private readonly string logtag = "MEDIAPLAYERSERVICE";

        private MediaPlayer mediaPlayer;

        public MediaPlayerService ()
        {
            binder = new MediaPlayerBinder (this);
        }

        public bool AudioFileIsSet { get; private set; }

        public bool OnError (MediaPlayer mp, MediaError what, int extra)
        {
            return false;
        }

        public void OnPrepared (MediaPlayer mp)
        {
            //the media player will be prepared when starting an activity,
            // but that may not be the right time to start the audio
            //therefore this is empty
        }

        public override IBinder OnBind (Intent intent)
        {
            return binder;
        }

        public override void OnCreate ()
        {
        }

        //following are the functions for the app to use to control the media player
        public void StartSound ()
        {
            if (AudioFileIsSet)
            {
                mediaPlayer.Start ();
            }
        }

        public void StopSound ()
        {
            if (AudioFileIsSet)
            {
                mediaPlayer.Stop ();
            }
        }

        public void PauseSound ()
        {
            if (AudioFileIsSet)
            {
                mediaPlayer.Pause ();
            }
        }

        /// <summary>
        ///     Sets a specific audio file and prepares it for beeing played
        /// </summary>
        /// <param name="audio"></param>
        public void SetAudioFile (Audio audio)
        {
            try
            {
                if (mediaPlayer != null)
                {
                    mediaPlayer.Stop ();
                    mediaPlayer.Reset ();
                }

                var path = CopyAudioToTemp (audio);
                mediaPlayer = new MediaPlayer ();
                mediaPlayer.SetAudioStreamType (Stream.Music);
                mediaPlayer.SetDataSource (path);
                mediaPlayer.Prepare ();
                AudioFileIsSet = true;
            }
            catch (Exception e)
            {
                Log.Error (logtag, e.Message);
            }
        }

        private string CopyAudioToTemp (Audio audio)
        {
            var filepath = "";
            try
            {
                var tempMp3 = File.CreateTempFile ("temp", ".mp3", CacheDir);
                tempMp3.DeleteOnExit ();
                var fos = new FileOutputStream (tempMp3);
                fos.Write (audio.Data);
                fos.Close ();
                filepath = tempMp3.AbsolutePath;
            }
            catch (IOException ioe)
            {
                Log.Warn (logtag, "Could not write audio to temp file, exception message:" + ioe.Message);
            }
            return filepath;
        }

        /// <summary>
        ///     Gets the total time for the currently playing audio.
        /// </summary>
        /// <returns>The total duration of the audio.</returns>
        public long GetTimeTotal ()
        {
            return mediaPlayer.Duration;
        }

        /// <summary>
        ///     Gets the currently played time for the audio.
        /// </summary>
        /// <returns>The already played duration of the audio.</returns>
        public long GetTimeCurrent ()
        {
            return mediaPlayer.CurrentPosition;
        }

        /// <summary>
        ///     Seeks to the given time in the audio.
        /// </summary>
        /// <param name="time">The time in miliseconds to seek to.</param>
        public void SeekTo (int time)
        {
            mediaPlayer.SeekTo (time);
        }

        public void AddOnCompleteListener (EventHandler del)
        {
            if (mediaPlayer != null)
            {
                Log.Info(logtag, "Added audio listener");
                mediaPlayer.Completion += del;
            }
        }

        public void RemoveOnCompleteListener(EventHandler del)
        {
            if (mediaPlayer != null)
            {
                Log.Info (logtag, "Removed audio listener");
                mediaPlayer.Completion -= del;
            }
        }

        public class MediaPlayerBinder : Binder {

            private readonly MediaPlayerService service;

            public MediaPlayerBinder (MediaPlayerService service)
            {
                this.service = service;
            }

            public MediaPlayerService GetService ()
            {
                return service;
            }
        }
    }
}