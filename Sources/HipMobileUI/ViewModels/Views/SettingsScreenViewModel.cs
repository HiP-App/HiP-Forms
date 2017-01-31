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

namespace HipMobileUI.ViewModels.Views {
    /// <summary>
    /// ViewModel for the SettingsScreenViewModel.
    /// </summary>
    public class SettingsScreenViewModel : NavigationViewModel {
        /// <summary>
        /// After end of audio playback, switch automatically to next page
        /// </summary>
        private bool autoSwitchPage;
        public bool AutoSwitchPage
        {
            get { return autoSwitchPage; }
            set { SetProperty(ref autoSwitchPage, value); }
        }

        /// <summary>
        /// Automatically start audio playback for current page
        /// </summary>
        private bool autoStartAudio;
        public bool AutoStartAudio
        {
            get { return autoStartAudio; }
            set { SetProperty(ref autoStartAudio, value); }
        }

        /// <summary>
        /// Show hint for audio playback again
        /// </summary>
        private bool repeatHintAudio;
        public bool RepeatHintAudio
        {
            get { return repeatHintAudio; }
            set { SetProperty(ref repeatHintAudio, value); }
        }

        /// <summary>
        /// Show hint for automatically switching to next page again
        /// </summary>
        private bool repeatHintAutoPageSwitch;
        public bool RepeatHintAutoPageSwitch
        {
            get { return repeatHintAutoPageSwitch; }
            set { SetProperty(ref repeatHintAutoPageSwitch, value); }
        }

        /// <summary>
        /// Show hint for timeslider again
        /// </summary>
        private bool repeatHintTimeSlider;
        public bool RepeatHintTimeSlider
        {
            get { return repeatHintTimeSlider; }
            set { SetProperty(ref repeatHintTimeSlider, value); }
        }

        /// <summary>
        /// Show app introduction by restarting the app again
        /// </summary>
        private bool repeatIntro;
        public bool RepeatIntro
        {
            get { return repeatIntro; }
            set { SetProperty(ref repeatIntro, value); }
        }
    }
}
