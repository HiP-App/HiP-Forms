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

using PaderbornUniversity.SILab.Hip.Mobile.UI.Properties;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.AudioPlayer
{
    public delegate void ProgressChangedDelegate(double newProgress);

    public delegate void AudioCompletedDelegate();

    public delegate void IsPlayingDelegate(bool newValue);

    /// <summary>
    /// Interface describing an audio player.
    /// </summary>
    public interface IAudioPlayer
    {
        /// <summary>
        /// Indicating if the audio player is currently playing.
        /// </summary>
        bool IsPlaying { get; }

        /// <summary>
        /// Gets the current progress in milliseconds.
        /// </summary>
        double CurrentProgress { get; }

        /// <summary>
        /// Gets the overall progress of the currently played track.
        /// </summary>
        double MaximumProgress { get; }

        /// <summary>
        /// Gets or sets the currently selected audio object.
        /// </summary>
        Shared.BusinessLayer.Models.Audio CurrentAudio { get; set; }

        /// <summary>
        /// Event that is raised about 60 times a seconds delivering the current audio progress.
        /// </summary>
        event ProgressChangedDelegate ProgressChanged;

        /// <summary>
        /// Event that is raised when the state of IsPlaying changed.
        /// </summary>
        event IsPlayingDelegate IsPlayingChanged;

        /// <summary>
        /// Event that is raised when the current audio finished playing.
        /// </summary>
        event AudioCompletedDelegate AudioCompleted;

        /// <summary>
        /// Start playing audio.
        /// </summary>
        void Play();

        /// <summary>
        /// Pauses the audio currently playing. Playback can be resumed with Play().
        /// </summary>
        void Pause();

        /// <summary>
        /// Stops the currently playing audio.
        /// </summary>
        void Stop();

        /// <summary>
        /// Sets the current playback location to 
        /// </summary>
        /// <param name="progress"></param>
        void SeekTo(double progress);

        /// <summary>
        /// Title for the audio track used in the audio captions
        /// </summary>
        string AudioTitle { set; }
    }
}