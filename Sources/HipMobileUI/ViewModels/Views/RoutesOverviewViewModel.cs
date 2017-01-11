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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using HipMobileUI.Helpers;

namespace HipMobileUI.ViewModels.Views {
    public class RoutesOverviewViewModel : NavigationViewModel {

        private ObservableCollection<RoutesOverviewListItemViewModel> routes;

        public ObservableCollection<RoutesOverviewListItemViewModel> Routes
        {
            get { return routes;}
            set { SetProperty (ref routes, value); }
        }

        public RoutesOverviewViewModel ()
        {
            Routes = new ObservableCollection<RoutesOverviewListItemViewModel> ();
            foreach (Route route in RouteManager.GetRoutes ())
            {
                Routes.Add (new RoutesOverviewListItemViewModel (route.Id));
            }

        }


    }
}