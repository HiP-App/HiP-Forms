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

using System.Windows.Input;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using Xamarin.Forms;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views
{
    /// <summary>
    /// ViewModel for the SettingsScreenViewModel.
    /// </summary>
    public class SettingsScreenViewModel : NavigationViewModel
    {
        private string size;

        public SettingsScreenViewModel()
        {
            RemoveAllDownloads = new Command(RemoveAllDownloadsClicked);
            SelectCharacterCommand = new Command(OnSelectCharacterTapped);
            Size = BuildSizeDescription();
        }

        public ICommand RemoveAllDownloads { get; }
        public ICommand SelectCharacterCommand { get; }

        private void OnSelectCharacterTapped()
        {
            Navigation.StartNewLocalNavigationStack(new CharacterSelectionPageViewModel(this));
        }

        private async void RemoveAllDownloadsClicked()
        {
            var result = await IoCManager.Resolve<INavigationService>()
                                         .DisplayAlert(Strings.SettingsScreenView_RemoveAllDownloadsPrompt_Title, Strings.SettingsScreenView_RemoveAllDownloadsPrompt_Question,
                                                       Strings.SettingsScreenView_RemoveAllDownloadsPrompt_Confirm, Strings.Cancel);

            if (result)
            {
                // Delete the whole DB
                DbManager.DeleteDatabase();
                Size = BuildSizeDescription();
            }
        }

        private static string BuildSizeDescription()
        {
            var dbSizeMb = IoCManager.Resolve<IStorageSizeProvider>().GetDatabaseSizeMb();
            var mediaSizeMb = IoCManager.Resolve<IMediaFileManager>().TotalSizeBytes / (1024 * 1024);
            return $"{dbSizeMb + mediaSizeMb} MB";
        }

        /// <summary>
        /// After end of audio playback, switch automatically to next page
        /// </summary>
        public bool AutoSwitchPage
        {
            get => Settings.AutoSwitchPage;
            set

            {
                Settings.AutoSwitchPage = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Automatically start audio playback for current page
        /// </summary>
        public bool AutoStartAudio
        {
            get => Settings.AutoStartAudio;
            set

            {
                Settings.AutoStartAudio = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Show hint for audio playback again
        /// </summary>
        public bool RepeatHintAudio
        {
            get => Settings.RepeatHintAudio;
            set

            {
                Settings.RepeatHintAudio = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Show hint for automatically switching to next page again
        /// </summary>
        public bool RepeatHintAutoPageSwitch
        {
            get => Settings.RepeatHintAutoPageSwitch;
            set

            {
                Settings.RepeatHintAutoPageSwitch = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Show app introduction by restarting the app again
        /// </summary>
        public bool RepeatIntro
        {
            get => Settings.RepeatIntro;
            set

            {
                Settings.RepeatIntro = value;
                Settings.InitialThemeSelected = !value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Allow user to enable prompt for data download when data updated
        /// </summary>
        public bool AlwaysDownloadData
        {
            get => Settings.AlwaysDownloadData;
            set

            {
                Settings.AlwaysDownloadData = value;
                OnPropertyChanged();
            }
        }


        public string AppModeText => Settings.AdventurerMode ? Strings.SettingsScreenView_CharacterSelection_Text_IsAdventurer : Strings.SettingsScreenView_CharacterSelection_Text_IsProfessor;

        public override void OnAppearing()
        {
            base.OnAppearing();
            Size = BuildSizeDescription();
        }

        public override void OnRevealed()
        {
            base.OnRevealed();
            Size = BuildSizeDescription();
        }

        public string Size
        {
            set
            {
                if (size != value)
                {
                    size = value;
                    OnPropertyChanged();
                }
            }
            get => size;
        }


        public bool WifiOnly
        {
            get => Settings.WifiOnly;
            set

            {
                Settings.WifiOnly = value;
                OnPropertyChanged();
            }
        }
    }
}