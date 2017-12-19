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

using System.Threading;
using System.Threading.Tasks;
using MvvmHelpers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels
{
    public abstract class NavigationViewModel : BaseViewModel
    {
        protected readonly INavigationService Navigation = IoCManager.Resolve<INavigationService>();
        protected readonly AchievementNotificationViewModel AchievementNotification = IoCManager.Resolve<AchievementNotificationViewModel>();

        /// <summary>
        /// Method called when the view disappears. Note that this method is not called automatically for every view.
        /// </summary>
        public virtual void OnDisappearing()
        {
            StopAutoCheckForAchievements();
        }

        /// <summary>
        /// Method called when the view appears for the first time. Note that this method is not called automatically for every view.
        /// </summary>
        public virtual void OnAppearing()
        {
            AchievementNotification.ReloadDisplayedData();
            StartAutoCheckForAchievements();
        }

        /// <summary>
        /// Method called when the view is hidden, for example another page is pushed on top of the view.
        /// </summary>
        public virtual void OnHidden()
        {
            StopAutoCheckForAchievements();
        }

        /// <summary>
        /// Method called when the view is visible again, after it was hidden.
        /// </summary>
        public virtual void OnRevealed()
        {
            AchievementNotification.ReloadDisplayedData();
            StartAutoCheckForAchievements();
        }

        private int isAutoChecking = 0;
        
        private void StartAutoCheckForAchievements()
        {
            if (Interlocked.Exchange(ref isAutoChecking, 1) == 1)
            {
                return;
            }

            async Task DequeueAndRepeat()
            {
                var pending = AchievementManager.DequeuePendingAchievementNotifications();
                AchievementNotification.QueueAchievementNotifications(pending);
                await Task.Delay(5000);
                if (isAutoChecking == 1)
                {
                    Device.BeginInvokeOnMainThread(async () => await DequeueAndRepeat());
                }
            }
            Device.BeginInvokeOnMainThread(async () => await DequeueAndRepeat());
        }

        private void StopAutoCheckForAchievements()
        {
            Interlocked.Exchange(ref isAutoChecking, 0);
        }
    }
}