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
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Location;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using Plugin.Geolocator.Abstractions;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views
{
    class ExhibitsOverviewViewModel : NavigationViewModel, ILocationListener, IDbChangedObserver
    {
        private ObservableCollection<ExhibitsOverviewListItemViewModel> exhibitsList;
        private ICommand itemTappedCommand;
        private ILocationManager locationManager;
        private INearbyExhibitManager nearbyExhibitManager;
        private readonly INearbyRouteManager nearbyRouteManager;
        private IDbChangedHandler dbChangedHandler;
        private bool displayDistances;
        private ExhibitSet displayedExhibitSet;
        private Position position;

        public ExhibitsOverviewViewModel(ExhibitSet set)
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


            locationManager = IoCManager.Resolve<ILocationManager> ();
            nearbyExhibitManager = IoCManager.Resolve<INearbyExhibitManager> ();
            nearbyRouteManager = IoCManager.Resolve<INearbyRouteManager>();
            dbChangedHandler = IoCManager.Resolve<IDbChangedHandler> ();
            dbChangedHandler.AddObserver (this);
        }

        public ExhibitsOverviewViewModel (string exhibitSetId) : this(ExhibitManager.GetExhibitSet(exhibitSetId))
        {   
        }

        /// <summary>
        /// React to position changes.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="args">The event params.</param>
        public void LocationChanged(object sender, PositionEventArgs args)
        {
            Position = args.Position;
            SetDistances(args.Position);

            nearbyExhibitManager.CheckNearExhibit (displayedExhibitSet, args.Position.ToGeoLocation (), true);
            nearbyRouteManager.CheckNearRoute(RouteManager.GetRoutes(), args.Position.ToGeoLocation ());
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
        /// Called when the view was removed from the visual tree.
        /// </summary>
        public override void OnDisappearing ()
        {
            base.OnDisappearing ();

            locationManager.RemoveLocationListener (this);
        }

        /// <summary>
        /// Called when the view was added to the visual tree.
        /// </summary>
        public override void OnAppearing ()
        {
            base.OnAppearing ();

            locationManager.AddLocationListener (this);
        }

        public override void OnHidden ()
        {
            base.OnHidden ();

            locationManager.RemoveLocationListener(this);
        }

        public override void OnRevealed ()
        {
            base.OnRevealed ();

            locationManager.AddLocationListener(this);
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


        public void DbChanged ()
        {
            var set = ExhibitManager.GetExhibitSet ();
            DisplayedExhibitSet = set;
            ExhibitsList = new ObservableCollection<ExhibitsOverviewListItemViewModel>();
            foreach (Exhibit exhibit in set)
            {
                var listItem = new ExhibitsOverviewListItemViewModel(exhibit);
                ExhibitsList.Add(listItem);
            }
        }

    }
}
