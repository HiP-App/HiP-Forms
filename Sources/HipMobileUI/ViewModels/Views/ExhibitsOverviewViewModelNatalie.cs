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
	public class ExhibitsOverviewViewModelNatalie : NavigationViewModel, ILocationListener, IDbChangedObserver
    {
        private readonly ILocationManager locationManager;
        private readonly INearbyExhibitManager nearbyExhibitManager;
        private readonly INearbyRouteManager nearbyRouteManager;

        private ObservableCollection<ExhibitsOverviewListItemViewModel> exhibits; // observable because items are reordered according to distance to user
        private bool displayDistances = false;
        private GeoLocation? position;
        private GeoLocation gpsLocation;
        private ICommand mapFocusCommand;

        public ExhibitsOverviewViewModelNatalie(IReadOnlyList<Exhibit> exhibits)
		{
		    if (exhibits != null)
		    {
		        Exhibits = new ObservableCollection<ExhibitsOverviewListItemViewModel>(
		            exhibits.Select(ex => new ExhibitsOverviewListItemViewModel(ex)));
		    }

		    ItemTappedCommand = new Command(item => NavigateToExhibitDetails(item as ExhibitsOverviewListItemViewModel));



            locationManager = IoCManager.Resolve<ILocationManager>();
		    nearbyExhibitManager = IoCManager.Resolve<INearbyExhibitManager>();
		    nearbyRouteManager = IoCManager.Resolve<INearbyRouteManager>();
		    var dbChangedHandler = IoCManager.Resolve<IDbChangedHandler>();
		    dbChangedHandler.AddObserver(this);
		    FocusGps = new Command(FocusGpsClicked);
		    DownloadUpdatedData();
		}

        private async void NavigateToExhibitDetails(ExhibitsOverviewListItemViewModel item)
        {
            if (item != null)
            {
                await Navigation.PushAsync(new AppetizerPageViewModel(item.Exhibit));
            }
        }


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
        private void SetDistances(Position pos)
        {
            DisplayDistances = true;
            foreach (var exhibit in Exhibits)
            {
                exhibit.UpdateDistance(pos);
            }
            Exhibits.SortCollection(exhibit => exhibit.Distance);
        }



        private void FocusGpsClicked()
        {
            MapFocusCommand.Execute(GpsLocation);
        }

        public GeoLocation? Position
        {
            get => position;
            set => SetProperty(ref position, value);
        }

        public ICommand ItemTappedCommand { get; }

        public ICommand FocusGps { get; }

        public bool DisplayDistances
        {
            get => displayDistances;
            set => SetProperty(ref displayDistances, value);
        }

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
        public ObservableCollection<ExhibitsOverviewListItemViewModel> Exhibits
        {
            get => exhibits;
            set => SetProperty(ref exhibits, value);
        }
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