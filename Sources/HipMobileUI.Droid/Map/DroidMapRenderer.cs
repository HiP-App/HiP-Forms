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
using System.Threading;
using Android.App;
using Android.OS;
using Android.Support.V4.Content;
using Android.Support.V4.Content.Res;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using Org.Osmdroid;
using Org.Osmdroid.Bonuspack.Overlays;
using Org.Osmdroid.Events;
using Org.Osmdroid.Tileprovider;
using Org.Osmdroid.Tileprovider.Tilesource;
using Org.Osmdroid.Util;
using Org.Osmdroid.Views;
using Org.Osmdroid.Views.Overlay;
using Org.Osmdroid.Views.Overlay.Compass;
using PaderbornUniversity.SILab.Hip.Mobile.Droid.Map;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Routing;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Map;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Color = Android.Graphics.Color;

[assembly: ExportRenderer (typeof (OsmMap), typeof (DroidMapRenderer))]

namespace PaderbornUniversity.SILab.Hip.Mobile.Droid.Map {
    internal class DroidMapRenderer : Xamarin.Forms.Platform.Android.AppCompat.ViewRenderer<OsmMap, MapView>, IMapListener {

        private Activity activity;
        private Polyline currentRouteOverlay;
        private Polyline currentSectionOverlay;
        private MyLocationOverlay locationOverlay;
        private MapController mapController;
        private MapView mapView;
        private OsmMap osmMap;
        private RouteCalculator routeCalculator;
        private Marker userMarkerPosition;

        public bool OnScroll (ScrollEvent p0)
        {
            return false;
        }

        public bool OnZoom (ZoomEvent p0)
        {
            AppSharedData.MapZoomLevel = p0.ZoomLevel;
            return true;
        }

        protected override void OnElementChanged (ElementChangedEventArgs<OsmMap> e)
        {
            base.OnElementChanged (e);

            if (Control == null)
            {
                mapView = new MapView (Forms.Context, 11);
                activity = Context as Activity;
                routeCalculator = RouteCalculator.Instance;
                SetNativeControl (mapView);

                //Watercolorstyle map with labels layer
                mapView.SetTileSource (new XYTileSource ("OSM", null, 0, 18, 256, ".png",
                                                         new[] {"http://c.tile.stamen.com/watercolor/"}));
                MapTileProviderBasic tileProvider = new MapTileProviderBasic (activity);
                ITileSource tileSource = new XYTileSource ("MyCustomTiles", null, 1, 16, 256, ".png",
                                                           new[] {"http://b.sm.mapstack.stamen.com/(watercolor,streets-and-labels)/"});
                tileProvider.TileSource = (tileSource);
                TilesOverlay tilesOverlay = new TilesOverlay (tileProvider, activity.BaseContext);
                tilesOverlay.LoadingBackgroundColor = Color.Transparent;
                mapView.OverlayManager.Add (tilesOverlay);
                mapView.SetMultiTouchControls (true);
                mapView.TilesScaledToDpi = true;

                mapController = (MapController) mapView.Controller;
                mapController.SetCenter (new GeoPoint (AppSharedData.PaderbornCenter.Latitude, AppSharedData.PaderbornCenter.Longitude));
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
                osmMap.GpsLocationChanged += NewElementOnGpsLocationChanged;
                NewElementOnGpsLocationChanged (e.NewElement.GpsLocation);
                osmMap.ExhibitSetChanged += NewElementOnExhibitSetChanged;
                NewElementOnExhibitSetChanged (e.NewElement.ExhibitSet);
                osmMap.DetailsRouteChanged += NewElementOnDetailsRouteChanged;
                NewElementOnDetailsRouteChanged (osmMap.DetailsRoute);

                e.NewElement.CenterLocationCalled += CenterMap;
            }
        }

        /// <summary>
        /// Called from center Button in navigation
        /// </summary>
        /// <param name="location"></param>
        private void CenterMap (GeoLocation location)
        {
            if (location != null)
                mapController.SetCenter (new GeoPoint (location.Latitude, location.Longitude));
            else
            {
                mapController.SetCenter (new GeoPoint (AppSharedData.PaderbornCenter.Latitude, AppSharedData.PaderbornCenter.Longitude));
            }
        }

        /// <summary>
        /// Displays all markers on Mainmap and updates it if set changes
        /// </summary>
        /// <param name="set"></param>
        private void NewElementOnExhibitSetChanged (ExhibitSet set)
        {
            SetMainScreenMarkers (set);
        }

        /// <summary>
        /// Everything regarding location changes is handled here
        /// </summary>
        /// <param name="gpsLocation">Location from Gps</param>
        private void NewElementOnGpsLocationChanged (GeoLocation gpsLocation)
        {
            //Userposition is always updated and shown if position is available
            if (gpsLocation != null)
            {
                var userPosition = new GeoPoint (gpsLocation.Latitude, gpsLocation.Longitude);
                if (userMarkerPosition != null)
                    mapView.OverlayManager.Remove (userMarkerPosition);


                userMarkerPosition = new Marker (mapView);
                userMarkerPosition.SetIcon (ResourcesCompat.GetDrawable (Resources, Resource.Drawable.ic_my_location, null));
                userMarkerPosition.Position = userPosition;
                userMarkerPosition.SetInfoWindow (null);
                mapView.OverlayManager.Add (userMarkerPosition);
                mapView.Invalidate ();
            }

            //if navigation is enabled the route will be drawn and updated
            if (osmMap.ShowNavigation)
                CalculateRoute (gpsLocation);
            else if (osmMap.ShowDetailsRoute)
                DrawDetailsRoute (gpsLocation);
        }

        /// <summary>
        /// Draws the markers in navigation
        /// </summary>
        /// <param name="route">Route to process</param>
        private void NewElementOnDetailsRouteChanged (Route route)
        {
            //The direct polyline is only draw if related bool is true


            if ((route != null) && route.Waypoints.Any ())
            {
                var markerInfoWindow = new ViaPointInfoWindow (Resource.Layout.navigation_info_window, mapView, activity);
                var mapMarkerIcon = ContextCompat.GetDrawable (activity, Resource.Drawable.marker_blue);
                var setMarker = new SetMarker (mapView, markerInfoWindow);

                foreach (var waypoint in route.Waypoints)
                {
                    //Bubbles
                    var geoPoint = new GeoPoint (waypoint.Exhibit.Location.Latitude, waypoint.Exhibit.Location.Longitude);
                    var bubble = setMarker.AddMarker (null, waypoint.Exhibit.Name, waypoint.Exhibit.Description, geoPoint, mapMarkerIcon, waypoint.Exhibit.Id);
                    mapView.OverlayManager.Add (bubble);
                }
                mapView.Invalidate ();
            }
        }

        /// <summary>
        /// Here the direct route in routedetails is calculated
        /// </summary>
        /// <param name="location"></param>
        private void DrawDetailsRoute (GeoLocation location)
        {
            var myPath = new PathOverlay (Resources.GetColor (Resource.Color.colorPrimaryDark), 7, new DefaultResourceProxyImpl (activity));
            if (location != null)
                myPath.AddPoint (new GeoPoint (location.Latitude, location.Longitude));

            if ((osmMap.DetailsRoute != null) && osmMap.DetailsRoute.Waypoints.Any ())
                foreach (var waypoint in osmMap.DetailsRoute.Waypoints)
                {
                    myPath.AddPoint (new GeoPoint (waypoint.Location.Latitude, waypoint.Location.Longitude));
                }


            mapView.OverlayManager.Add (myPath);
            mapView.Invalidate ();
        }

        //here all Markers for the Exhibits in the Main Map are set
        //and some general stuff
        private void SetMainScreenMarkers (ExhibitSet set)
        {
            locationOverlay = new MyLocationOverlay (activity, mapView);
            var compassOverlay = new CompassOverlay (activity, mapView);
            compassOverlay.EnableCompass ();
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

        /// <summary>
        /// Calculates the navigation route in a seperate thread
        /// and throws error message if no route was found
        /// </summary>
        /// <param name="userPosition">position of the user</param>
        private void CalculateRoute (GeoLocation userPosition)
        {
            var id = osmMap.DetailsRoute.Id;

            ThreadPool.QueueUserWorkItem (state => {
                                              var geoPoints = new List<GeoPoint> ();

                                              if (userPosition != null)
                                                  geoPoints.Add (new GeoPoint (userPosition.Latitude, userPosition.Longitude));


                                              Action action;

                                              try
                                              {
                                                  OrderedRoute locations = routeCalculator.CreateOrderedRoute (id, userPosition);

                                                  /*foreach (var w in locations)
                                                  {
                                                      var point = new GeoPoint (w.Latitude, w.Longitude);
                                                      geoPoints.Add (point);
                                                  }*/

                                                  action = () => DrawRoute (locations, userPosition != null);
                                              }
                                              catch (Exception)
                                              {
                                                  action = () => { };
                                              }

                                              activity.RunOnUiThread (() => {
                                                                          var handler = new Handler ();
                                                                          handler.PostDelayed (action, 0);
                                                                      });
                                          });
        }

        /// <summary>
        /// Settings how the line should look like
        /// </summary>
        /// <param name="geoPoints"></param>
        private void DrawRoute (List<GeoPoint> geoPoints)
        {
            if (disposed)
                return;

            //Cleanup route if drawn before
            if (currentRouteOverlay != null)
                mapView.OverlayManager.Remove (currentRouteOverlay);

            currentRouteOverlay = new Polyline (activity)
            {
                Title = osmMap.DetailsRoute.Title,
                Width = 5f,
                Color = Color.Blue,
                Points = geoPoints,
                Geodesic = true
            };
            mapView.OverlayManager.Add (currentRouteOverlay);
            mapView.Invalidate ();
        }

        private void DrawRoute (OrderedRoute route, bool userLocationIncluded)
        {
            if (disposed)
                return;

            //Cleanup route if drawn before
            if (currentRouteOverlay != null)
                mapView.OverlayManager.Remove (currentRouteOverlay);
            if (currentSectionOverlay != null)
            {
                mapView.OverlayManager.Remove (currentSectionOverlay);
            }

            var resources = IoCManager.Resolve<ApplicationResourcesProvider>();
            if (userLocationIncluded)
            {
                currentSectionOverlay = new Polyline (activity)
                {
                    Title = osmMap.DetailsRoute.Title,
                    Width = 5f,
                    Color = ((Xamarin.Forms.Color)resources.GetResourceValue("AccentColor")).ToAndroid (),
                    //Color = Color.Orange,
                    Points = route.FirstSection.Select (geoLocation => new GeoPoint (geoLocation.Latitude, geoLocation.Longitude)).ToList (),
                    Geodesic = true
                };
                currentRouteOverlay = new Polyline (activity)
                {
                    Title = osmMap.DetailsRoute.Title,
                    Width = 5f,
                    Color = ((Xamarin.Forms.Color)resources.GetResourceValue("PrimaryColor")).ToAndroid (),
                    //Color = Color.Blue,
                    Points = route.NonFirstSections.Select (geoLocation => new GeoPoint (geoLocation.Latitude, geoLocation.Longitude)).ToList (),
                    Geodesic = true
                };
            }
            else
            {
                currentRouteOverlay = new Polyline (activity)
                {
                    Title = osmMap.DetailsRoute.Title,
                    Width = 5f,

                    Color = ((Xamarin.Forms.Color)resources.GetResourceValue("PrimaryColor")).ToAndroid(),
                    Points = route.Locations.Select(geoLocation => new GeoPoint(geoLocation.Latitude, geoLocation.Longitude)).ToList(),

                    Geodesic = true
                };
            }
            int index = mapView.OverlayManager.IndexOf (locationOverlay);
            if (userLocationIncluded)
            {
                mapView.OverlayManager.Add (index, currentSectionOverlay);
            }
            mapView.OverlayManager.Add (index, currentRouteOverlay);
            mapView.Invalidate ();
        }

        private bool disposed;

        /// <summary>
        ///     Called when this view will be disposed.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose (bool disposing)
        {
            if (disposing)
            {
                disposed = true;

                osmMap.ExhibitSetChanged -= NewElementOnExhibitSetChanged;
                osmMap.GpsLocationChanged -= NewElementOnGpsLocationChanged;
                osmMap.DetailsRouteChanged -= NewElementOnDetailsRouteChanged;
            }
            base.Dispose (disposing);
        }

    }
}