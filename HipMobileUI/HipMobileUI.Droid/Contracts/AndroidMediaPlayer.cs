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
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Android.Media;
using Android.Util;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using de.upb.hip.mobile.pcl.Common.Contracts;
using HipMobileUI.Annotations;
using Java.IO;

namespace HipMobileUI.Droid.Contracts {
    public class AndroidMediaPlayer : IMediaPlayer, INotifyPropertyChanged {

        private MediaPlayer mediaPlayer;

        public void Setup ()
        {
            mediaPlayer = new MediaPlayer ();
        }

        public void TearDown ()
        {
            throw new System.NotImplementedException ();
        }

        public void ChangeTrack (Audio audio)
        {
            try
            {
                if (mediaPlayer != null)
                {
                    mediaPlayer.Stop();
                    mediaPlayer.Reset();
                }

                var path = CopyAudioToTemp(audio);
                mediaPlayer = new MediaPlayer();
                mediaPlayer.SetAudioStreamType(Stream.Music);
                mediaPlayer.SetDataSource(path);
                mediaPlayer.Prepare();
            }
            catch (Exception e)
            {
                Log.Error("", e.Message);
            }
        }

        public void Play ()
        {
            var exh = ExhibitManager.GetExhibits ().First (e => e.Name.Equals ("Die Pfalz Karls des Großen"));
            this.ChangeTrack (exh.Pages[1].Audio);
            mediaPlayer.Start();
        }

        public void Pause ()
        {
            mediaPlayer.Stop ();
        }

        public bool IsPlaying { get; }
        public int Progress { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged ([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke (this, new PropertyChangedEventArgs (propertyName));
        }

        private string CopyAudioToTemp(Audio audio)
        {
            var filepath = "";
            try
            {
                var tempMp3 = File.CreateTempFile("temp", ".mp3", new File(System.IO.Path.GetTempPath()));
                tempMp3.DeleteOnExit();
                var fos = new FileOutputStream(tempMp3);
                fos.Write(audio.Data);
                fos.Close();
                filepath = tempMp3.AbsolutePath;
            }
            catch (IOException ioe)
            {
                Log.Warn("AndroidMediaPlayer", "Could not write audio to temp file, exception message:" + ioe.Message);
            }
            return filepath;
        }

    }
}