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
using HipMobileUI.ViewModels.Views;
using Xamarin.Forms;

namespace HipMobileUI.ViewModels.Pages
{
    public class MainPageViewModel : NavigationViewModel
    {

        public MainPageViewModel()
        {
            MainScreenViewModels = new ObservableCollection<NavigationViewModel>
            {
                new DummyViewModel
                {
                    Title = "Übersicht",
                    Icon = "ic_home.png",
                    Color = Color.Gray
                },
                new RoutesOverviewViewModel
                {
                    Title = "Routen",
                    Icon = "ic_directions.png"
                },
                new DummyViewModel
                {
                    Title = "Einstellungen",
                    Icon = "ic_settings.png",
                    Color = Color.Red
                },
                new DummyViewModel
                {
                    Title = "Feedback",
                    Icon = "ic_feedback.png",
                    Color = Color.Green
                },
                new LicenseScreenViewModel
                {
                    Title = "Rechtliche Hinweise",
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
            set { SetProperty(ref selectedViewModel, value); }
        }

    }
}