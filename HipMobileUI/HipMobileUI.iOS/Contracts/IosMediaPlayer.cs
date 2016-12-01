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

using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using AVFoundation;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using de.upb.hip.mobile.pcl.Common.Contracts;
using Foundation;
using HipMobileUI.Annotations;
using UIKit;

namespace HipMobileUI.iOS.Contracts {
    public class IosMediaPlayer : IMediaPlayer, INotifyPropertyChanged {

        private AVAudioPlayer player;

        public void Setup ()
        {
            var session = AVAudioSession.SharedInstance ();
            session.SetActive (true);
            session.SetCategory (AVAudioSessionCategory.Playback);
            UIApplication.SharedApplication.BeginReceivingRemoteControlEvents ();
        }

        public void TearDown ()
        {
            var session = AVAudioSession.SharedInstance();
            session.SetActive(false);
        }

        public void ChangeTrack (Audio audio)
        {
            NSError err;
            player = new AVAudioPlayer (NSData.FromArray (audio.Data), "mp3", out err);
        }

        public void Play ()
        {
            var exh = ExhibitManager.GetExhibits().First(e => e.Name.Equals("Die Pfalz Karls des Großen"));
            this.ChangeTrack(exh.Pages[1].Audio);
            player.Play ();
            Thread t = new Thread (() => {
                                       while (player.Playing)
                                       {
                                           OnPropertyChanged ("Progress");
                                           Thread.Sleep (100);
                                       }
                                   });
            t.Start();
        }

        public void Pause ()
        {
            player.Stop ();
        }

        public bool IsPlaying { get; }

        public double Progress {
            get {
                if (player != null && player.Playing)
                {
                    return player.CurrentTime / player.Duration;
                }
                return 0;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged ([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke (this, new PropertyChangedEventArgs (propertyName));
        }

    }
}