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
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.UserManagement;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.UserApiAccesses;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views
{
    public class ProfileScreenViewModel : NavigationViewModel
    {
        private readonly MainPageViewModel mainPageViewModel;
        private GridLength completenessBar;
        private ObservableCollection<string> tabs;

        private int maxAchievementsCount;

        private const string AdventurerImage = "ic_adventurer.png";
        private const string ProfessorImage = "ic_professor.png";

        private ProfilePictureApiAccess client;
        private ProfilePictureManager manager;

        public ProfileScreenViewModel(MainPageViewModel mainPageVm)
        {
            mainPageViewModel = mainPageVm;

            Tabs = new ObservableCollection<string> { Strings.MainPageViewModel_OverviewPage, Strings.ProfileView_Statistic };

            AppModeVisible = !Settings.DisableAdventurerMode;
            ChangeAppModeCommand = new Command(OnChangeAppModeTapped);
            Logout = new Command(LogoutDummy);
            GoToAchievementsCommand = new Command(GoToAchievements);
            OnImageTappedCommand = new Command(OnImageTapped);

            //Add maximum number of achievements here
            maxAchievementsCount = 120;

            SetCompletenessBarLength(Settings.Achievements);

            client = new ProfilePictureApiAccess(new UserApiClient(ServerEndpoints.RegisterUrl));
            manager = new ProfilePictureManager(client);
        }

        public override async void OnAppearing()
        {
            if (Settings.ProfilePicture == null)
            {
                var currentAvatar = await manager.GetProfilePicture(Settings.UserId, Settings.AccessToken);
                if (currentAvatar != null)
                {
                    Settings.ProfilePicture = Convert.ToBase64String(currentAvatar.Data);
                    Avatar =  ImageSource.FromStream(() => new MemoryStream(currentAvatar.Data));
                    return;
                }
            }
            else
            {
                var currentAvatarBytes = Convert.FromBase64String(Settings.ProfilePicture);
                Avatar = ImageSource.FromStream(() => new MemoryStream(currentAvatarBytes));
                return;
            }

            Avatar = Settings.AdventurerMode ? ImageSource.FromFile(AdventurerImage) : ImageSource.FromFile(ProfessorImage);
        }

        private void OnImageTapped()
        {
            GoToProfilePicture();
        }

        /// <summary>
        /// If the adventurer mode is disabled, hide the options for mode switching
        /// </summary>
        private bool appModeVisible;
        public bool AppModeVisible
        {
            get { return appModeVisible; }
            set
            {
                SetProperty(ref appModeVisible, value);
            }
        }

        private void OnChangeAppModeTapped()
        {
            Navigation.StartNewLocalNavigationStack(new CharacterSelectionPageViewModel(this));
        }

        private void SetCompletenessBarLength(int achievements)
        {
            if (achievements <= 0)
            {
                completenessBar = new GridLength(0, GridUnitType.Absolute);
            }
            else if (achievements >= maxAchievementsCount)
            {
                completenessBar = new GridLength(short.MaxValue, GridUnitType.Star);
            }
            else
            {
                var prop = (1 / (1 - (float)achievements / (float)maxAchievementsCount) - 1);
                completenessBar = new GridLength(prop, GridUnitType.Star);
            }
        }

        private void GoToAchievements()
        {
            mainPageViewModel.SwitchToAchievementsView();
        }

        private void GoToProfilePicture()
        {
            mainPageViewModel.SwitchToProfilePictureView();
        }

        async void LogoutDummy()
        {
            var result = await Navigation.DisplayAlert(Strings.ProfileScreenViewModel_Dialog_Logout_Title
                                                       , Strings.ProfileScreenViewModel_Dialog_Logout_Message
                                                       , Strings.Yes, Strings.No);
            if (!result)
                return;
            Settings.IsLoggedIn = false;
            mainPageViewModel.UpdateAccountViews();
        }

        #region Properties
        public ICommand Logout { get; }
        public ICommand ChangeAppModeCommand { get; }
        public ICommand GoToAchievementsCommand { get; }
        public ICommand OnImageTappedCommand { get; }

        private ImageSource avatar;
        public ImageSource Avatar
        {
            get { return avatar; }
            set { avatar = value; OnPropertyChanged(); }
        }

        public string Username => Settings.Username;
        public string EMail => Settings.EMail; // displaying the correct email
        public int Score => Settings.Score;
        public string AchievementCount => Settings.Achievements + " / " + maxAchievementsCount;
        public string Completeness => Settings.Completeness + "%";

        public GridLength CompletenessBar
        {
            get { return completenessBar; }
            set { SetProperty(ref completenessBar, value); }
        }

        public ObservableCollection<string> Tabs
        {
            get { return tabs; }
            set { SetProperty(ref tabs, value); }
        }

        public string Character
        {
            get { return Settings.AdventurerMode ? Strings.ProfileView_Adventurer : Strings.ProfileView_Professor; }
        }
        #endregion
    }
}