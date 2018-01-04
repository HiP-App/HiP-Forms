﻿using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using Xamarin.Forms;
using System.ComponentModel;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views
{
    public class AchievementsScreenViewModel : NavigationViewModel
    {
        public AchievementsScreenViewModel()
        {
            IsLoggedIn = Settings.IsLoggedIn;
            Achievements = new ObservableCollection<AchievementViewModel>();
            Settings.ChangeEvents.PropertyChanged += LoginChangedHandler;
            Device.BeginInvokeOnMainThread(async () => await UpdateAchievements());
        }

        private async void LoginChangedHandler(object o, PropertyChangedEventArgs args)
        {
            IsLoggedIn = Settings.IsLoggedIn;
            await UpdateAchievements();
        }

        private async Task UpdateAchievements()
        {
            Achievements.Clear();

            var newlyUnlocked = await IoCManager.Resolve<IAchievementFetcher>().UpdateAchievements();
            AchievementNotification.QueueAchievementNotifications(newlyUnlocked);
            foreach (var achievement in AchievementManager.GetAchievements())
            {
                Achievements.Add(AchievementViewModel.CreateFrom(achievement));
            }
            Score = $"{Strings.AchievementsScreenView_Score} {AppSharedData.CurrentAchievementsScore()}";
        }

        public override async void OnAppearing()
        {
            base.OnAppearing();
            await UpdateAchievements();
        }

        private ObservableCollection<AchievementViewModel> achievements;

        public ObservableCollection<AchievementViewModel> Achievements
        {
            get => achievements;
            set => SetProperty(ref achievements, value);
        }

        private string score;

        public string Score
        {
            get => score;
            set => SetProperty(ref score, value);
        }

        private bool isLoggedIn;

        public bool IsLoggedIn
        {
            get => isLoggedIn;
            set => SetProperty(ref isLoggedIn, value);
        }
    }
}