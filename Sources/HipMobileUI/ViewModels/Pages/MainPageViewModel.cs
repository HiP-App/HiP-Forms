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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.FeatureToggling;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Appearance;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages
{
    public class MainPageViewModel : NavigationViewModel
    {
        private Command logoutCommand;
        private readonly MenuConfiguration menuConfiguration;

        private readonly ProfileScreenViewModel profileScreenViewModel;
        private readonly LoginScreenViewModel loginScreenViewModel;
        private readonly RegisterScreenViewModel registerScreenViewModel;
        private readonly ForgotPasswordScreenViewModel forgotPasswordScreenViewModel;
        private readonly AchievementsScreenViewModel achievementsScreenViewModel;

        private readonly IDisposable achievementsFeatureSubscription;

        public Command LogoutCommand
        {
            get => logoutCommand;
            set => SetProperty(ref logoutCommand, value);
        }

        private async void LogoutAsync()
        {
            var result = await Navigation.DisplayAlert(Strings.ProfileScreenViewModel_Dialog_Logout_Title
                                                       , Strings.ProfileScreenViewModel_Dialog_Logout_Message
                                                       , Strings.Yes, Strings.No);
            if (!result)
                return;
            Settings.IsLoggedIn = false;
            UpdateAccountViews();
        }

        public MainPageViewModel() : this(DbManager.DataAccess.Exhibits().GetExhibits().ToList())
        {
            LogoutCommand = new Command(LogoutAsync);
        }

        private MainPageViewModel(IReadOnlyList<Exhibit> exhibits)
        {
            menuConfiguration = new MenuConfiguration(this, exhibits);
            UpdateMenuConfiguration();

            profileScreenViewModel = MainScreenViewModels.OfType<ProfileScreenViewModel>().SingleOrDefault();
            loginScreenViewModel = menuConfiguration.GetLoginScreenViewModel();
            registerScreenViewModel = menuConfiguration.GetRegisterScreenViewModel();
            forgotPasswordScreenViewModel = menuConfiguration.GetForgotPasswordScreenViewModel();
            achievementsScreenViewModel = menuConfiguration.GetAchievementsScreenViewModel();

            Settings.ChangeEvents.PropertyChanged += LoginChangedHandler;
            UpdateUserLogginInfo();

            achievementsFeatureSubscription = IoCManager.Resolve<IFeatureToggleRouter>()
                                                        .IsFeatureEnabled(FeatureId.Achievements)
                                                        .Subscribe(new AchievementsVmHider(MainScreenViewModels.OfType<AchievementsScreenViewModel>().FirstOrDefault(),
                                                                                           MainScreenViewModels));

            UpdateAccountViews();
        }

        private class AchievementsVmHider : IObserver<bool>
        {
            private readonly AchievementsScreenViewModel vm;
            private readonly ObservableCollection<NavigationViewModel> vms;

            public AchievementsVmHider(AchievementsScreenViewModel vm, ObservableCollection<NavigationViewModel> vms)
            {
                this.vm = vm;
                this.vms = vms;
            }

            public void OnCompleted()
            {
            }

            public void OnError(Exception error)
            {
            }

            public void OnNext(bool enabled)
            {
                if (enabled && !vms.Contains(vm))
                {
                    vms.Add(vm);
                }
                else if (!enabled)
                {
                    vms.Remove(vm);
                }
            }
        }

        /// <summary>
        /// Used to control the displayed menu items in the main menu. Initializes all
        /// NavigationViewModels and grants access to predefined collections for the exploration modes.
        /// </summary>
        private class MenuConfiguration
        {
            private readonly ExhibitsOverviewViewModel exhibitsOverviewViewModel;
            private readonly RoutesOverviewViewModel routesOverviewViewModel;
            private readonly LoginScreenViewModel loginScreenViewModel;
            private readonly ForgotPasswordScreenViewModel forgotPasswordScreenViewModel;
            private readonly RegisterScreenViewModel registerScreenViewModel;
            private readonly ProfileScreenViewModel profileScreenViewModel;
            private readonly AchievementsScreenViewModel achievementsScreenViewModel;
            private readonly SettingsScreenViewModel settingsScreenViewModel;
            private readonly LicenseScreenViewModel licenseScreenViewModel;

            public MenuConfiguration(MainPageViewModel mainPageViewModel, IReadOnlyList<Exhibit> exhibitSet)
            {
                exhibitsOverviewViewModel = new ExhibitsOverviewViewModel(exhibitSet)
                {
                    Title = Strings.MainPageViewModel_OverviewPage,
                    Icon = "ic_home.png"
                };
                routesOverviewViewModel = new RoutesOverviewViewModel
                {
                    Title = Strings.MainPageViewModel_Routes,
                    Icon = "ic_directions.png"
                };
                loginScreenViewModel = new LoginScreenViewModel(mainPageViewModel)
                {
                    Title = Strings.MainPageViewModel_Account,
                    Icon = "ic_account_circle.png"
                };
                forgotPasswordScreenViewModel = new ForgotPasswordScreenViewModel(mainPageViewModel)
                {
                    Title = Strings.MainPageViewModel_Account,
                    Icon = "ic_account_circle.png"
                };
                registerScreenViewModel = new RegisterScreenViewModel(mainPageViewModel)
                {
                    Title = Strings.MainPageViewModel_Account,
                    Icon = "ic_account_circle.png"
                };
                profileScreenViewModel = new ProfileScreenViewModel(mainPageViewModel)
                {
                    Title = Strings.MainPageViewModel_Profile,
                    Icon = "ic_account_circle.png"
                };
                achievementsScreenViewModel = new AchievementsScreenViewModel
                {
                    Title = Strings.MainPageViewModel_Achievements,
                    Icon = "ic_equalizer.png"
                };
                settingsScreenViewModel = new SettingsScreenViewModel
                {
                    Title = Strings.MainPageViewModel_Settings,
                    Icon = "ic_settings.png"
                };
                licenseScreenViewModel = new LicenseScreenViewModel
                {
                    Title = Strings.MainPageViewModel_LegalNotices,
                    Icon = "ic_gavel.png"
                };
            }

            /// <summary>
            /// Creates and returns the collection of menu items for the adventurer mode
            /// </summary>
            public ObservableCollection<NavigationViewModel> AdventurerCollection()
            {
                return new ObservableCollection<NavigationViewModel>
                {
                    exhibitsOverviewViewModel,
                    profileScreenViewModel,
                    achievementsScreenViewModel,
                    settingsScreenViewModel,
                    licenseScreenViewModel
                };
            }

            /// <summary>
            /// Creates and returns the collection of menu items for the professor mode
            /// </summary>
            public ObservableCollection<NavigationViewModel> ProfessorCollection()
            {
                return new ObservableCollection<NavigationViewModel>
                {
                    exhibitsOverviewViewModel,
                    routesOverviewViewModel,
                    profileScreenViewModel,
                    achievementsScreenViewModel,
                    settingsScreenViewModel,
                    licenseScreenViewModel
                };
            }

            public LoginScreenViewModel GetLoginScreenViewModel()
            {
                return loginScreenViewModel;
            }

            public RegisterScreenViewModel GetRegisterScreenViewModel()
            {
                return registerScreenViewModel;
            }

            public ForgotPasswordScreenViewModel GetForgotPasswordScreenViewModel()
            {
                return forgotPasswordScreenViewModel;
            }

            public AchievementsScreenViewModel GetAchievementsScreenViewModel()
            {
                return achievementsScreenViewModel;
            }
        }

        private void UpdateMenuConfiguration()
        {
            MainScreenViewModels = Settings.AdventurerMode ? menuConfiguration.AdventurerCollection() : menuConfiguration.ProfessorCollection();
        }

        private void LoginChangedHandler(object o, PropertyChangedEventArgs args)
        {
            UpdateUserLogginInfo();
        }
               
        private void UpdateUserLogginInfo()
        {
            EmailDisplayed = Settings.IsLoggedIn;
            Email = Settings.IsLoggedIn ? Settings.EMail : "";
        }


        /// <summary>
        /// Call this method after a change to 'Settings.IsLoggedIn' to display the correct view.
        /// </summary>
        public void UpdateAccountViews()
        {
            if (Settings.IsLoggedIn)
                SwitchToProfileView();
            else
                SwitchToLoginView();
        }

        public void SwitchToAchievementsView()
        {
            if (MainScreenViewModels.Contains(achievementsScreenViewModel))
                MainScreenViewModels[MainScreenViewModels.IndexOf(achievementsScreenViewModel)] = achievementsScreenViewModel;
            SelectedViewModel = achievementsScreenViewModel;
        }

        public void SwitchToProfileView()
        {
            if (MainScreenViewModels.Contains(loginScreenViewModel))
                MainScreenViewModels[MainScreenViewModels.IndexOf(loginScreenViewModel)] = profileScreenViewModel;
            SelectedViewModel = profileScreenViewModel;
        }

        public void SwitchToLoginView()
        {
            if (MainScreenViewModels.Contains(profileScreenViewModel))
                MainScreenViewModels[MainScreenViewModels.IndexOf(profileScreenViewModel)] = loginScreenViewModel;
            SelectedViewModel = loginScreenViewModel;
        }

        public void SwitchToRegisterView()
        {
            SelectedViewModel = registerScreenViewModel;
        }

        public void SwitchToForgotPasswordView()
        {
            SelectedViewModel = forgotPasswordScreenViewModel;
        }

        public void SwitchToSettingsScreenView()
        {
            SelectedViewModel = MainScreenViewModels.OfType<SettingsScreenViewModel>().SingleOrDefault();

            IoCManager.Resolve<IThemeManager>().AdjustTheme();
        }

        private ObservableCollection<NavigationViewModel> mainScreenViewModels;

        public ObservableCollection<NavigationViewModel> MainScreenViewModels
        {
            get => mainScreenViewModels;
            set => SetProperty(ref mainScreenViewModels, value);
        }

        private NavigationViewModel selectedViewModel;

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

        private bool userNameDisplayed;
        public bool UserNameDisplayed
        {
            get => userNameDisplayed;
            set => SetProperty(ref userNameDisplayed, value);
        }
        
        private bool emailDisplayed;
        public bool EmailDisplayed
        {
            get => emailDisplayed;
            set => SetProperty(ref emailDisplayed, value);
        }

        private string userName;
        public string UserName
        {
            get => userName;
            set => SetProperty(ref userName, value);
        }
        
        private string email;
        public string Email
        {
            get => email;
            set => SetProperty(ref email, value);
        }
        
        public override void OnDisappearing()
        {
            base.OnDisappearing();

            SelectedViewModel.OnDisappearing();
            achievementsFeatureSubscription?.Dispose();
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