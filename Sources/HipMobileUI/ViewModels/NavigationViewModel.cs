// Copyright (C) 2016 History in Paderborn App - Universität Paderborn
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
using MvvmHelpers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels
{
    public abstract class NavigationViewModel : BaseViewModel
    {
        protected INavigationService Navigation = IoCManager.Resolve<INavigationService>();

        /// <summary>
        /// Method called when the view disappears. Note that this method is not called automatically for every view.
        /// </summary>
        public virtual void OnDisappearing()
        {
        }

        /// <summary>
        /// Method called when the view appears for the first time. Note that this method is not called automatically for every view.
        /// </summary>
        public virtual void OnAppearing()
        {
        }

        /// <summary>
        /// Method called when the view is hidden, for example another page is pushed on top of the view.
        /// </summary>
        public virtual void OnHidden()
        {
        }

        /// <summary>
        /// Method called when the view is visible again, after it was hidden.
        /// </summary>
        public virtual void OnRevealed()
        {
        }

        /// <summary>
        /// Queues a notification for a recently unlocked achievement. Will be displayed on the current page and requires it to have an AchievementNotificationView.
        /// </summary>
        /// <param name="achievementNotificationViewModel">The viewmodel responsible for the notifications. Used as BindingContext on the current page.</param>
        /// <param name="achievements">A collection of achievements to be displayed.</param>
        protected void QueueAchievementNotification(AchievementNotificationViewModel achievementNotificationViewModel, IEnumerable<IAchievement> achievements)
        {
            achievementNotificationViewModel.QueueAchievementNotifications(achievements);
        }
    }
}