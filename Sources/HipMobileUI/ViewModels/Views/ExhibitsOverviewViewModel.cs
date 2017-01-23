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

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using HipMobileUI.ViewModels.Pages;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Xamarin.Forms;

namespace HipMobileUI.ViewModels.Views
{
    class ExhibitsOverviewViewModel : NavigationViewModel
    {

        public ExhibitsOverviewViewModel (string exhibitSetId)
        {
            Title = "Sehenswürdigkeiten";
            ExhibitSet set = ExhibitManager.GetExhibitSet (exhibitSetId);
            if (set != null)
            {
                ExhibitsList = new ObservableCollection<ExhibitsOverviewListItemVIewModel> ();
                foreach (Exhibit exhibit in set)
                {
                    var listItem = new ExhibitsOverviewListItemVIewModel (exhibit);
                    ExhibitsList.Add (listItem);
                }
            }
            ItemTappedCommand = new Command (item => NavigateToExhibitDetails (item as ExhibitsOverviewListItemVIewModel));
            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 10;
            locator.PositionChanged+=LocatorOnPositionChanged;
            locator.StartListeningAsync (4000, 10);
           // SetDistances (locator.GetPositionAsync ().Result);
        }

        private void LocatorOnPositionChanged (object sender, PositionEventArgs positionEventArgs)
        {
            SetDistances (positionEventArgs.Position);
        }

        private void SetDistances (Position position)
        {
            foreach (var exhibit in ExhibitsList)
            {
                exhibit.UpdateDistance(position);
            }
        }

        public void NavigateToExhibitDetails (ExhibitsOverviewListItemVIewModel item)
        {
            if (item != null)
            {
                Navigation.PushAsync (new ExhibitDetailsViewModel (item.ExhibitId));
            }
        }

        private ObservableCollection<ExhibitsOverviewListItemVIewModel> exhibitsList;
        private ICommand itemTappedCommand;

        public ObservableCollection<ExhibitsOverviewListItemVIewModel> ExhibitsList {
            get { return exhibitsList; }
            set { SetProperty (ref exhibitsList, value); }
        }

        public ICommand ItemTappedCommand {
            get { return itemTappedCommand; }
            set { SetProperty (ref itemTappedCommand, value); }
        }

    }
}
