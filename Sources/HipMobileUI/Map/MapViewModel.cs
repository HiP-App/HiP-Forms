using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using de.upb.hip.mobile.pcl.Helpers;
using HipMobileUI.ViewModels;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;

namespace HipMobileUI.Map {
    class MapViewModel : NavigationViewModel {

        private ExhibitSet exhibitSet;
        private GeoLocation gpsLocation;

        //TODO just for testing can be deleted later
        public MapViewModel ()
        {
            Title = "Map";
            ExhibitSet = ExhibitManager.GetExhibitSets ().First ();
            gpsLocation = new GeoLocation (51.73296887, 8.7352252);
            var locator = CrossGeolocator.Current;
            locator.PositionChanged += position_Changed;
            locator.StartListeningAsync (minTime: AppSharedConstants.MinTimeBwUpdates, minDistance: AppSharedConstants.MinDistanceChangeForUpdates);
        }


        // Callback function for when GPS location changes
        void position_Changed (object obj, PositionEventArgs e)
        {
            UpdateGpsDataList (e.Position);
        }

        // Update GPS location displays and database
        private void UpdateGpsDataList (Position position)
        {
            gpsLocation.Latitude = position.Latitude;
            gpsLocation.Longitude = position.Longitude;
        }

        public ExhibitSet ExhibitSet {
            get { return exhibitSet; }
            set { SetProperty (ref exhibitSet, value); }
        }

        public GeoLocation GpsLocation {
            get { return gpsLocation; }
            set { SetProperty (ref gpsLocation, value); }
        }

    }
}