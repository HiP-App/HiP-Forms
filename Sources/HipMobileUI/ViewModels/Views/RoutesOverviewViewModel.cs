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
using System.Windows.Input;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using Xamarin.Forms;

namespace HipMobileUI.ViewModels.Views
{
    /// <summary>
    /// View model for the overview screen of all routes
    /// </summary>
    public class RoutesOverviewViewModel : NavigationViewModel
    {
        private ObservableCollection<RoutesOverviewListItemViewModel> routes;
        public ObservableCollection<RoutesOverviewListItemViewModel> Routes
        {
            get { return routes; }
            set { SetProperty(ref routes, value); }
        }

        /// <summary>
        /// Creates the view model and fills the route data with all routes using the <see cref="RouteManager"/>
        /// </summary>
        public RoutesOverviewViewModel()
        {
            Routes = new ObservableCollection<RoutesOverviewListItemViewModel>();
            foreach (Route route in RouteManager.GetRoutes())
            {
                Routes.Add(new RoutesOverviewListItemViewModel(route));
            }

            ItemSelectedCommand = new Command(x => NavigateToRoute(x as RoutesOverviewListItemViewModel));
        }

        /// <summary>
        /// Navigates to the route details of the select route
        /// </summary>
        /// <param name="selectedRouteItemViewModel"></param>
        private void NavigateToRoute(RoutesOverviewListItemViewModel selectedRouteItemViewModel)
        {
            if (selectedRouteItemViewModel == null)
            {
                return;
            }

            var route = selectedRouteItemViewModel.Route;

            Navigation.PushAsync (new RouteDetailsPageViewModel (route));
        }

        public ICommand ItemSelectedCommand { get; set; }

    }
}