// Copyright (C) 2016 History in Paderborn App - Universitšt Paderborn
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
// limitations under the License.using System;

using System.Linq;
using Android.App;
using Android.Support.V4.Content;
using Android.Support.V4.Content.Res;
using de.upb.hip.mobile.droid.Map;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using HipMobileUI.Helpers;
using HipMobileUI.Map;
using Org.Osmdroid;
using Org.Osmdroid.Bonuspack.Overlays;
using Org.Osmdroid.Events;
using Org.Osmdroid.Tileprovider.Tilesource;
using Org.Osmdroid.Util;
using Org.Osmdroid.Views;
using Org.Osmdroid.Views.Overlay;
using Org.Osmdroid.Views.Overlay.Compass;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer (typeof (OsmMap), typeof (DroidMapRenderer))]

namespace de.upb.hip.mobile.droid.Map {
    class DroidMapRenderer : Xamarin.Forms.Platform.Android.AppCompat.ViewRenderer<OsmMap, MapView>, IMapListener {

        private OsmMap osmMap;
        private MapView mapView;
        private MapController mapController;
        private GeoPoint userPosition;
        private Marker userMarkerPosition;
        private MyLocationOverlay locationOverlay;
        private Activity activity;

        protected override void OnElementChanged (ElementChangedEventArgs<OsmMap> e)
        {
            base.OnElementChanged (e);


            if (Control == null)
            {
                mapView = new MapView (Forms.Context, 11);
                activity = Context as Activity;

                this.SetNativeControl (mapView);
                userPosition = new GeoPoint (AppSharedData.PaderbornMainStation.Latitude, AppSharedData.PaderbornMainStation.Longitude);
                mapView.SetTileSource (TileSourceFactory.DefaultTileSource);
                /*mapView.SetTileSource(new XYTileSource("
                 * ", ResourceProxyString.OnlineMode, 0, 18, 1024, ".png",
                new[] {"http://tile.openstreetmap.org/"}));*/

                mapView.SetMultiTouchControls (true);
                mapView.TilesScaledToDpi = true;

                mapController = (MapController) mapView.Controller;
                mapController.SetCenter (userPosition);
                mapController.SetZoom (AppSharedData.MapZoomLevel);
                mapView.SetMapListener (this);
            }

            if (e.OldElement != null)
            {
                // Unsubscribe
                e.OldElement.ExhibitSetChanged -= NewElementOnExhibitSetChanged;
                e.OldElement.GpsLocationChanged -= NewElementOnGpsLocationChanged;
                e.OldElement.DetailsRouteChanged -= NewElementOnDetailsRouteChanged;
            }
            if (e.NewElement != null)
            {
                // Subscribe
                osmMap = e.NewElement;
                e.NewElement.GpsLocationChanged += NewElementOnGpsLocationChanged;
                NewElementOnGpsLocationChanged (e.NewElement.GpsLocation);
                e.NewElement.ExhibitSetChanged += NewElementOnExhibitSetChanged;
                NewElementOnExhibitSetChanged (e.NewElement.ExhibitSet);
                e.NewElement.DetailsRouteChanged += NewElementOnDetailsRouteChanged;
                NewElementOnDetailsRouteChanged (e.NewElement.DetailsRoute);
            }
        }

        private void NewElementOnExhibitSetChanged (ExhibitSet set)
        {
            SetAllMarkers (set);
        }


        private void NewElementOnGpsLocationChanged (GeoLocation gpsLocation)
        {
            if (gpsLocation != null)
            {
                userPosition = new GeoPoint (gpsLocation.Latitude, gpsLocation.Longitude);
                mapController.SetCenter (userPosition);
                if (userMarkerPosition != null)
                {
                    userMarkerPosition.SetIcon (ResourcesCompat.GetDrawable (Resources, Resource.Drawable.ic_my_location, null));
                    userMarkerPosition.Position = userPosition;
                    userMarkerPosition.SetInfoWindow (null);
                    mapView.OverlayManager.Add (userMarkerPosition);
                    mapView.Invalidate ();
                }
            }
        }

        private void NewElementOnDetailsRouteChanged (Route route)
        {

            if (osmMap.ShowDetailsRoute)
            {
                PathOverlay myPath = new PathOverlay (Resources.GetColor (Resource.Color.colorPrimaryDark), 7, new DefaultResourceProxyImpl (activity));

                if (userPosition != null)
                {
                    myPath.AddPoint (userPosition);
                }

                if (route != null && route.Waypoints.Any ())
                {
                    foreach (Waypoint waypoint in route.Waypoints)
                    {
                        myPath.AddPoint (new GeoPoint (waypoint.Location.Latitude, waypoint.Location.Longitude));
                        var marker = new Marker (mapView);
                        marker.SetIcon(ResourcesCompat.GetDrawable(Resources, Resource.Drawable.marker_blue, null));
                        marker.Position = new GeoPoint(waypoint.Location.Latitude,waypoint.Location.Longitude);
                        mapView.OverlayManager.Add(marker);
                    }
                }

                mapView.OverlayManager.Add (myPath);
                mapView.Invalidate ();
            }
        }


        private void SetAllMarkers (ExhibitSet set)
        {
            locationOverlay = new MyLocationOverlay (activity, mapView);
            CompassOverlay compassOverlay = new CompassOverlay (activity, mapView);
            compassOverlay.EnableCompass ();
            userMarkerPosition = new Marker (mapView);
            userMarkerPosition.SetIcon (ResourcesCompat.GetDrawable (Resources, Resource.Drawable.ic_my_location, null));
            userMarkerPosition.Position = userPosition;
            userMarkerPosition.SetInfoWindow (null);
            mapView.OverlayManager.Add (userMarkerPosition);
            mapView.OverlayManager.Add (locationOverlay);
            mapView.OverlayManager.Add (compassOverlay);

            if (set != null)
            {
                var markerInfoWindow = new ViaPointInfoWindow (Resource.Layout.navigation_info_window, mapView, activity);
                var mapMarkerIcon = ContextCompat.GetDrawable (activity, Resource.Drawable.marker_blue);
                var setMarker = new SetMarker (mapView, markerInfoWindow);


                foreach (var e in set.ActiveSet)
                {
                    //One Marker Object
                    var geoPoint = new GeoPoint (e.Location.Latitude, e.Location.Longitude);
                    var marker = setMarker.AddMarker (null, e.Name, e.Description, geoPoint, mapMarkerIcon, e.Id);
                    mapView.OverlayManager.Add (marker);
                }
            }


            mapView.Invalidate ();
        }

        public bool OnScroll (ScrollEvent p0)
        {
            return false;
        }

        public bool OnZoom (ZoomEvent p0)
        {
            AppSharedData.MapZoomLevel = p0.ZoomLevel;
            return true;
        }

        /// <summary>
        /// Called when this view will be disposed.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                osmMap.ExhibitSetChanged -= NewElementOnExhibitSetChanged;
                osmMap.GpsLocationChanged -= NewElementOnGpsLocationChanged;
                osmMap.DetailsRouteChanged -= NewElementOnDetailsRouteChanged;
            }
            base.Dispose(disposing);
        }

    }
}