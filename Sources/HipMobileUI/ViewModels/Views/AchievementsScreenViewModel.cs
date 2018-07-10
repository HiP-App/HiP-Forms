﻿using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using Xamarin.Forms;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.DtoToModelConverters;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;

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
            await BackupData.WaitForInitAsync();
            Achievements.Clear();

            try
            {
                var newlyUnlocked = await IoCManager.Resolve<IAchievementFetcher>().UpdateAchievements();
                AchievementNotification.QueueAchievementNotifications(newlyUnlocked);

                var achievementCounter = 0;
                var unlockedCounter = 0;
                foreach (var achievement in DbManager.DataAccess.Achievements().GetAchievements())
                {
                    Achievements.Add(AchievementViewModel.CreateFrom(achievement));

                    if (achievement.IsUnlocked)
                    {
                        unlockedCounter++;
                    }

                    achievementCounter++;
                }

                Score = $"{Strings.AchievementsScreenView_Score} {AppSharedData.CurrentAchievementsScore()}";
                AchievementCount = $"{unlockedCounter + "/" + achievementCounter}{Strings.AchievementsScreenView_Achievement_Count}";
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                await IoCManager.Resolve<INavigationService>()
                                .DisplayAlert(Strings.Alert_No_Internet_Title,
                                              Strings.Alert_No_Internet_Description,
                                              Strings.Alert_Confirm);
            }
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

        private string achievementCount;

        public string AchievementCount
        {
            get => achievementCount;
            set => SetProperty(ref achievementCount, value);
        }
    }
}