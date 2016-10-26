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

using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V4.Content.Res;
using Android.Views;
using de.upb.hip.mobile.droid.Helpers;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using Org.Osmdroid.Bonuspack.Overlays;
using Org.Osmdroid.Tileprovider.Tilesource;
using Org.Osmdroid.Util;
using Org.Osmdroid.Views;
using Org.Osmdroid.Views.Overlay;

namespace de.upb.hip.mobile.droid.fragments {
    /// <summary>
    ///     Fragment displaying a map, including markers for exhibits contained in an ExhibitSet.
    /// </summary>
    public class MapFragment : Fragment {

        /// <summary>
        ///     ExhibitSet containing the exhibit that should be displayed in the RecyclerView.
        /// </summary>
        public ExhibitSet ExhibitSet { get; set; }

        /// <summary>
        ///     GeoLocation for the current position of the user.
        /// </summary>
        public GeoLocation GeoLocation { get; set; }

        /// <summary>
        ///     LocationOverlay used for the map.
        /// </summary>
        private MyLocationOverlay LocationOverlay { get; set; }

        private ScaleBarOverlay MyScaleBarOverlay { get; set; }

        private Marker userPosition;

        private MapView mapView;
        private MapController mapController;

        private int zoomlvl = 13;

        public override void OnSaveInstanceState (Bundle outState)
        {
            base.OnSaveInstanceState (outState);

            outState.PutString (KeyExhibitSetId, ExhibitSet.Id);
            outState.PutDouble (KeyGeoLocationLatitude, GeoLocation.Latitude);
            outState.PutDouble (KeyGeoLocationLongitude, GeoLocation.Longitude);
            outState.PutInt (zoomlevel,mapView.ZoomLevel);
        }

        public override void OnCreate (Bundle savedInstanceState)
        {
            base.OnCreate (savedInstanceState);
            RetainInstance = true;
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

            mapView = view.FindViewById<MapView> (Resource.Id.mapview);
            mapView.SetTileSource (TileSourceFactory.DefaultTileSource);
            mapView.SetBuiltInZoomControls (true);
            mapView.SetMultiTouchControls (true);
            mapView.TilesScaledToDpi = true;

            //mapView.SetTileSource (new XYTileSource ("OSM", 0, 18, 1024, ".png",
            //new[] {"http://tile.openstreetmap.org/"}));

            mapController = (MapController)mapView.Controller;
            if (savedInstanceState != null)
                zoomlvl = savedInstanceState.GetInt (zoomlevel);
            mapController.SetZoom (zoomlvl);

            // var centreOfMap = new GeoPoint(51496994, -134733);
            var centreOfMap = new GeoPoint (GeoLocation.Latitude, GeoLocation.Longitude);
            mapController.SetCenter (centreOfMap);


            SetAllMarkers (mapView);

            return view;
        }

        /// <summary>
        ///     Adds all markers of the current ExhibitSet to the specified map.
        /// </summary>
        /// <param name="mapView">MapView where the markers should be displayed.</param>
        private void SetAllMarkers (MapView mapView)
        {
            LocationOverlay = new MyLocationOverlay (Activity, mapView);
            MyScaleBarOverlay = new ScaleBarOverlay (Activity);

            var markerInfoWindow = new ViaPointInfoWindow (Resource.Layout.navigation_info_window, mapView, Activity);
            var mapMarkerIcon = ContextCompat.GetDrawable (Activity, Resource.Drawable.marker_blue);
            var setMarker = new SetMarker (mapView, markerInfoWindow);


            foreach (var e in ExhibitSet.ActiveSet)
            {
                //One Marker Object
                var geoPoint = new GeoPoint (e.Location.Latitude, e.Location.Longitude);
                var marker = setMarker.AddMarker (null, e.Name, e.Description, geoPoint, mapMarkerIcon, e.Id);
                mapView.OverlayManager.Add (marker);
            }


            userPosition = new Marker (mapView);
            userPosition.SetIcon (ResourcesCompat.GetDrawable (Resources, Resource.Drawable.ic_my_location, null));
            userPosition.Position = new GeoPoint (GeoLocation.Latitude, GeoLocation.Longitude);
            userPosition.SetInfoWindow (null);
            mapView.OverlayManager.Add (userPosition);

            mapView.OverlayManager.Add (MyScaleBarOverlay);
            mapView.OverlayManager.Add (LocationOverlay);
            mapView.Invalidate ();
        }

        public void Update (GeoLocation loc)
        {
            GeoPoint p = new GeoPoint (loc.Latitude,loc.Longitude);

            mapController.SetCenter (p);
            
            userPosition.SetIcon(ResourcesCompat.GetDrawable(Resources, Resource.Drawable.ic_my_location, null));
            userPosition.Position = new GeoPoint(p.Latitude, p.Longitude);
            userPosition.SetInfoWindow(null);
            mapView.OverlayManager.Add(userPosition);
            mapView.Invalidate();
            GeoLocation = loc;
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

        #region

        // Keys to save/restore instance state.
        private const string KeyExhibitSetId = "ExhibitSetId";
        private const string KeyGeoLocationLatitude = "GeoLocation.Latitude";
        private const string KeyGeoLocationLongitude = "GeoLocation.Longitude";
        private const string zoomlevel = "zoomlevel";

        #endregion
    }
}