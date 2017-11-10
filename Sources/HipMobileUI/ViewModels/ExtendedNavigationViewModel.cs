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

using System.Collections.Generic;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels
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