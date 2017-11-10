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

using System.Collections.ObjectModel;
using System.Windows.Input;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views
{
    public class ProfileScreenViewModel : ExtendedNavigationViewModel
    {
        private readonly MainPageViewModel mainPageViewModel;

        public ProfileScreenViewModel(MainPageViewModel mainPageVm)
        {
            mainPageViewModel = mainPageVm;

            Tabs = new ObservableCollection<string> { Strings.MainPageViewModel_OverviewPage, "Erfolge", "Statistik" };

            Achievements = new ObservableCollection<AchievementViewModel>();
            Device.BeginInvokeOnMainThread(InitAchievements);

            ChangeAppModeCommand = new Command(OnChangeAppModeTapped);
            Logout = new Command(LogoutDummy);
        }

        private async void InitAchievements()
        {
            Achievements.Clear();
            await new AchievementFetcher().FetchUnlockedAchievementsIntoDatabase(); // TODO Use return value
            var localAchievements = AchievementManager.GetAchievements();
            foreach (var achievement in localAchievements)
            {
                Achievements.Add(await AchievementViewModel.CreateFrom(achievement));
            }
        }

        public ICommand Logout { get; }
        public ICommand ChangeAppModeCommand { get; }

        //public ImageSource Avatar => ImageSource.FromFile ("ic_account_circle.png");
        public ImageSource Avatar => Settings.AdventurerMode ? ImageSource.FromFile("ic_adventurer.png") : ImageSource.FromFile("ic_professor.png");

        public string Username => Settings.Username;
        public string EMail => Settings.EMail;
        public int Score => Settings.Score;
        public string AchievementCount => Settings.Achievements + " / 30";
        public string Completeness => Settings.Completeness + "%";

        private void OnChangeAppModeTapped()
        {
            Navigation.StartNewNavigationStack(new CharacterSelectionPageViewModel(this));
        }

        async void LogoutDummy()
        {
            var result = await Navigation.DisplayAlert("Abmeldung", "Wollen Sie sich wirklich abmelden?", "Ja", "Nein");
            if (!result)
                return;
            Settings.IsLoggedIn = false;
            mainPageViewModel.UpdateAccountViews();
        }

        private ObservableCollection<AchievementViewModel> achievements;

        public ObservableCollection<AchievementViewModel> Achievements
        {
            get { return achievements; }
            set { SetProperty(ref achievements, value); }
        }

        private ObservableCollection<string> tabs;

        public ObservableCollection<string> Tabs
        {
            get { return tabs; }
            set { SetProperty(ref tabs, value); }
        }

        public string Character
        {
            get { return Settings.AdventurerMode ? Strings.ProfileView_Adventurer : Strings.ProfileView_Professor; }
        }
    }
}