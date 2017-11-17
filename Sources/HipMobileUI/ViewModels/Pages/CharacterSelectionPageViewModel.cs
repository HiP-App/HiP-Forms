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

using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Contracts;
using System.Windows.Input;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Appearance;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages
{
    public class CharacterSelectionPageViewModel : NavigationViewModel
    {
        private readonly NavigationViewModel parentViewModel;

        public CharacterSelectionPageViewModel(NavigationViewModel parentViewModel)
        {
            this.parentViewModel = parentViewModel;

            AdventurerGridTappedCommand = new Command(OnAdventurerGridTapped);
            ProfessorGridTappedCommand = new Command(OnProfessorGridTapped);
        }

        public ICommand AdventurerGridTappedCommand { get; }
        public ICommand ProfessorGridTappedCommand { get; }

        private void OnAdventurerGridTapped()
        {
            Settings.AdventurerMode = true;
            AdjustThemeAndContinue();
        }

        private void OnProfessorGridTapped()
        {
            Settings.AdventurerMode = false;
            AdjustThemeAndContinue();
        }

        private void AdjustThemeAndContinue()
        {
            SwitchToNextPage();

            // Make sure all related components are already initialized before adjusting theme
            if (Settings.InitialThemeSelected)
                IoCManager.Resolve<IThemeManager>().AdjustTopBarTheme();
        }

        /// <summary>
        /// Switches to the next page after a character has been selected. If the parent view is the Settings- or ProfileScreenView, the next page is the previous page.
        /// </summary>
        public void SwitchToNextPage()
        {
            var statusBarController = IoCManager.Resolve<IStatusBarController>();
            statusBarController.ShowStatusBar();

            if (parentViewModel.GetType() == typeof(UserOnboardingPageViewModel))
            {
                Navigation.StartNewNavigationStack(new LoadingPageViewModel());
            }
            else if (parentViewModel.GetType() == typeof(ProfileScreenViewModel))
            {
                var mainPageViewModel = new MainPageViewModel();
                Navigation.StartNewNavigationStack(mainPageViewModel);
                mainPageViewModel.SwitchToProfileView();
            }
            else if (parentViewModel.GetType() == typeof(SettingsScreenViewModel))
            {
                var mainPageViewModel = new MainPageViewModel();
                Navigation.StartNewNavigationStack(mainPageViewModel);
                mainPageViewModel.SwitchToSettingsScreenView();
            }
        }

        public NavigationViewModel ParentViewModel => parentViewModel;
    }
}