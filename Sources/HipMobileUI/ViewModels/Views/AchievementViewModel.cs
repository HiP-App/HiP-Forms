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
using System.Windows.Input;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views
{
    public class AchievementViewModel
    {
        public IAchievement Achievement { get; }
        public ImageSource Image { get; }
        public ICommand ItemTappedCommand { get; }

        private AchievementViewModel(IAchievement achievement, ImageSource image)
        {
            ItemTappedCommand = new Command(x => NavigateToAchievementDetails());
            Achievement = achievement;
            Image = image;
        }

        private async void NavigateToAchievementDetails()
        {
            NavigationViewModel viewModel;
            switch (Achievement)
            {
                case ExhibitsVisitedAchievement e:
                    viewModel = new AchievementsDetailsExhibitViewModel(e);
                    break;
                case RouteFinishedAchievement r:
                    viewModel = new AchievementsDetailsRouteViewModel(r);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Unknown achievement type.");
            }
            await IoCManager.Resolve<INavigationService>().PushAsync(viewModel);
        }

        public static AchievementViewModel CreateFrom(IAchievement achievement)
        {
            var src = new StreamImageSource
            {
                Stream = token => achievement.LoadImage()
            };
            return new AchievementViewModel(achievement, src);
        }
    }
}