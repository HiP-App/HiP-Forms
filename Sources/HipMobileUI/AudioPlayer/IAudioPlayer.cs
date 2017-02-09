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

namespace HipMobileUI.AudioPlayer {

    public delegate void ProgressChangedDelegate(double newProgress);

    public delegate void AudioCompletedDelegate ();

    public delegate void IsPlayingDelegate (bool newValue);

    public interface IAudioPlayer {

        bool IsPlaying { get; }

        double CurrentProgress { get; }

        double MaximumProgress { get; }

        de.upb.hip.mobile.pcl.BusinessLayer.Models.Audio CurrentAudio { get; set; }

        event ProgressChangedDelegate ProgressChanged;

        event IsPlayingDelegate IsPlayingChanged;

        event AudioCompletedDelegate AudioCompleted;

        void Play ();

        void Pause ();

        void Stop ();

        void SeekTo (double progress);

    }
}