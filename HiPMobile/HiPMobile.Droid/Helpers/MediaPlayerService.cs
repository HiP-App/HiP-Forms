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

        public static readonly string ACTION_PLAY = "de.upb.hip.mobile.PLAY"; //the intentions can probably be erased
        public static readonly string ACTION_STOP = "de.upb.hip.mobile.STOP";

        private Audio a1 = BusinessEntitiyFactory.CreateBusinessObject<Audio> ();

        private bool AudioFileIsSet;
        private readonly IBinder binder;

        private MediaPlayer mediaPlayer;

        public MediaPlayerService ()
        {
            binder = new MediaPlayerBinder (this);
        }

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
            // is this the onCreate in android?
            try
            {
                //the service is bound, therefore this function is used rather than OnStartCommand
                //mediaPlayer = MediaPlayer.Create(this, a1.getAudioDir());
                mediaPlayer.SetAudioStreamType (Stream.Music);
                mediaPlayer.SetOnPreparedListener (this);
                mediaPlayer.PrepareAsync ();
            }
            catch (Exception e)
            {
            }
        }

        public int OnStartCommand (Intent intent, int flags, int startId)
        {
            try
            {
                //            mMediaPlayer = MediaPlayer.create(this, songList[current].getAudioDir());
                mediaPlayer.SetAudioStreamType (Stream.Music);
                mediaPlayer.SetOnPreparedListener (this);
                mediaPlayer.PrepareAsync ();
            }
            catch (Exception e)
            {
                //            add an exception handling
            }

            return 1; //keeps the service running until told otherwise
            // 1 = START_STICKY
        }

        //following are the functions for the app to use to control the media player
        public void StartSound ()
        {
            if (!AudioFileIsSet)
            {
                mediaPlayer = new MediaPlayer ();
            }
            mediaPlayer.Start ();
        }

        public void StopSound ()
        {
            mediaPlayer.Stop ();
        }

        public void PauseSound ()
        {
            mediaPlayer.Pause ();
        }

        public void changeAudioFile ()
        {
            try
            {
                mediaPlayer.Reset ();
            }
            catch (Exception e)
            {
            }
        }

        /** sets a specific audio file*/

        public void SetAudioFile (Audio audio)
        {
            try
            {
                if (mediaPlayer != null)
                {
                    mediaPlayer.Stop ();
                    mediaPlayer.Reset ();
                }
                //may be needed for handling audio files later. until know, leave this commented in here.
                var path = CopyAudioToTemp (audio);
                mediaPlayer = new MediaPlayer ();
                mediaPlayer.SetAudioStreamType (Stream.Music);
                mediaPlayer.SetDataSource (path);
                mediaPlayer.Prepare ();
                AudioFileIsSet = true;
            }
            catch (Exception e)
            {
                //            add an exception handling
            }
        }

        /** sets a specific audio file*/

        public void SetAudioFile (int audio)
        {
            try
            {
                mediaPlayer.Stop ();
                mediaPlayer.Reset ();
                //mediaPlayer = MediaPlayer.create(this, audio);
                AudioFileIsSet = true;
            }
            catch (Exception e)
            {
                //            add an exception handling
            }
        }

        public bool GetAudioFileIsSet ()
        {
            //since the mediaplayer is used as a service, in the beginning it can't yet be called
            //as it is not yet created. however, as soon as the play button is used, the media player
            //surely is existend. the first time, it is pushed, there is not yet set an audiofile
            //because it couldn't be set before, so this needs to be checked and an audio file can then
            //be set if necessary
            return AudioFileIsSet;
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
                Log.Warn ("MedialPlayerService", "Could not write audio to temp file");
            }
            return filepath;
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