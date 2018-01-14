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
using System.Linq;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.FeatureToggling;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages
{
    public class MainPageViewModel : NavigationViewModel
    {
        private ObservableCollection<NavigationViewModel> mainScreenViewModels;
        private readonly ProfileScreenViewModel profileScreenViewModel;
        private readonly LoginScreenViewModel loginScreenViewModel;
        private readonly ForgotPasswordScreenViewModel forgotPasswordScreenViewModel;
        private readonly RegisterScreenViewModel registerScreenViewModel;

        private NavigationViewModel selectedViewModel;
        private IDisposable achievementsFeatureSubscription;

        public MainPageViewModel() : this(ExhibitManager.GetExhibitSets().FirstOrDefault())
        {
        }

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

            var achievementsScreenViewModel = new AchievementsScreenViewModel
            {
                Title = Strings.MainPageViewModel_Achievements,
                Icon = "ic_equalizer.png"
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
                achievementsScreenViewModel
            };
            achievementsFeatureSubscription = IoCManager.Resolve<IFeatureToggleRouter>()
                      .IsFeatureEnabled(FeatureId.Achievements)
                      .Subscribe(new AchievementsVmHider(achievementsScreenViewModel, mainScreenViewModels));
            UpdateAccountViews();
        }
        
        private class AchievementsVmHider: IObserver<bool>
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
                } else if (!enabled)
                {
                    vms.Remove(vm);
                }
            }
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

        public void SwitchToLoginView()
        {
            ChangeAccountRelatedView(loginScreenViewModel);
        }

        public void SwitchToForgotPasswordView()
        {
            ChangeAccountRelatedView(forgotPasswordScreenViewModel);
        }

        public void SwitchToProfileView()
        {
            ChangeAccountRelatedView(profileScreenViewModel);
        }

        public void SwitchToRegisterView()
        {
            ChangeAccountRelatedView(registerScreenViewModel);
        }

        private void ChangeAccountRelatedView(NavigationViewModel accountRelatedView)
        {
            mainScreenViewModels.RemoveAt(4);
            mainScreenViewModels.Insert(4, accountRelatedView);
            SelectedViewModel = mainScreenViewModels[4];
        }

        public void SwitchToSettingsScreenView()
        {
            SelectedViewModel = mainScreenViewModels[2];
        }

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