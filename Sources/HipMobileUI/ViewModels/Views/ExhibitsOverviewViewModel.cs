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
using Plugin.Geolocator.Abstractions;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Diagnostics;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views
{
    public class ExhibitsOverviewViewModel : NavigationViewModel, ILocationListener, IDbChangedObserver
    {
        private readonly ILocationManager locationManager;
        private readonly INearbyExhibitManager nearbyExhibitManager;
        private readonly INearbyRouteManager nearbyRouteManager;

        private ObservableCollection<ExhibitsOverviewListItemViewModel> exhibits; // observable because items are reordered according to distance to user
        private bool displayDistances = false;
        private GeoLocation? position;
        private GeoLocation gpsLocation;
        private ICommand mapFocusCommand;
        private ICommand selectedExhibitChangedCommand;
        private Exhibit selectedExhibit;

        public ExhibitsOverviewViewModel(IReadOnlyList<Exhibit> exhibits)
        {
            if (exhibits != null)
            {
                Exhibits = new ObservableCollection<ExhibitsOverviewListItemViewModel>(
                    exhibits.Select(ex => new ExhibitsOverviewListItemViewModel(ex)));
                SelectedExhibit = exhibits.ElementAt(0);
                
            }

            locationManager = IoCManager.Resolve<ILocationManager>();
            nearbyExhibitManager = IoCManager.Resolve<INearbyExhibitManager>();
            nearbyRouteManager = IoCManager.Resolve<INearbyRouteManager>();
            var dbChangedHandler = IoCManager.Resolve<IDbChangedHandler>();
            dbChangedHandler.AddObserver(this);
            FocusGps = new Command(FocusGpsClicked);
            DownloadUpdatedData();
        }
        /// <summary>
        /// React to position changes.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="args">The event params.</param>
        public void LocationChanged(object sender, PositionEventArgs args)
        {
            Position = new GeoLocation(args.Position.Latitude, args.Position.Longitude);
            GpsLocation = args.Position.ToGeoLocation();

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
        /// 
        private void FocusGpsClicked()
        {
            MapFocusCommand.Execute(GpsLocation);
        }

        private void SetDistances(Position pos)
        {
            DisplayDistances = true;
            foreach (var exhibit in Exhibits)
            {
                exhibit.UpdateDistance(pos);
            }
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
        /// The list of displayed exhibits.
        /// </summary>
        public ObservableCollection<ExhibitsOverviewListItemViewModel> Exhibits
        {
            get => exhibits;
            set => SetProperty(ref exhibits, value);
        }

        // Temporarily needed for OsmMap binding. TODO: No longer needed when merged with HIPM-868.
        public IReadOnlyList<Exhibit> RawExhibits => Exhibits.Select(vm => vm.Exhibit).ToList();

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
        public GeoLocation? Position
        {
            get => position;
            set => SetProperty(ref position, value);
        }

        public ICommand FocusGps { get; }

        public GeoLocation GpsLocation
        {
            get { return gpsLocation; }
            set { SetProperty(ref gpsLocation, value); }
        }

        public ICommand MapFocusCommand
        {
            get { return mapFocusCommand; }
            set { SetProperty(ref mapFocusCommand, value); }
        }

        /// <summary>
        /// Refreshs the exhibitsList depending on the changed database
        /// </summary>
        public void DbChanged()
        {
            Exhibits = new ObservableCollection<ExhibitsOverviewListItemViewModel>(
                DbManager.DataAccess.Exhibits().GetExhibits().Select(ex => new ExhibitsOverviewListItemViewModel(ex)));
        }

        public void ExhibitChanged(Exhibit exhibit)
        {
            SelectedExhibit = exhibit;
            Debug.WriteLine(selectedExhibit.Name);
            //if (exhibit != null) { SelectedExhibitChangedCommand.Execute(exhibit); }
            
            //Todo add setting map markers 
        }
        public Exhibit SelectedExhibit
        {
            get { return selectedExhibit; }
            set { SetProperty(ref selectedExhibit,value);}
        }

        public ICommand SelectedExhibitChangedCommand
        {
            get { return selectedExhibitChangedCommand; }
            set { SetProperty(ref selectedExhibitChangedCommand, value); }
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
                    Strings.Yes, Strings.No, Strings.DownloadData_Always);

                if (result == Strings.DownloadData_Always)
                {
                    Settings.AlwaysDownloadData = true;
                    downloadData = true;
                }
                else if (result == Strings.Yes)
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