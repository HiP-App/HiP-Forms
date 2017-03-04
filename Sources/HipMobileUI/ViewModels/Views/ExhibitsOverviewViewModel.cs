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
using HipMobileUI.Helpers;
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
        private readonly IGeolocator locator;
        private bool displayDistances;
        private ExhibitSet displayedExhibitSet;
        private Position position;

        public ExhibitsOverviewViewModel(ExhibitSet set, IGeolocator geolocator)
        {
            if (set != null)
            {
                DisplayedExhibitSet = set;
                ExhibitsList = new ObservableCollection<ExhibitsOverviewListItemViewModel>();
                foreach (Exhibit exhibit in set)
                {
                    var listItem = new ExhibitsOverviewListItemViewModel(exhibit);
                    ExhibitsList.Add(listItem);
                }
            }
            ItemTappedCommand = new Command(item => NavigateToExhibitDetails(item as ExhibitsOverviewListItemViewModel));
            DisplayDistances = false;

            if (geolocator != null)
            {
                locator = geolocator;
                locator.DesiredAccuracy = AppSharedData.MinDistanceChangeForUpdates;
            }

            MessagingCenter.Subscribe<App>(this, AppSharedData.WillSleepMessage, WillSleep);
            MessagingCenter.Subscribe<App>(this, AppSharedData.WillWakeUpMessage, WillWakeUp);
        }

        public ExhibitsOverviewViewModel (ExhibitSet set) : this(set, null)
        {
            locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = AppSharedData.MinDistanceChangeForUpdates;
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
            Position = positionEventArgs.Position;
            SetDistances (positionEventArgs.Position);
        }

        /// <summary>
        /// Update the distances according to the new position.
        /// </summary>
        /// <param name="pos">The new position.</param>
        private void SetDistances (Position pos)
        {
            DisplayDistances = true;
            foreach (var exhibit in ExhibitsList)
            {
                exhibit.UpdateDistance(pos);
            }
            ExhibitsList.SortCollection (exhibit => exhibit.Distance);
        }

        /// <summary>
        /// Called when the app will go to the background or the screen is locked.
        /// </summary>
        /// <param name="obj">The caller.</param>
        private void WillSleep(App obj)
        {
            locator.PositionChanged -= LocatorOnPositionChanged;
            if (locator.IsListening)
            {
                locator.StopListeningAsync();
            }
        }

        /// <summary>
        /// Called when the app will wake up.
        /// </summary>
        /// <param name="obj">The sender of the event.</param>
        private void WillWakeUp(App obj)
        {
            locator.PositionChanged += LocatorOnPositionChanged;
            if (!locator.IsListening)
            {
                locator.StartListeningAsync(AppSharedData.MinTimeBwUpdates, AppSharedData.MinDistanceChangeForUpdates);
            }
        }

        /// <summary>
        /// Called when the view was removed from the visual tree.
        /// </summary>
        public override void OnDisappearing ()
        {
            base.OnDisappearing ();

            locator.PositionChanged -= LocatorOnPositionChanged;
        }

        /// <summary>
        /// Called when the view was added to the visual tree.
        /// </summary>
        public override void OnAppearing ()
        {
            base.OnAppearing ();

            locator.PositionChanged += LocatorOnPositionChanged;
            if (!locator.IsListening)
            {
                locator.StartListeningAsync (AppSharedData.MinTimeBwUpdates, AppSharedData.MinDistanceChangeForUpdates);
            }
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

        /// <summary>
        /// Whether to display the distance to exhibit.
        /// </summary>
        public bool DisplayDistances {
            get { return displayDistances; }
            set { SetProperty (ref displayDistances, value); }
        }

        /// <summary>
        /// The displayed set of exhibits on the map.
        /// </summary>
        public ExhibitSet DisplayedExhibitSet {
            get { return displayedExhibitSet; }
            set { SetProperty (ref displayedExhibitSet, value); }
        }

        /// <summary>
        /// The geolocation of the user
        /// </summary>
        public Position Position {
            get { return position; }
            set { SetProperty (ref position, value); }
        }

    }
}
