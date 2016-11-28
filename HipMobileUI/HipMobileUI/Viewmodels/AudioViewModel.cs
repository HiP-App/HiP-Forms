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
using System.Runtime.CompilerServices;
using System.Windows.Input;
using de.upb.hip.mobile.pcl.Common;
using de.upb.hip.mobile.pcl.Common.Contracts;
using HipMobileUI.Annotations;
using Xamarin.Forms;
using Microsoft.Practices.Unity;

namespace HipMobileUI.Viewmodels {
    public class AudioViewModel : INotifyPropertyChanged {

        private IMediaPlayer player;

        public AudioViewModel ()
        {
            player = IoCManager.UnityContainer.Resolve<IMediaPlayer> ();
            Play = new Command (PlayAudio);
            Stop = new Command (StopAudio);
        }

        private void PlayAudio ()
        {
            player.Play ();
        }

        private void StopAudio ()
        {
            player.Pause ();   
        }

        public ICommand Play { get; set; }

        public ICommand Stop { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged ([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke (this, new PropertyChangedEventArgs (propertyName));
        }

    }
}