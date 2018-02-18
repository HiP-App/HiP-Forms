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
using System.Linq;
using System.Windows.Input;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentHandling;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Location;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using Plugin.Geolocator.Abstractions;
using Xamarin.Forms;
using System.Collections.Generic;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views
{
    public class ExhibitsOverviewViewModel : NavigationViewModel, ILocationListener, IDbChangedObserver
    {
        private readonly ILocationManager locationManager;
        private readonly INearbyExhibitManager nearbyExhibitManager;
        private readonly INearbyRouteManager nearbyRouteManager;

        private ObservableCollection<ExhibitsOverviewListItemViewModel> exhibits; // observable because items are reordered according to distance to user
        private bool displayDistances = false;
        private Position position;

        public ExhibitsOverviewViewModel(IReadOnlyList<Exhibit> exhibits)
        {
            if (exhibits != null)
            {
                Exhibits = new ObservableCollection<ExhibitsOverviewListItemViewModel>(
                    DbManager.DataAccess.Exhibits().GetExhibits().Select(ex => new ExhibitsOverviewListItemViewModel(ex)));
            }

            ItemTappedCommand = new Command(item => NavigateToExhibitDetails(item as ExhibitsOverviewListItemViewModel));

            locationManager = IoCManager.Resolve<ILocationManager>();
            nearbyExhibitManager = IoCManager.Resolve<INearbyExhibitManager>();
            nearbyRouteManager = IoCManager.Resolve<INearbyRouteManager>();
            var dbChangedHandler = IoCManager.Resolve<IDbChangedHandler>();
            dbChangedHandler.AddObserver(this);

            DownloadUpdatedData();
        }

        /// <summary>
        /// React to position changes.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="args">The event params.</param>
        public void LocationChanged(object sender, PositionEventArgs args)
        {
            Position = args.Position;

            if (Exhibits == null)
                return;

            SetDistances(args.Position);

            nearbyExhibitManager.CheckNearExhibit(exhibits.Select(vm => vm.Exhibit), args.Position.ToGeoLocation(), true, locationManager.ListeningInBackground);
            nearbyRouteManager.CheckNearRoute(DbManager.DataAccess.Routes().GetRoutes(), args.Position.ToGeoLocation());
        }

        /// <summary>
        /// Update the distances according to the new position.
        /// </summary>
        /// <param name="pos">The new position.</param>
        private void SetDistances(Position pos)
        {
            DisplayDistances = true;
            foreach (var exhibit in Exhibits)
            {
                exhibit.UpdateDistance(pos);
            }
            Exhibits.SortCollection(exhibit => exhibit.Distance);
        }

        /// <summary>
        /// Called when the view was added to the visual tree.
        /// </summary>
        public override void OnAppearing()
        {
            base.OnAppearing();

            locationManager.AddLocationListener(this);
        }

        /// <summary>
        /// Called when the view was removed from the visual tree.
        /// </summary>
        public override void OnDisappearing()
        {
            base.OnDisappearing();

            locationManager.RemoveLocationListener(this);
        }

        /// <summary>
        /// Open the exhibitdetails page.
        /// </summary>
        /// <param name="item"></param>
        private async void NavigateToExhibitDetails(ExhibitsOverviewListItemViewModel item)
        {
            if (item != null)
            {
                await Navigation.PushAsync(new AppetizerPageViewModel(item.Exhibit));
            }
        }

        /// <summary>
        /// The list of displayed exhibits.
        /// </summary>
        public ObservableCollection<ExhibitsOverviewListItemViewModel> Exhibits
        {
            get => exhibits;
            set => SetProperty(ref exhibits, value);
        }

        /// <summary>
        /// The command for tapping on exhibits.
        /// </summary>
        public ICommand ItemTappedCommand { get; }

        /// <summary>
        /// Whether to display the distance to exhibit.
        /// </summary>
        public bool DisplayDistances
        {
            get => displayDistances;
            set => SetProperty(ref displayDistances, value);
        }

        /// <summary>
        /// The geolocation of the user
        /// </summary>
        public Position Position
        {
            get => position;
            set => SetProperty(ref position, value);
        }

        /// <summary>
        /// Refreshs the exhibitsList depending on the changed database
        /// </summary>
        public void DbChanged()
        {
            Exhibits = new ObservableCollection<ExhibitsOverviewListItemViewModel>(
                DbManager.DataAccess.Exhibits().GetExhibits().Select(ex => new ExhibitsOverviewListItemViewModel(ex)));
        }

        /// <summary>
        /// Download updated data
        /// </summary>
        private async void DownloadUpdatedData()
        {
            var newDataCenter = IoCManager.Resolve<INewDataCenter>();

            if (!newDataCenter.IsNewDataAvailabe())
                return;

            var downloadData = false;
            if (!Settings.AlwaysDownloadData)
            {
                var result = await Navigation.DisplayActionSheet(Strings.DownloadData_Title, null, null,
                    Strings.DownloadData_Accept, Strings.DownloadData_Cancel, Strings.DownloadData_Always);

                if (result == Strings.DownloadData_Always)
                {
                    Settings.AlwaysDownloadData = true;
                    downloadData = true;
                }
                else if (result == Strings.DownloadData_Accept)
                {
                    downloadData = true;
                }
            }
            else
            {
                downloadData = true;
            }

            if (downloadData)
            {
                //TODO Not defined until now what screen should be displayed while new data is downloaded
                await newDataCenter.UpdateData();
            }
        }
    }
}