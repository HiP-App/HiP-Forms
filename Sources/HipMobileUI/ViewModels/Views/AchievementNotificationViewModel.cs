using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views
{
    public class AchievementNotificationViewModel : NavigationViewModel
    {
        private readonly IList<IAchievement> recentlyUnlockedAchievements;
        private CancellationTokenSource cancellationTokenSource;
        private bool achievementNotificationDisplayed;

        public AchievementNotificationViewModel()
        {
            IsVisible = true;
            achievementNotificationDisplayed = false;
            ResetCancellationToken();
            recentlyUnlockedAchievements = new List<IAchievement>();
            DisposeNotificationCommand = new Command(DisposeAchievementNotification);
        }

        private ICommand disposeNotificationCommand;
        public ICommand DisposeNotificationCommand
        {
            get { return disposeNotificationCommand; }
            set { SetProperty(ref disposeNotificationCommand, value); }
        }

        public void QueueAchievementNotifications(IEnumerable<IAchievement> achievements)
        {
            foreach (var achievement in achievements)
            {
                recentlyUnlockedAchievements.Add(achievement);
            }

            if (!achievementNotificationDisplayed)
            {
                DisplayAchievementNotification();
            }
        }

        private async void DisplayAchievementNotification()
        {
            var achievement = recentlyUnlockedAchievements.First();

            achievementNotificationDisplayed = true;
            await UpdateDisplayedData(achievement);

            IsVisible = true;
            Opacity = 0;

            await FadeTo(0, 1, 0.1, 25);
            await Task.Delay(3000, cancellationTokenSource.Token).ContinueWith(task => { });
            await FadeTo(1, 0, -0.1, 25);

            ResetCancellationToken();
            IsVisible = false;
            recentlyUnlockedAchievements.Remove(achievement);
            achievementNotificationDisplayed = false;

            if (recentlyUnlockedAchievements.Count != 0)
                DisplayAchievementNotification();
        }

        private void DisposeAchievementNotification()
        {
            if (achievementNotificationDisplayed)
                cancellationTokenSource.Cancel();
        }

        private async Task UpdateDisplayedData(IAchievement achievement)
        {
            AchievementTitle = achievement.Title;
            AchievementDescription = achievement.Description;
            AchievementImage = ImageSource.FromFile("ic_account_circle.png"); // only temp
            //var stream = await achievement.LoadImage();
            //AchievementImage = ImageSource.FromStream(() => stream);
        }

        private async Task FadeTo(double from, double to, double increment, int delay)
        {
            for (var x = from; Math.Abs(x - to) > Math.Abs(increment); x = x + increment)
            {
                Opacity = x;
                await Task.Delay(delay);
            }
            Opacity = to;
        }

        private void ResetCancellationToken()
        {
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = new CancellationTokenSource();
        }

        private string achievementTitle;
        public string AchievementTitle
        {
            get { return achievementTitle; }
            set { SetProperty(ref achievementTitle, value); }
        }

        private string achievementDescription;
        public string AchievementDescription
        {
            get { return achievementDescription; }
            set { SetProperty(ref achievementDescription, value); }
        }

        private ImageSource achievementImage;
        public ImageSource AchievementImage
        {
            get { return achievementImage; }
            set { SetProperty(ref achievementImage, value); }
        }

        private bool isVisible;
        public bool IsVisible
        {
            get { return isVisible; }
            set { SetProperty(ref isVisible, value); }
        }

        private double opacity;
        public double Opacity
        {
            get { return opacity; }
            set { SetProperty(ref opacity, value); }
        }
    }
}
