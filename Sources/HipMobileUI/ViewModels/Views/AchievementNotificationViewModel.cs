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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmHelpers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views
{
    public class AchievementNotificationViewModel : BaseViewModel
    {
        private static IList<IAchievement> recentlyUnlockedAchievements;
        private CancellationTokenSource cancellationTokenSource;
        private bool achievementNotificationDisplayed;
        private bool notificationsActive;

        public AchievementNotificationViewModel()
        {
            IsVisible = false;
            achievementNotificationDisplayed = false;
            notificationsActive = true;
            ResetCancellationToken();
            recentlyUnlockedAchievements = new List<IAchievement>();
            DisposeNotificationCommand = new Command(DisposeAchievementNotification);
        }

        // Temporary method for testing
        public void CreateAndDisplayDummyNotifications()
        {
            var collection = new ObservableCollection<IAchievement> {
                new ExhibitsVisitedAchievement {
                    Title = "The exhibit visitor",
                    Description = "Visit an exhibit for the first time",
                    ImageUrl = "https://docker-hip.cs.upb.de/public/thumbnailservice/api/Thumbnails?Url=achievements/api/image/0/"},
                new ExhibitsVisitedAchievement{
                    Title = "The route completer",
                    Description = "Complete a route for the first time",
                    ImageUrl = "https://docker-hip.cs.upb.de/public/thumbnailservice/api/Thumbnails?Url=achievements/api/image/1/"} };
            QueueAchievementNotifications(collection);
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
            if (recentlyUnlockedAchievements.Count == 0)
                return;

            while (notificationsActive)
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

        public void DisableNotifications()
        {
            DisposeAchievementNotification();
            notificationsActive = false;
        }

        public void EnableNotifications()
        {
            notificationsActive = true;
            DisplayAchievementNotification();
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
