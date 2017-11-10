using System.Collections.Generic;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages
{
    public class ExtendedNavigationViewModel : NavigationViewModel
    {
        protected ExtendedNavigationViewModel()
        {
            AchievementNotificationViewModel = new AchievementNotificationViewModel();
        }
        /// <summary>
        /// Queues a notification for a recently unlocked achievement. Will be displayed on the current page and requires it to have an AchievementNotificationView.
        /// </summary>
        /// <param name="achievements">A collection of achievements to be displayed.</param>
        protected void QueueAchievementNotification(IEnumerable<IAchievement> achievements)
        {
            AchievementNotificationViewModel.QueueAchievementNotifications(achievements);
        }

        private AchievementNotificationViewModel achievementNotificationViewModel;
        public AchievementNotificationViewModel AchievementNotificationViewModel
        {
            get { return achievementNotificationViewModel; }
            set { SetProperty(ref achievementNotificationViewModel, value); }
        }

        public override void OnAppearing()
        {
            base.OnAppearing();

            AchievementNotificationViewModel.EnableNotifications();
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();

            AchievementNotificationViewModel.DisableNotifications();
        }

        public override void OnRevealed()
        {
            base.OnRevealed();

            AchievementNotificationViewModel.EnableNotifications();
        }

        public override void OnHidden()
        {
            base.OnHidden();

            AchievementNotificationViewModel.DisableNotifications();
        }
    }
}