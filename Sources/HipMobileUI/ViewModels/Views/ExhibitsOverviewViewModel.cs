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
using HipMobileUI.ViewModels.Pages;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Xamarin.Forms;

namespace HipMobileUI.ViewModels.Views
{
    class ExhibitsOverviewViewModel : NavigationViewModel
    {
        private ObservableCollection<ExhibitsOverviewListItemViewModel> exhibitsList;
        private ICommand itemTappedCommand;
        private IGeolocator locator;

        public ExhibitsOverviewViewModel(ExhibitSet set, IGeolocator geolocator)
        {
            Title = "Übersicht";
            if (set != null)
            {
                ExhibitsList = new ObservableCollection<ExhibitsOverviewListItemViewModel>();
                foreach (Exhibit exhibit in set)
                {
                    var listItem = new ExhibitsOverviewListItemViewModel(exhibit);
                    ExhibitsList.Add(listItem);
                }
            }
            ItemTappedCommand = new Command(item => NavigateToExhibitDetails(item as ExhibitsOverviewListItemViewModel));

            if (geolocator != null)
            {
                locator = geolocator;
            }
        }

        public ExhibitsOverviewViewModel (ExhibitSet set) : this(set, null)
        {
            // TODO use application constants here
            locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 10;
            locator.PositionChanged += LocatorOnPositionChanged;
            locator.StartListeningAsync(4000, 10);
        }

        public ExhibitsOverviewViewModel (string exhibitSetId) : this(ExhibitManager.GetExhibitSet(exhibitSetId))
        {   
        }

        /// <summary>
        /// React to position changes.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="positionEventArgs">The event params.</param>
        private void LocatorOnPositionChanged (object sender, PositionEventArgs positionEventArgs)
        {
            SetDistances (positionEventArgs.Position);
        }

        /// <summary>
        /// Update the distances according to the new position.
        /// </summary>
        /// <param name="position">The new position.</param>
        private void SetDistances (Position position)
        {
            foreach (var exhibit in ExhibitsList)
            {
                exhibit.UpdateDistance(position);
            }
        }

        /// <summary>
        /// Remove the location listener once this view disappears.
        /// </summary>
        public override void OnDisappearing ()
        {
            base.OnDisappearing ();

            locator.PositionChanged -= LocatorOnPositionChanged;
        }

        /// <summary>
        /// Open the exhibitdetails page.
        /// </summary>
        /// <param name="item"></param>
        private void NavigateToExhibitDetails (ExhibitsOverviewListItemViewModel item)
        {
            if (item != null)
            {
                Navigation.PushAsync (new ExhibitDetailsViewModel (item.Exhibit));
            }
        }

        /// <summary>
        /// The list of displayed exhibits.
        /// </summary>
        public ObservableCollection<ExhibitsOverviewListItemViewModel> ExhibitsList {
            get { return exhibitsList; }
            set { SetProperty (ref exhibitsList, value); }
        }

        /// <summary>
        /// The command for tapping on exhibits.
        /// </summary>
        public ICommand ItemTappedCommand {
            get { return itemTappedCommand; }
            set { SetProperty (ref itemTappedCommand, value); }
        }

    }
}
