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

using System.Collections.ObjectModel;
using System.Linq;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages
{
    public class MainPageViewModel : NavigationViewModel
    {
        public MainPageViewModel(ExhibitSet set)
        {
            profileScreenViewModel = new ProfileScreenViewModel(this)
            {
                Title = Strings.MainPageViewModel_Profile,
                Icon = "ic_account_circle.png"
            };
            loginScreenViewModel = new LoginScreenViewModel(this)
            {
                Title = Strings.MainPageViewModel_Account,
                Icon = "ic_account_circle.png"
            };
            forgotPasswordScreenViewModel = new ForgotPasswordScreenViewModel(this)
            {
                Title = Strings.MainPageViewModel_Account,
                Icon = "ic_account_circle.png"
            };

            registerScreenViewModel = new RegisterScreenViewModel(this)
            {
                Title = Strings.MainPageViewModel_Account,
                Icon = "ic_account_circle.png"
            };

            MainScreenViewModels = new ObservableCollection<NavigationViewModel>
            {
                new ExhibitsOverviewViewModel(set)
                {
                    Title = Strings.MainPageViewModel_OverviewPage,
                    Icon = "ic_home.png"
                },
                new RoutesOverviewViewModel
                {
                    Title = Strings.MainPageViewModel_Routes,
                    Icon = "ic_directions.png"
                },
                new SettingsScreenViewModel
                {
                    Title = Strings.MainPageViewModel_Settings,
                    Icon = "ic_settings.png"
                },
                new LicenseScreenViewModel
                {
                    Title = Strings.MainPageViewModel_LegalNotices,
                    Icon = "ic_gavel.png"
                },
                profileScreenViewModel,
                new AchievementsScreenViewModel
                {
                    Title = Strings.MainPageViewModel_Achievements,
                    Icon = "ic_equalizer.png"
                }
            };
            UpdateAccountViews();
        }

        /** Call this method after a change to 'Settings.IsLoggedIn' to display the correct view. */
        public void UpdateAccountViews()
        {
            if (Settings.IsLoggedIn)
                SwitchToProfileView();
            else
                SwitchToLoginView();
        }

        public void SwitchToLoginView()
        {
            mainScreenViewModels.RemoveAt(4);
            mainScreenViewModels.Insert(4, loginScreenViewModel);
            SelectedViewModel = mainScreenViewModels[4];
        }

        public void SwitchToForgotPasswordView()
        {
            mainScreenViewModels.RemoveAt(4);
            mainScreenViewModels.Insert(4, forgotPasswordScreenViewModel);
            SelectedViewModel = mainScreenViewModels[4];
        }

        public void SwitchToProfileView()
        {
            mainScreenViewModels.RemoveAt(4);
            mainScreenViewModels.Insert(4, profileScreenViewModel);
            SelectedViewModel = mainScreenViewModels[4];
        }

        public void SwitchToRegisterView()
        {
            mainScreenViewModels.RemoveAt(4);
            mainScreenViewModels.Insert(4, registerScreenViewModel);
            SelectedViewModel = mainScreenViewModels[4];
        }

        public void SwitchToSettingsScreenView()
        {
            SelectedViewModel = mainScreenViewModels[2];
        }

        public MainPageViewModel() : this(ExhibitManager.GetExhibitSets().FirstOrDefault())
        {
        }

        private ObservableCollection<NavigationViewModel> mainScreenViewModels;
        private readonly ProfileScreenViewModel profileScreenViewModel;
        private readonly LoginScreenViewModel loginScreenViewModel;
        private readonly ForgotPasswordScreenViewModel forgotPasswordScreenViewModel;
        private readonly RegisterScreenViewModel registerScreenViewModel;

        private NavigationViewModel selectedViewModel;

        public ObservableCollection<NavigationViewModel> MainScreenViewModels
        {
            get => mainScreenViewModels;
            set => SetProperty(ref mainScreenViewModels, value);
        }

        public NavigationViewModel SelectedViewModel
        {
            get => selectedViewModel;
            set

            {
                var oldViewModel = SelectedViewModel;
                if (SetProperty(ref (selectedViewModel), value))
                {
                    oldViewModel?.OnDisappearing();
                    SelectedViewModel?.OnAppearing();
                }
            }
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();

            SelectedViewModel.OnDisappearing();
        }

        public override void OnAppearing()
        {
            base.OnAppearing();

            SelectedViewModel.OnAppearing();
        }

        public override void OnHidden()
        {
            base.OnHidden();

            SelectedViewModel.OnHidden();
        }

        public override void OnRevealed()
        {
            base.OnRevealed();

            SelectedViewModel.OnRevealed();
        }
    }
}