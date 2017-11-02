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

        /// <summary>
        /// Adds newly unlocked achievements to the list of achievements notifications are displayed for.
        /// </summary>
        /// <param name="achievements">Collection of recently unlocked achievements.</param>
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

        /// <summary>
        /// Displays all queued achievements in succession.
        /// </summary>
        private async void DisplayAchievementNotification()
        {
            while (true)
            {
                var achievement = recentlyUnlockedAchievements.First();

                achievementNotificationDisplayed = true;
                await UpdateDisplayedData(achievement);

                IsVisible = true;
                Opacity = 0;

                await Animate();

                ResetCancellationToken();
                IsVisible = false;
                recentlyUnlockedAchievements.Remove(achievement);
                achievementNotificationDisplayed = false;

                if (recentlyUnlockedAchievements.Count != 0)
                    continue;
                break;
            }
        }

        /// <summary>
        /// Animates the notification.
        /// </summary>
        private async Task Animate()
        {
            await FadeTo(0, 1, 0.1, 25);
            await Task.Delay(3000, cancellationTokenSource.Token).ContinueWith(task => { });
            await FadeTo(1, 0, -0.1, 25);
        }

        /// <summary>
        /// Updates the data displayed by the notification.
        /// </summary>
        /// <param name="achievement">Achievement to be displayed.</param>
        /// <returns></returns>
        private async Task UpdateDisplayedData(IAchievement achievement)
        {
            AchievementTitle = achievement.Title;
            AchievementDescription = achievement.Description;
            var stream = await achievement.LoadImage();
            AchievementImage = ImageSource.FromStream(() => stream);
        }

        /// <summary>
        /// Disposes of the current notification.
        /// </summary>
        private void DisposeAchievementNotification()
        {
            if (achievementNotificationDisplayed)
                cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Removes all notification from the queue.
        /// </summary>
        public void RemoveAllAchievementNotifications()
        {
            DisposeAchievementNotification();
            recentlyUnlockedAchievements.Clear();
        }

        /// <summary>
        /// Helper method to realize the fade-animation.
        /// </summary>
        /// <param name="from">Start-Opacity [0 .. 1]</param>
        /// <param name="to">Target-Opacity [0 .. 1]</param>
        /// <param name="increment">Step size of opacity change.</param>
        /// <param name="delay">Time in ms between the steps.</param>
        /// <returns></returns>
        private async Task FadeTo(double from, double to, double increment, int delay)
        {
            for (var x = from; Math.Abs(x - to) > Math.Abs(increment); x = x + increment)
            {
                Opacity = x;
                await Task.Delay(delay);
            }
            Opacity = to;
        }

        /// <summary>
        /// Resets the cancellationToken so it can be reused.
        /// </summary>
        private void ResetCancellationToken()
        {
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = new CancellationTokenSource();
        }

        private ICommand disposeNotificationCommand;
        public ICommand DisposeNotificationCommand
        {
            get { return disposeNotificationCommand; }
            set { SetProperty(ref disposeNotificationCommand, value); }
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
