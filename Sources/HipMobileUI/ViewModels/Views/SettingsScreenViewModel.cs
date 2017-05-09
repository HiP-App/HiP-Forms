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

using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views {
    /// <summary>
    /// ViewModel for the SettingsScreenViewModel.
    /// </summary>
    public class SettingsScreenViewModel : NavigationViewModel {
        /// <summary>
        /// After end of audio playback, switch automatically to next page
        /// </summary>
        public bool AutoSwitchPage
        {
            get { return Settings.AutoSwitchPage; }
            set {
                Settings.AutoSwitchPage = value;
                OnPropertyChanged ();
            }
        }

        /// <summary>
        /// Automatically start audio playback for current page
        /// </summary>
        public bool AutoStartAudio
        {
            get { return Settings.AutoStartAudio; }
            set {
                Settings.AutoStartAudio = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Show hint for audio playback again
        /// </summary>
        public bool RepeatHintAudio
        {
            get { return Settings.RepeatHintAudio; }
            set {
                Settings.RepeatHintAudio = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Show hint for automatically switching to next page again
        /// </summary>
        public bool RepeatHintAutoPageSwitch
        {
            get { return Settings.RepeatHintAutoPageSwitch; }
            set {
                Settings.RepeatHintAutoPageSwitch = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Show app introduction by restarting the app again
        /// </summary>
        public bool RepeatIntro
        {
            get { return Settings.RepeatIntro; }
            set {
                Settings.RepeatIntro = value;
                OnPropertyChanged ();
            }
        }

		/// <summary>
		/// Allow user to enable prompt for data download when data updated
		/// </summary>
		public bool AlwaysDownloadData
		{
			get { return Settings.AlwaysDownloadData; }
			set
			{
				Settings.AlwaysDownloadData = value;
				OnPropertyChanged();
			}
		}


        public bool WifiOnly
        {
            get { return Settings.WifiOnly; }
            set
            {
                Settings.WifiOnly = value;
                OnPropertyChanged();
            }
        }

    }
}
