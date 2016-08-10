// Copyright (C) 2016 History in Paderborn App - Universität Paderborn
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//       http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections.Generic;
using Android.OS;
using Android.Support.V4.Content;
using Android.Views;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using Osmdroid.TileProvider.TileSource;
using Osmdroid.Util;
using Osmdroid.Views;
using Osmdroid.Views.Overlay;
using Fragment = Android.Support.V4.App.Fragment;

namespace de.upb.hip.mobile.droid.fragments {
    /// <summary>
    /// Fragment displaying a map, including markers for exhibits contained in an ExhibitSet.
    /// </summary>
    public class MapFragment : Fragment {

        /// <summary>
        /// ExhibitSet containing the exhibit that should be displayed in the RecyclerView.
        /// </summary>
        public ExhibitSet ExhibitSet { get; set; }

        /// <summary>
        /// GeoLocation for the current position of the user.
        /// </summary>
        public GeoLocation GeoLocation { get; set; }

        /// <summary>
        /// LocationOverlay used for the map.
        /// </summary>
        private MyLocationOverlay LocationOverlay { get; set; }

        #region

        // Keys to save/restore instance state.
        private const string KeyExhibitSetId = "ExhibitSetId";
        private const string KeyGeoLocationLatitude = "GeoLocation.Latitude";
        private const string KeyGeoLocationLongitude = "GeoLocation.Longitude";

        #endregion

        public override void OnSaveInstanceState (Bundle outState)
        {
            base.OnSaveInstanceState (outState);

            outState.PutString (KeyExhibitSetId, ExhibitSet.Id);
            outState.PutDouble (KeyGeoLocationLatitude, GeoLocation.Latitude);
            outState.PutDouble (KeyGeoLocationLongitude, GeoLocation.Longitude);
        }

        public override void OnCreate (Bundle savedInstanceState)
        {
            base.OnCreate (savedInstanceState);

            if (savedInstanceState != null)
            {
                var latitude = savedInstanceState.GetDouble (KeyGeoLocationLatitude);
                var longitude = savedInstanceState.GetDouble (KeyGeoLocationLongitude);
                GeoLocation = new GeoLocation
                {
                    Latitude = latitude,
                    Longitude = longitude
                };

                var exhibitId = savedInstanceState.GetString (KeyExhibitSetId);
                ExhibitSet = ExhibitManager.GetExhibitSet (exhibitId);
            }
        }

        public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate (Resource.Layout.fragment_map, container, false);

            var mapView = view.FindViewById<MapView> (Resource.Id.mapview);
            mapView.SetTileSource(TileSourceFactory.DefaultTileSource);
            mapView.SetBuiltInZoomControls (false);
            mapView.SetMultiTouchControls (true);
            mapView.TilesScaledToDpi = true;

            //mapView.SetTileSource (new XYTileSource ("OSM", 0, 18, 1024, ".png",
                                                     //new[] {"http://tile.openstreetmap.org/"}));

            var mapController = mapView.Controller;
            mapController.SetZoom (13);

            // var centreOfMap = new GeoPoint(51496994, -134733);
            var centreOfMap = new GeoPoint (GeoLocation.Latitude, GeoLocation.Longitude);
            mapController.SetCenter (centreOfMap);

            SetAllMarkers (mapView);

            return view;
        }

        /// <summary>
        /// Adds all markers of the current ExhibitSet to the specified map.
        /// </summary>
        /// <param name="mapView">MapView where the markers should be displayed.</param>
        private void SetAllMarkers (MapView mapView)
        {
            //SetUp Markers TODO rewrite with markers from bonuspack
            var mapMarkerArray = new List<OverlayItem> ();
            LocationOverlay = new MyLocationOverlay (this.Activity, mapView);
            var mapMarkerIcon = ContextCompat.GetDrawable (this.Activity, Resource.Drawable.marker_blue);
            var myScaleBarOverlay = new ScaleBarOverlay (this.Activity);

            foreach (var e in ExhibitSet.ActiveSet)
            {
                //One Marker Object
                var marker = new OverlayItem (e.Marker.Title, e.Marker.Text, new GeoPoint (e.Location.Latitude, e.Location.Longitude));
                marker.SetMarker (mapMarkerIcon);
                mapMarkerArray.Add (marker);
            }

            //Initialize this after markers are added to 
            var mapMarkerItemizedOverlay = new ItemizedIconOverlay (this.Activity, mapMarkerArray, null);
            mapView.OverlayManager.Add (mapMarkerItemizedOverlay);
            mapView.OverlayManager.Add (myScaleBarOverlay);
            mapView.OverlayManager.Add (LocationOverlay);
            mapView.PostInvalidate ();
        }

        public override void OnResume ()
        {
            base.OnResume ();
            LocationOverlay.EnableMyLocation ();
            LocationOverlay.EnableCompass ();
        }

        public override void OnPause ()
        {
            base.OnPause ();
            LocationOverlay.DisableMyLocation ();
            LocationOverlay.DisableCompass ();
        }

    }
}