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
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using HipMobileUI.Resources;
using HipMobileUI.ViewModels.Views;
using Xamarin.Forms;

namespace HipMobileUI.ViewModels.Pages
{
    public class MainPageViewModel : NavigationViewModel
    {

        public MainPageViewModel()
        {
            ExhibitSet exhibitSet = ExhibitManager.GetExhibitSets ().FirstOrDefault ();
            MainScreenViewModels = new ObservableCollection<NavigationViewModel>
            {
                new ExhibitsOverviewViewModel (exhibitSet)
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
                new DummyViewModel
                {
                    Title = Strings.MainPageViewModel_Feedback,
                    Icon = "ic_feedback.png",
                    Color = Color.Green
                },
                new LicenseScreenViewModel
                {
                    Title = Strings.MainPageViewModel_LegalNotices,
                    Icon = "ic_gavel.png"
                }
            };
        }

        private ObservableCollection<NavigationViewModel> mainScreenViewModels;
        private NavigationViewModel selectedViewModel;

        public ObservableCollection<NavigationViewModel> MainScreenViewModels
        {
            get { return mainScreenViewModels; }
            set { SetProperty(ref mainScreenViewModels, value); }
        }

        public NavigationViewModel SelectedViewModel
        {
            get { return selectedViewModel; }
            set {
                var oldViewModel = SelectedViewModel;
                if (SetProperty (ref (selectedViewModel), value))
                {
                    oldViewModel?.OnDisappearing ();
                }
            }
        }

    }
}