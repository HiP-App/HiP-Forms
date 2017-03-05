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
// limitations under the License.using System;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Support.V4.Content.Res;
using Android.Views;
using Android.Widget;
using de.upb.hip.mobile.droid.Map;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using de.upb.hip.mobile.pcl.BusinessLayer.Routing;
using de.upb.hip.mobile.pcl.Helpers;
using HipMobileUI.Helpers;
using HipMobileUI.Map;
using Org.Osmdroid;
using Org.Osmdroid.Bonuspack.Overlays;
using Org.Osmdroid.Events;
using Org.Osmdroid.Tileprovider.Tilesource;
using Org.Osmdroid.Util;
using Org.Osmdroid.Util.Constants;
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
        private RouteCalculator routeCalculator;
        private Polyline currentRouteOverlay;

        protected override void OnElementChanged (ElementChangedEventArgs<OsmMap> e)
        {
            base.OnElementChanged (e);


            if (Control == null)
            {
                mapView = new MapView (Forms.Context, 11);
                activity = Context as Activity;
                routeCalculator = RouteCalculator.Instance;
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
                e.NewElement.CenterLocationCalled+=CenterMap;
            }
        }

        private void CenterMap (GeoLocation location)
        {
            mapController.SetCenter(new GeoPoint (location.Latitude, location.Longitude));
        }

        private void NewElementOnExhibitSetChanged (ExhibitSet set)
        {
            SetAllMarkers (set);
        }


        private void NewElementOnGpsLocationChanged (GeoLocation gpsLocation)
        {
                //Userposition is always updated
            if (gpsLocation != null)
            {

                userPosition = new GeoPoint (gpsLocation.Latitude, gpsLocation.Longitude);
                mapController.SetCenter (userPosition);
                if (userMarkerPosition != null)//because this is called after exhibitset we have to check if the marker is initialized
                {
                    userMarkerPosition.SetIcon (ResourcesCompat.GetDrawable (Resources, Resource.Drawable.ic_my_location, null));
                    userMarkerPosition.Position = userPosition;
                    userMarkerPosition.SetInfoWindow (null);
                    mapView.OverlayManager.Add (userMarkerPosition);
                    mapView.Invalidate ();
                }

                //if navigation is enabled the route will be drawn and updated
				if (osmMap.ShowNavigation)
				{
					UpdateRoute (new GeoPoint(gpsLocation.Latitude,gpsLocation.Longitude));
				}
            }

        }

        private void NewElementOnDetailsRouteChanged (Route route)
        {
            //The direct polyline is only draw if related bool is true
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

            if(osmMap.ShowDetailsRoute)
                mapView.OverlayManager.Add (myPath);
            mapView.Invalidate ();
        }

        //here all Markers for the Exhibits in the Main Map are set
        //and some general stuff
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

            //Here all exhibit markers and bubbles are set if the Exhibit is not null
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

        private void CalculateRoute()
        {
            var id = osmMap.DetailsRoute.Id;

            ThreadPool.QueueUserWorkItem(state => {
                var geoPoints = new List<GeoPoint> { new GeoPoint(userPosition.Latitude, userPosition.Longitude) };
               
                Action action;

                try
                {
                    var locations = routeCalculator.CreateRouteWithSeveralWaypoints(new GeoLocation(userPosition.Latitude, userPosition.Longitude),
                                                                                     id);

                    foreach (var w in locations)
                    {
                        var point = new GeoPoint(w.Latitude, w.Longitude);
                        geoPoints.Add(point);
                    }

                    action = () => DrawRoute(geoPoints);
                }
                catch (Exception)
                {
                    action = ShowRouteCalculationError;
                }

                activity.RunOnUiThread(() => {
                    var handler = new Handler();
                    handler.PostDelayed(action, 0);
                });
            });
        }

        private void ShowRouteCalculationError()
        {
            Toast.MakeText(activity,"Sie befinden sich nicht innerhalb von Paderborn", ToastLength.Long).Show();
        }

        private void DrawRoute(List<GeoPoint> geoPoints)
        {
            //Cleanup route if drawn before
            if (currentRouteOverlay != null)
                mapView.OverlayManager.Remove(currentRouteOverlay);

            currentRouteOverlay = new Polyline(activity)
            {
                Title = osmMap.DetailsRoute.Title,
                Width = 5f,
                Color = Android.Graphics.Color.Blue,
                Points = geoPoints,
                Geodesic = true
            };
            mapView.OverlayManager.Add(currentRouteOverlay);
            mapView.Invalidate();
        }

        //TODO needs connection to some gps listener
        private void UpdateRoute(GeoPoint currentLocation)
        {
            
            userPosition = currentLocation;
            mapView.Invalidate();

            CalculateRoute();
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