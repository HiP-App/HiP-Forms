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

using System.Collections.ObjectModel;

using HipMobileUI.Map;
using System.Linq;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using HipMobileUI.ViewModels.Views;
using HipMobileUI.Views;
using Xamarin.Forms;

namespace HipMobileUI.ViewModels.Pages
{
    public class MainPageViewModel : NavigationViewModel
    {

        public MainPageViewModel()
        {
            MainScreenViewModels = new ObservableCollection<NavigationViewModel>();

            var routesOverviewViewModel = new RoutesOverviewViewModel
            {
                Title = "Routen"
            };

            var vm = new DummyViewModel ()

            {
                Title = "Blue",
                Color = Color.Blue
            };
            var vm1 = new DummyViewModel()
            {
                Title = "Red",
                Color = Color.Red
            };
            var vm2 = new DummyViewModel()
            {
                Title = "Green",
                Color = Color.Green
            };


            MainScreenViewModels.Add (vm);
            MainScreenViewModels.Add(routesOverviewViewModel);
            MainScreenViewModels.Add(vm);
            MainScreenViewModels.Add(vm1);
            MainScreenViewModels.Add(vm2);
            MainScreenViewModels.Add (new MapViewModel ());

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
            set { SetProperty(ref (selectedViewModel), value); }
        }

    }
}