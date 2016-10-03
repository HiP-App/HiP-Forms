/*
 * Copyright (C) 2016 History in Paderborn App - Universität Paderborn
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V4.Content.Res;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using de.upb.hip.mobile.droid.Adapters;
using de.upb.hip.mobile.droid.Helpers;
using de.upb.hip.mobile.droid.Listeners;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using de.upb.hip.mobile.pcl.Common;
using Org.Osmdroid.Bonuspack.Overlays;
using Org.Osmdroid.Bonuspack.Routing;
using Org.Osmdroid.Tileprovider;
using Org.Osmdroid.Tileprovider.Tilesource;
using Org.Osmdroid.Util;
using Org.Osmdroid.Views;
using Org.Osmdroid.Views.Overlay;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace de.upb.hip.mobile.droid.Activities {
    [Activity (Theme = "@style/AppTheme", Label = "RouteNavigationActivity")]
    public class RouteNavigationActivity : AppCompatActivity, ExtendedLocationListenerAdapter {//ILocationListener {

        public const string IntentRoute = "route";
        private double distanceWalked;
        private IList<GeoPoint> geoPoints;
        private GeoPoint gpsLocation;
        protected ExtendedLocationListener GpsTracker;
        private readonly int IndexNextWaypointNode = 1;

        private DirectedLocationOverlay locationOverlay;
        private MapController mapController;

        protected MapView MapView;
        private Marker position;
        private ProgressDialog progressDialog;
        private Road road;
        private RoadManager roadManager;
        private FolderOverlay roadMarkers;
        private Overlay roadOverlay;
        private Route route;
        protected Button TrackingModeButton;
        private IList<Waypoint> wayPoints;
        private readonly string LogId = "RouteNavigationActivity";


        public void LocationChanged(Location location)
        {
            var currentLocation = new GeoPoint(location);
            //calculate the distance walked from last positions
            distanceWalked = currentLocation.DistanceTo(gpsLocation);
            var tempNode = (RoadNode)road.MNodes[IndexNextWaypointNode];

            //if distance > 20 m new request to mapquest
            if (distanceWalked > 20)
            {
                UpdateRoute(currentLocation, tempNode);
            }
            else
            {
                UpdateInstructions(currentLocation, tempNode);
            }
            MapView.Invalidate();
        }

        protected override void OnCreate (Bundle savedInstanceState)
        {
            base.OnCreate (savedInstanceState);
            SetContentView (Resource.Layout.activity_route_navigation);
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);

            var toolbar = (Toolbar) FindViewById (Resource.Id.toolbar);
            SetSupportActionBar (toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled (true);
            SupportActionBar.Title = "Navigation";

            geoPoints = new List<GeoPoint> ();
            // getting location
            GpsTracker = ExtendedLocationListener.GetInstance();
            GpsTracker.SetContext(this);
            GpsTracker.SetExtendedLocationListenerAdapter(this);
            GpsTracker.EnableLocationUpdates();
            GpsTracker.EnableCheckForExhibits();
            gpsLocation = new GeoPoint(GpsTracker.GetLocation().Latitude, GpsTracker.GetLocation().Longitude);

            // TODO Remove this as soon as no needs to run in emulator
            // set default coordinats for emulator
            if (Build.Model.Contains ("google_sdk") ||
                Build.Model.Contains ("Emulator") ||
                Build.Model.Contains ("Android SDK"))
            {
                gpsLocation = new GeoPoint(AndroidConstants.PADERBORN_HBF.Latitude,
                                            AndroidConstants.PADERBORN_HBF.Longitude);
            }
            //catch if gps ist still zero
            if (gpsLocation.Latitude == 0f && gpsLocation.Longitude == 0f)
                gpsLocation = new GeoPoint(AndroidConstants.PADERBORN_HBF.Latitude,
                                            AndroidConstants.PADERBORN_HBF.Longitude);

            //Get the Route from RouteDetailsActivity
            var extras = Intent.Extras;
            var routeId = extras.GetString (IntentRoute);
            route = RouteManager.GetRoute (routeId);


            SetUpMap ();

            ShowRoute ();

            //Stuff for the tracking button
            TrackingModeButton = (Button) FindViewById (Resource.Id.routeNavigationTrackingModeButton);
            TrackingModeButton.Click += (sender, args) => { mapController.SetCenter (gpsLocation); };

            //Initiliazation of the Navigation
            var tempNode = (RoadNode) road.MNodes [0];
            distanceWalked = 0;

            //This Marker represents the position of the user
            position = new Marker (MapView);
            position.SetIcon (ResourcesCompat.GetDrawable (Resources, Resource.Drawable.thumb, null));
            position.Position = gpsLocation;
            position.SetInfoWindow (null);
            MapView.Overlays.Add (position);


            //Set start Infos
            var iconIds = Resources.ObtainTypedArray (Resource.Array.direction_icons);
            var iconId = iconIds.GetResourceId (tempNode.MManeuverType, Resource.Drawable.ic_empty);
            var image = ContextCompat.GetDrawable (this, iconId);

            /*FindViewById<TextView> (Resource.Id.routeNavigationInstruction).Text = tempNode.MInstructions;
            FindViewById<TextView> (Resource.Id.routeNavigationDistance).Text = Road.GetLengthDurationText (tempNode.MLength, tempNode.MDuration);
            var ivManeuverIcon = (ImageView)FindViewById(Resource.Id.routeNavigationManeuverIcon);
            ivManeuverIcon.SetImageBitmap(((BitmapDrawable)image).Bitmap);*/
        }


        private void SetUpMap ()
        {
            // init progress dialog
            progressDialog = new ProgressDialog (this);
            progressDialog.SetCancelable (true);
            locationOverlay = new DirectedLocationOverlay (this);

            // getting the map
            var genericMap = (GenericMapView) FindViewById (Resource.Id.routeNavigationMap);
            var bitmapProvider = new MapTileProviderBasic (this);
            genericMap.SetTileProvider (bitmapProvider);
            MapView = genericMap.GetMapView ();

            MapView.Overlays.Add (locationOverlay);
            MapView.SetBuiltInZoomControls (true);
            MapView.SetMultiTouchControls (true);

            MapView.SetTileSource (TileSourceFactory.Mapnik);
            MapView.TilesScaledToDpi = true;

            // mMap prefs:
            mapController = (MapController) MapView.Controller;
            mapController.SetZoom (16);
            mapController.SetCenter (gpsLocation);

            MapView.Invalidate ();
        }

        private void ShowRoute ()
        {
            var policy = new StrictMode.ThreadPolicy.Builder ().PermitAll ().Build ();
            StrictMode.SetThreadPolicy (policy);

            //Creates the RoadManager
            var key = KeyManager.Instance.GetKey ("mapquest");
            if (string.IsNullOrEmpty (key))
            {
                Log.Error (LogId, "Mapquest key is zero.");
            }
            roadManager = new MapQuestRoadManager (key);
            roadManager.AddRequestOption ("routeType=pedestrian");
            roadManager.AddRequestOption ("locale=de_DE");

            //Theses are the waypoints of the exhibits
            wayPoints = route.Waypoints;

            //Add current position to road
            geoPoints.Add (new GeoPoint (gpsLocation.Latitude, gpsLocation.Longitude));

            foreach (var w in wayPoints)
            {
                var point = new GeoPoint (w.Location.Latitude, w.Location.Longitude);
                geoPoints.Add (point);
            }

            //Get Road throw error if road not ok
            road = roadManager.GetRoad (geoPoints);
            if (road.MStatus != Road.StatusOk)
                Toast.MakeText (Application.Context, "Error when loading the road - status=" + road.MStatus, ToastLength.Short).Show ();

            roadOverlay = RoadManager.BuildRoadOverlay (road, Application.Context);
            MapView.Overlays.Add (roadOverlay);

            //Add bubbles
            var wayPointMarkers = new FolderOverlay (Application.Context);
            MapView.Overlays.Add (wayPointMarkers);
            var wayPointIcon = ResourcesCompat.GetDrawable (Resources, Resource.Drawable.marker_via, null);
            //add bubbles
            var markerInfoWindow = new ViaPointInfoWindow (Resource.Layout.navigation_info_window, MapView, this);
            var mapMarkerIcon = ContextCompat.GetDrawable (this, Resource.Drawable.marker_blue);
            var setMarker = new SetMarker (MapView, markerInfoWindow);

            for (var i = 0; i < route.Waypoints.Count; i++)
            {
                //Set different marker vor last waypoint
                if (i == route.Waypoints.Count - 1)
                    wayPointIcon = ResourcesCompat.GetDrawable (Resources, Resource.Drawable.marker_destination, null);


                var nodeMarker = setMarker.AddMarker (null, route.Waypoints.ElementAt (i).Exhibit.Name, route.Waypoints.ElementAt (i).Exhibit.Description,
                                                      new GeoPoint (route.Waypoints.ElementAt (i).Location.Latitude, route.Waypoints.ElementAt (i).Location.Longitude),
                                                      mapMarkerIcon, route.Waypoints.ElementAt (i).Exhibit.Id);
                nodeMarker.SetIcon (wayPointIcon);
                wayPointMarkers.Add (nodeMarker);
            }


            /* roadMarkers = new FolderOverlay (Application.Context);
            MapView.Overlays.Add (roadMarkers);
            var nodeIcon = ResourcesCompat.GetDrawable (Resources, Resource.Drawable.marker_node, null);
            for (var i = 0; i < road.MNodes.Count; i++)
            {
                var node = (RoadNode) road.MNodes [i];
                var nodeMarker = new Marker (MapView);
                nodeMarker.Position = node.MLocation;
                nodeMarker.SetIcon (nodeIcon);

                //roadMarkers.Add (nodeMarker);
            }*/
        }


        private void UpdateRoute (GeoPoint currentLocation, RoadNode currentNode)
        {
            //reset distance
            distanceWalked = 0;
            position.Position = currentLocation;

            /* geoPoints.RemoveAt (0);
            geoPoints.Insert (0, currentLocation);

            road = roadManager.GetRoad (geoPoints);
            MapView.Overlays.Remove (roadOverlay);
            roadOverlay = RoadManager.BuildRoadOverlay (road, Application.Context);
            MapView.Overlays.Add (roadOverlay);
            MapView.Invalidate ();


            position.Position = currentLocation;
            MapView.Overlays.Remove (position);
            MapView.Overlays.Remove (roadMarkers);
            MapView.Overlays.Add (position);

            roadMarkers = new FolderOverlay (Application.Context);

            var nodeIcon = ResourcesCompat.GetDrawable (Resources, Resource.Drawable.marker_node, null);
            for (var i = 1; i < road.MNodes.Count; i++)
            {
                var node = (RoadNode) road.MNodes [i];
                var nodeMarker = new Marker (MapView);
                nodeMarker.Position = node.MLocation;
                nodeMarker.SetIcon (nodeIcon);

                roadMarkers.Add (nodeMarker);
            }

            MapView.Overlays.Add (roadMarkers);


            currentNode = (RoadNode) road.MNodes [IndexNextWaypointNode - 1];

            var iconIds = Resources.ObtainTypedArray (Resource.Array.direction_icons);
            var iconId = iconIds.GetResourceId (currentNode.MManeuverType, Resource.Drawable.ic_empty);
            var image = ContextCompat.GetDrawable (this, iconId);

            FindViewById<TextView> (Resource.Id.routeNavigationInstruction).Text = currentNode.MInstructions;
            FindViewById<TextView> (Resource.Id.routeNavigationDistance).Text = Road.GetLengthDurationText (currentNode.MLength, currentNode.MDuration);
            var ivManeuverIcon = (ImageView) FindViewById (Resource.Id.routeNavigationManeuverIcon);
            ivManeuverIcon.SetImageBitmap (((BitmapDrawable) image).Bitmap);*/
            gpsLocation = currentLocation;
        }

        private void UpdateInstructions (GeoPoint currentLocation, RoadNode currentNode)
        {
            position.Position = currentLocation;
            MapView.Overlays.Remove (position);
            MapView.Overlays.Add (position);

            //if the distance to the nex roadnode < 10 than update instructions to next node 
            /* if (currentLocation.DistanceTo (currentNode.MLocation) <= 10)
            {
                var iconIds = Resources.ObtainTypedArray (Resource.Array.direction_icons);
                var iconId = iconIds.GetResourceId (currentNode.MManeuverType, Resource.Drawable.ic_empty);
                var image = ContextCompat.GetDrawable (this, iconId);

                //we are not far away from the next marker set new instruction
                FindViewById<TextView> (Resource.Id.routeNavigationInstruction).Text = currentNode.MInstructions;
                FindViewById<TextView> (Resource.Id.routeNavigationDistance).Text = Road.GetLengthDurationText (currentNode.MLength, currentNode.MDuration);
                var ivManeuverIcon = (ImageView) FindViewById (Resource.Id.routeNavigationManeuverIcon);
                ivManeuverIcon.SetImageBitmap (((BitmapDrawable) image).Bitmap);
            }*/
        }

        protected override void OnDestroy()
        {


            base.OnDestroy();
        }

        protected override void OnPause()
        {
            base.OnPause();
            GpsTracker.Unregister();

        }

        protected override void OnResume ()
        {
            base.OnResume ();
            GpsTracker = ExtendedLocationListener.GetInstance();
            GpsTracker.SetContext(this);
            GpsTracker.SetExtendedLocationListenerAdapter(this);
            GpsTracker.EnableLocationUpdates();
            GpsTracker.EnableCheckForExhibits();
            gpsLocation = new GeoPoint(GpsTracker.GetLocation().Latitude, GpsTracker.GetLocation().Longitude);

            // TODO Remove this as soon as no needs to run in emulator
            // set default coordinats for emulator
            if (Build.Model.Contains("google_sdk") ||
                Build.Model.Contains("Emulator") ||
                Build.Model.Contains("Android SDK"))
            {
                gpsLocation = new GeoPoint(AndroidConstants.PADERBORN_HBF.Latitude,
                                            AndroidConstants.PADERBORN_HBF.Longitude);
            }
            //catch if gps ist still zero
            if (gpsLocation.Latitude == 0f && gpsLocation.Longitude == 0f)
                gpsLocation = new GeoPoint(AndroidConstants.PADERBORN_HBF.Latitude,
                                            AndroidConstants.PADERBORN_HBF.Longitude);
        }

        public void OnProviderDisabled(string provider)
        {
            Toast.MakeText(this, "GPS Disabled",
                            ToastLength.Short).Show();
        }

        public void OnProviderEnabled(string provider)
        {
            Toast.MakeText(this, "GPS Enabled",
                            ToastLength.Short).Show();
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            switch (status)
            {
                case Availability.OutOfService:
                    Toast.MakeText(this, "Status Changed: Out of Service",
                                    ToastLength.Short).Show();
                    break;
                case Availability.TemporarilyUnavailable:
                    Toast.MakeText(this, "Status Changed: Temporarily Unavailable",
                                    ToastLength.Short).Show();
                    break;
                case Availability.Available:
                    Toast.MakeText(this, "Status Changed: Available",
                                    ToastLength.Short).Show();
                    break;
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId.Equals(Android.Resource.Id.Home))
            {
                SupportFinishAfterTransition();
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }
    }
}

#region androidport

/*
 * Copyright (C) 2016 History in Paderborn App - Universität Paderborn
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */


//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Android.App;
//using Android.Content;
//using Android.Graphics.Drawables;
//using Android.Hardware;
//using Android.Locations;
//using Android.OS;
//using Android.Runtime;
//using Android.Support.V4.Content;
//using Android.Widget;
//using de.upb.hip.mobile.droid.Helpers;
//using de.upb.hip.mobile.droid.Listeners;
//using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
//using de.upb.hip.mobile.pcl.BusinessLayer.Models;
//using Java.Lang;
//using Org.Osmdroid.Bonuspack.Location;
//using Org.Osmdroid.Bonuspack.Overlays;
//using Org.Osmdroid.Bonuspack.Routing;
//using Org.Osmdroid.Tileprovider;
//using Org.Osmdroid.Tileprovider.Tilesource;
//using Org.Osmdroid.Util;
//using Org.Osmdroid.Views;
//using Org.Osmdroid.Views.Overlay;
///**
//* Create the route on the map and show step by step instruction for the navigation
//*/

//namespace de.upb.hip.mobile.droid.Activities {
//    [Activity (Theme = "@style/AppTheme", Label = "RouteNavigationActivity")]
//    public class RouteNavigationActivity : Activity, ILocationListener, ISensorEventListener, IMapEventsReceiver {

//        public const string IntentRoute = "route";


//        //Constants for saving the instance state
//        public const string SavedstateLocation = "location";
//        public const string SavedstateTrackingMode = "tracking_mode";
//        public const string SavedstateStart = "start";
//        public const string SavedstateDestination = "destination";
//        public const string SavedstateViapoints = "viapoints";
//        public const string SavedstateReachedNode = "mReachedNode";
//        public const string SavedstateNextNode = "mNextNode";
//        public const string SavedstateNextViaPoint = "mNextViaPoint";


//        protected const int RouteReject = 25; //in meters
//        protected const string ProxAlert = "de.upb.hip.mobile.activities.PROX_ALERT";
//        protected const long PointRadius = 5; // in Meters
//        protected const long ProxAlertExpiration = -1; //indicate no expiration
//        protected float MAzimuthAngleSpeed;
//        protected Polygon MDestinationPolygon; //enclosing polygon of destination location
//        // for the recalculation the route
//        protected int MDistanceBetweenLoc = -1;
//        // for location, location manager and setting dialog if no location
//        protected ExtendedLocationListener MGpsTracker;
//        protected FolderOverlay MItineraryMarkers;
//        //protected ViaPointInfoWindow mViaPointInfoWindow;
//        protected DirectedLocationOverlay MLocationOverlay;

//        protected MapView MMap;
//        protected int MNextNodeIndex = 1;
//        protected int MNextViaPoint;
//        // shows creation and loading route and map steps
//        protected ProgressDialog MProgressDialog;
//        protected PendingIntent MProximityIntent;
//        // for the detecting that location is reached
//        protected BroadcastReceiver MProximityIntentReceiver;
//        protected int MCurrentNodeIndex = 0;
//        protected FolderOverlay MRoadNodeMarkers;
//        protected Polyline[] MRoadOverlays;
//        protected int MSelectedRoad;
//        //protected SetMarker mMarker;
//        protected GeoPoint MStartPoint;
//        // setup the UI before the reaching the start point only once
//        protected bool MUpdateStartPointOnce = true;
//        protected List<ViaPointData> MViaPoints;
//        protected Road[] MRoads;
//        private Route route;
//        protected bool TrackingMode;
//        protected Button TrackingModeButton;
//        private MapController MMapController;


//        /**
//     * LocationListener implementation
//     */


//        public void OnLocationChanged (Location location)
//        {
//            var newLocation = new GeoPoint (location);

//            if (!MLocationOverlay.Enabled)
//            {
//                //we get the location for the first time:
//                MLocationOverlay.Enabled = true;
//                MMap.Controller.AnimateTo (newLocation);
//            }

//            if (MCurrentNodeIndex == MNextNodeIndex)
//            {
//                MNextNodeIndex += 1;
//                SetNextStepToAlert (newLocation);
//            }


//            RecalculateRoute (newLocation);


//            if (TrackingMode)
//            {
//                //keep the mMap view centered on current location:
//               MMapController.SetCenter (new GeoPoint(location));
//            }
//            else
//            {
//                //just redraw the location overlay:
//                MMap.Invalidate ();
//            }
//        }

//        public void OnProviderDisabled (string provider)
//        {
//            throw new NotImplementedException ();
//        }

//        public void OnProviderEnabled (string provider)
//        {
//            throw new NotImplementedException ();
//        }

//        public void OnStatusChanged (string provider, [GeneratedEnum] Availability status, Bundle extras)
//        {
//            throw new NotImplementedException ();
//        }

//        public bool LongPressHelper (GeoPoint p0)
//        {
//            return false;
//        }

//        public bool SingleTapConfirmedHelper (GeoPoint p0)
//        {
//            return false;
//        }

//        public void OnAccuracyChanged (Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
//        {
//            MLocationOverlay.SetAccuracy (2);
//            //        mMap.invalidate();
//        }

//        public void OnSensorChanged (SensorEvent e)
//        {
//            throw new NotImplementedException ();
//        }

//        protected override void OnCreate (Bundle savedInstanceState)
//        {
//            base.OnCreate (savedInstanceState);
//            SetContentView (Resource.Layout.activity_route_navigation);


//            // getting location
//            MGpsTracker = new ExtendedLocationListener (Application.Context);
//            var geoLocation = new GeoPoint (MGpsTracker.Latitude, MGpsTracker.Longitude);

//            // TODO Remove this as soon as no needs to run in emulator
//            // set default coordinats for emulator
//            if (Build.Model.Contains ("google_sdk") ||
//                Build.Model.Contains ("Emulator") ||
//                Build.Model.Contains ("Android SDK"))
//            {
//                geoLocation = new GeoPoint (ExtendedLocationListener.PADERBORN_HBF.Latitude,
//                                            ExtendedLocationListener.PADERBORN_HBF.Longitude);
//            }


//            // init progress dialog
//            MProgressDialog = new ProgressDialog (this);
//            MProgressDialog.SetCancelable (true);

//            //Get the Route from NavigationDetailsActivity
//            var extras = Intent.Extras;
//            var routeId = extras.GetString (IntentRoute);
//            route = RouteManager.GetRoute (routeId);

//            // getting the map
//            var genericMap = (GenericMapView) FindViewById (Resource.Id.routeNavigationMap);
//            var bitmapProvider = new MapTileProviderBasic (this);
//            genericMap.SetTileProvider (bitmapProvider);
//            MMap = genericMap.GetMapView ();
//            MMap.SetBuiltInZoomControls (true);
//            MMap.SetMultiTouchControls (true);
//            MMap.SetTileSource (TileSourceFactory.Mapnik);
//            MMap.TilesScaledToDpi = true;
//            //mMap.SetMaxZoomLevel(RouteDetailsActivity.MAX_ZOOM_LEVEL);


//            // mMap prefs:
//           MMapController = (MapController) MMap.Controller;
//            MMapController.SetZoom (RouteDetailsActivity.ZOOM_LEVEL);
//            MMapController.SetCenter (geoLocation);

//            // Itinerary markers:
//            MItineraryMarkers = new FolderOverlay (this);
//            MItineraryMarkers.Name = Resource.String.itinerary_markers_title.ToString ();
//            MMap.Overlays.Add (MItineraryMarkers);
//            /*mViaPointInfoWindow = new ViaPointInfoWindow(R.layout.navigation_info_window, mMap, this);
//            mMarker = new SetMarker(mMap, mItineraryMarkers, mViaPointInfoWindow);*/
//            MLocationOverlay = new DirectedLocationOverlay (this);
//            MMap.Overlays.Add (MLocationOverlay);

//            var policy = new StrictMode.ThreadPolicy.Builder ().PermitAll ().Build ();
//            StrictMode.SetThreadPolicy (policy);

//            //Create startroute
//            GenerateRoute (geoLocation);

//            /* if (savedInstanceState == null)
//            {
//                GetRoadAsync (0);
//            }
//            else
//            {
//                mLocationOverlay.Location = ((GeoPoint) savedInstanceState
//                    .GetParcelable (SAVEDSTATE_LOCATION));
//                mStartPoint = (GeoPoint) savedInstanceState.GetParcelable (SAVEDSTATE_START);
//                //mDestinationPoint = savedInstanceState.getParcelable(SAVEDSTATE_DESTINATION);
//                //mViaPoints = savedInstanceState.getParcelableArrayList(SAVEDSTATE_VIAPOINTS);
//                mReachedNode = savedInstanceState.GetInt (SAVEDSTATE_REACHED_NODE);
//                mNextNode = savedInstanceState.GetInt (SAVEDSTATE_NEXT_NODE);
//                mNextViaPoint = savedInstanceState.GetInt (SAVEDSTATE_NEXT_VIA_POINT);
//            }*/

//            // calculate distance between current location and start point
//            // if the start location was not reached
//            MDistanceBetweenLoc = geoLocation.DistanceTo (MStartPoint);

//            // UpdateUIWithItineraryMarkers ();

//            //Add the POIs around the starting point of the map
//            if (route.Waypoints.Count > 0)
//            {
//                AddPoIsToMap (MMap, new GeoPoint (route.Waypoints.ElementAt (0).Location.Latitude,
//                                                  route.Waypoints.ElementAt (0).Location.Longitude));
//            }

//            // a scale bar in the top-left corner of the screen
//            var scaleBarOverlay = new ScaleBarOverlay (MMap.Context);
//            MMap.Overlays.Add (scaleBarOverlay);

//            //Tracking system:
//            TrackingModeButton = (Button) FindViewById (Resource.Id.routeNavigationTrackingModeButton);
//            TrackingModeButton.Click += (sender, args) => {
//                TrackingMode = !TrackingMode;
//                UpdateUiWithTrackingMode ();
//            };

//            if (savedInstanceState != null)
//            {
//                TrackingMode = savedInstanceState.GetBoolean (SavedstateTrackingMode);
//                UpdateUiWithTrackingMode ();
//            }
//            else
//                TrackingMode = false;

//            MRoadNodeMarkers = new FolderOverlay (this);
//            MMap.Overlays.Add (MRoadNodeMarkers);

//            /*if (savedInstanceState != null)
//            {
//                UpdateUiWithRoads (mRoads);
//            }*/
//        }

//        private void GenerateRoute (GeoPoint geoLocation)
//        {
//            RoadManager roadManager = new MapQuestRoadManager ("WRWdd9j02K8tGtERI2LtFiCLsRUKyJnJ");
//            roadManager.AddRequestOption ("routeType=pedestrian");

//            var wayPoints = route.Waypoints;
//            var geoPoints = new List<GeoPoint> ();

//            MStartPoint = geoLocation;

//            geoPoints.Add(new GeoPoint(geoLocation.Latitude, geoLocation.Longitude));

//            foreach (var w in wayPoints)
//            {
//                var point = new GeoPoint(w.Location.Latitude, w.Location.Longitude);
//                geoPoints.Add(point);
//            }


//            var road = roadManager.GetRoad(geoPoints);
//            if (road.MStatus != Road.StatusOk)
//                Toast.MakeText(Application.Context, "Error when loading the road - status=" + road.MStatus, ToastLength.Short).Show();


//            #region 
//            // add viapoints
//            /*MViaPoints = new List<ViaPointData> ();

//            for (var i = 0; i < route.Waypoints.Count; i++)
//            {
//                var geoPoint = new GeoPoint (route.Waypoints.ElementAt (i).Location.Latitude,
//                                             route.Waypoints.ElementAt (i).Location.Longitude);

//                var viaPointsData = BusinessEntitiyFactory.CreateBusinessObject<ViaPointData> ();
//                var position = BusinessEntitiyFactory.CreateBusinessObject<GeoLocation> ();
//                // add related data to marker if start point is first waypoint
//                if (route.Waypoints.ElementAt (i).Exhibit.Id != null)
//                {
//                    var exhibit = route.Waypoints.ElementAt (i).Exhibit;

//                    position.Longitude = geoPoint.Longitude;
//                    position.Latitude = geoPoint.Latitude;

//                    viaPointsData.Location = position;
//                    viaPointsData.Title = exhibit.Name;
//                    viaPointsData.Description = exhibit.Description;
//                    viaPointsData.ExhibitId = exhibit.Id;
//                }
//                else
//                {
//                    if (i == route.Waypoints.Count - 1)
//                    {
//                        position.Longitude = geoPoint.Longitude;
//                        position.Latitude = geoPoint.Latitude;

//                        viaPointsData.Location = position;
//                        viaPointsData.Title = Resources.GetString (Resource.String.destination);
//                        viaPointsData.Description = "";
//                        viaPointsData.ExhibitId = "-1";
//                    }
//                    else
//                    {
//                        position.Longitude = geoPoint.Longitude;
//                        position.Latitude = geoPoint.Latitude;

//                        viaPointsData.Location = position;
//                        viaPointsData.Title = Resources.GetString (Resource.String.via_point);
//                        viaPointsData.Description = "";
//                        viaPointsData.ExhibitId = "-1";
//                    }
//                }
//                MViaPoints.Add (viaPointsData);
//            }

//            geoPoints.Add (MStartPoint);

//            foreach (var w in MViaPoints)
//            {
//                var point = new GeoPoint (w.Location.Latitude, w.Location.Longitude);
//                geoPoints.Add (point);
//            }


//            var road = roadManager.GetRoad (geoPoints);
//            if (road.MStatus != Road.StatusOk)
//                Toast.MakeText (Application.Context, "Error when loading the road - status=" + road.MStatus, ToastLength.Short).Show ();*/
//            #endregion

//            var roadOverlay = RoadManager.BuildRoadOverlay (road, Application.Context);
//            MMap.Overlays.Add (roadOverlay);
//        }

//        //TODO is not async not sure if still necessary
//        private void AddPoIsToMap (MapView map, GeoPoint position)
//        {
//            var poiProvider = new NominatimPOIProvider ();


//            var pois = poiProvider.GetPOICloseTo (position, "restaurant", 50, 0.05);

//            var poiMarkers = new FolderOverlay (Application.Context);

//            map.Overlays.Add (poiMarkers);

//            var poiIcon = ContextCompat.GetDrawable (this, Resource.Drawable.map_restaurant);
//            foreach (var poi in pois)
//            {
//                var poiMarker = new Marker (map);
//                poiMarker.Title = poi.MType;
//                poiMarker.Snippet = poi.MDescription;
//                poiMarker.Position = poi.MLocation;
//                poiMarker.SetIcon (poiIcon);
//                /*if (poi.mThumbnail != null){
//                        poiItem.setImage(new BitmapDrawable(poi.mThumbnail));
//                    }*/
//                //  poiMarker.SetInfoWindow (new CustomInfoWindow (map));
//                poiMarker.RelatedObject = poi;
//                poiMarkers.Add (poiMarker);
//            }
//        }

//        /**
//     * Returns the geopoint of next node
//     */

//        private GeoPoint GetNextNodeLocation ()
//        {
//            if (route == null)
//            {
//                return null;
//            }

//            var nextNodeLocation = new GeoPoint (0, 0);

//            if (MNextNodeIndex < route.Waypoints.Count)
//            {
//                // find next point
//                var node = route.Waypoints.ElementAt (MNextNodeIndex);
//                nextNodeLocation = new GeoPoint(node.Location.Latitude,node.Location.Longitude);
//            }

//            return nextNodeLocation;
//        }

//        /**
//     * Recalculate the route if
//     * distance between current location and next node >= 'ROUTE_REJECT' (20m)
//     * distance between reached node and and next node
//     */

//        private void RecalculateRoute (GeoPoint currentLoc)
//        {
//            var nextLoc = GetNextNodeLocation ();

//            if (nextLoc == null)
//            {
//                //Make note that target is reached
//            }

//            // distance from current loc to next node
//            var distFromCurrent = currentLoc.DistanceTo (nextLoc);

//            // update distance info on imageview
//            UpdateDistanceInfo (distFromCurrent + " m");
//        }

//        /**
//     * Setup next node for the proximity alert
//     * set instruction maneuver and distance in imageview
//     */

//        private void SetNextStepToAlert (GeoPoint geo)
//        {
//            if (MRoads == null || MCurrentNodeIndex >= MRoads [0].MNodes.Count)
//            {
//                return;
//            }

//            // setup the nearest point from current point to ProximityAlert
//            if (MCurrentNodeIndex == -1)
//            {
//                var node = (RoadNode) MRoads [0].MNodes [MNextNodeIndex];

//                var nextLocation = new GeoPoint (node.MLocation.Latitude, node.MLocation.Longitude);
//                AddProximityAlert (nextLocation.Latitude, nextLocation.Longitude);
//                var distToStartLoc = geo.DistanceTo (nextLocation);

//                DrawStepInfo (ContextCompat.GetDrawable (this, Resource.Drawable.marker_departure), Resources.GetString (Resource.String.start_point), distToStartLoc + " m");

//                MUpdateStartPointOnce = false;
//                MProgressDialog.Dismiss ();

//                return;
//            }

//            // getting direction icon depending on maneuver

//            var iconIds = Resources.ObtainTypedArray (Resource.Array.direction_icons);
//            var n = (RoadNode) MRoads [0].MNodes [MCurrentNodeIndex];
//            var iconId = iconIds.GetResourceId (n.MManeuverType, Resource.Drawable.ic_empty);
//            var image = ContextCompat.GetDrawable (this, iconId);
//            if (iconId != Resource.Drawable.ic_empty)
//            {
//                image = ContextCompat.GetDrawable (this, iconId);
//            }

//            // getting info from the current step to next.
//            var instructions = "";

//            n = (RoadNode) MRoads [0].MNodes [MCurrentNodeIndex];
//            if (n.MInstructions != null)
//            {
//                instructions = n.MInstructions;
//            }
//            var length = Integer.ValueOf ((int) (n.MLength * 1000)) + " m";
//            DrawStepInfo (image, instructions, length);

//            var type = -1;
//            for (var iLeg = 0; iLeg < MRoads [0].MLegs.Count; iLeg++)
//            {
//                var l = (RoadLeg) MRoads [0].MLegs [iLeg];
//                var mStartNodeIndex = l.MStartNodeIndex;
//                var mEndNodeIndex = l.MEndNodeIndex;

//                if (MCurrentNodeIndex == mEndNodeIndex)
//                {
//                    MNextViaPoint += 1;
//                    type = 0; // update via and node
//                    break;
//                }

//                if (MCurrentNodeIndex >= mStartNodeIndex && MCurrentNodeIndex < mEndNodeIndex)
//                {
//                    MNextViaPoint = iLeg;
//                    type = 1; // update node
//                    break;
//                }
//            }

//            // no via anymore --> destination point
//            if (MNextViaPoint == MViaPoints.Count)
//            {
//                // delete last viaPoint from mapOverlay
//                //UpdateUIWithItineraryMarkers(mNextViaPoint);
//            }

//            // no nodes anymore --> destination point
//            if (MNextNodeIndex == MRoads [0].MNodes.Count)
//            {
//                // delete last node from mapOverlay
//                UpdateRoadNodes (MRoads [0], MNextNodeIndex);

//                // set distance between current node and destination point

//                var n1 = (RoadNode) MRoads [0].MNodes [MCurrentNodeIndex - 1];
//                n = (RoadNode) MRoads [0].MNodes [MCurrentNodeIndex];

//                var prevReachedNodeLoc = n1.MLocation;
//                var lastReachedNodeLoc = n.MLocation;
//                MDistanceBetweenLoc = prevReachedNodeLoc.DistanceTo (lastReachedNodeLoc);

//                // add alert for the last point
//                AddProximityAlert (lastReachedNodeLoc.Latitude, lastReachedNodeLoc.Longitude);
//            }

//            switch (type)
//            {
//                case 0:
//                    // update viaPoints and 1 node
//                    //UpdateUIWithItineraryMarkers (mNextViaPoint);
//                    break;

//                case 1:
//                    if (MCurrentNodeIndex == 0)
//                    {
//                        //UpdateUIWithItineraryMarkers (mNextViaPoint);
//                    }
//                    // update nodes on map overlay
//                    UpdateRoadNodes (MRoads [0], MNextNodeIndex);

//                    // add alert for next location
//                    n = (RoadNode) MRoads [0].MNodes [MNextNodeIndex];
//                    var nextNodeLocation = n.MLocation;
//                    AddProximityAlert (nextNodeLocation.Latitude, nextNodeLocation.Longitude);


//                    n = (RoadNode) MRoads [0].MNodes [MCurrentNodeIndex];
//                    // set new distance between current node and next node
//                    var reachedNodeLocation = n.MLocation;
//                    MDistanceBetweenLoc = reachedNodeLocation.DistanceTo (nextNodeLocation);
//                    break;
//                default:
//                    break;
//            }

//            if (MProgressDialog.IsShowing)
//            {
//                MProgressDialog.Dismiss ();
//            }
//        }

//        /**
//     * setup UI info about the route for the user
//     */

//        private void DrawStepInfo (Drawable drawable, string instructions, string length)
//        {
//            // set maneuver icon
//            UpdateManeuverIcon (drawable);

//            // set maneuver instruction
//            UpdateInstructionInfo (instructions);

//            // set maneuver distance
//            UpdateDistanceInfo (length);
//        }

//        /**
//     * update maneuver icon until the next step
//     */

//        private void UpdateManeuverIcon (Drawable drawable)
//        {
//            var ivManeuverIcon = (ImageView) FindViewById (Resource.Id.routeNavigationManeuverIcon);
//            ivManeuverIcon.SetImageBitmap (((BitmapDrawable) drawable).Bitmap);
//        }

//        /**
//     * update textual instruction until the next step
//     */
//        //TODO calculate position for instruction if instruction too long for one line
//        private void UpdateInstructionInfo (string instructions)
//        {
//            var ivManeuverInstruction = (TextView) FindViewById (Resource.Id.routeNavigationInstruction);
//            ivManeuverInstruction.Text = instructions;
//        }

//        /**
//     * update textual distance for user information
//     */

//        private void UpdateDistanceInfo (string length)
//        {
//            var ivManeuverDistance = (TextView) FindViewById (Resource.Id.routeNavigationDistance);
//            ivManeuverDistance.Text = length;
//        }

//        /**
//     * update all itinenary markers
//     */

//        /*public void UpdateUIWithItineraryMarkers ()
//    {
//        updateUIWithItineraryMarkers (0);
//    }*/

//        /**
//     * update itinenary markers from specific one
//     */

//        /*public void UpdateUIWithItineraryMarkers (int iVia)
//    {
//        mItineraryMarkers.CloseAllInfoWindows ();
//        mItineraryMarkers.Items.Clear();

//        //Start marker:
//        if (mStartPoint != null)
//        {
//            ViaPointData viaPointData = new ViaPointData ();
//            viaPointData.Location.Latitude = mStartPoint.Latitude;
//            viaPointData.Location.Longitude = mStartPoint.Longitude;
//            viaPointData.Title = Resources.GetString (Resource.String.departure);
//            viaPointData.Description = "";
//            viaPointData.Id = "-1";

//            if (mReachedNode >= 0)
//            {
//                // set start marker as visited if we reached the first node
//                updateItineraryMarker (null, viaPointData, Resource.Drawable.marker_departure_visited);
//            }
//            else
//            {
//                updateItineraryMarker (null, viaPointData, Resource.Drawable.marker_departure);
//            }
//        }

//        // update via markers before specific one as visited
//        for (int index = 0; index < iVia; index++)
//        {
//            updateItineraryMarker (null, mViaPoints.ElementAt(index), Resource.Drawable.marker_via_visited);
//        }
//        // update via markers after specific one as non-visited
//        for (int index = iVia; index < mViaPoints.Count - 1; index++)
//        {
//            updateItineraryMarker (null, mViaPoints.ElementAt(index), Resource.Drawable.marker_via);
//        }

//        // Destination marker: (as visited would be set
//        updateItineraryMarker (null, mViaPoints.ElementAt (mViaPoints.Count - 1),
//                               Resource.Drawable.marker_destination);

//        if (mViaPoints.Count > 0 && mRoads != null)
//        {
//            if (mNextNode >= mRoads [0].MNodes.Count)
//            {
//                updateItineraryMarker (null, mViaPoints.ElementAt (mViaPoints.Count - 1),
//                                       Resource.Drawable.marker_destination_visited);
//            }
//        }
//    }*/

//        /**
//     * Update (or create if null) a marker in itineraryMarkers.
//     */

//        /* public Marker UpdateItineraryMarker (Marker marker, ViaPointData viaPointData, int markerResId)
//     {
//         Drawable icon = ContextCompat.GetDrawable (this, markerResId);
//         Drawable drawable = null;

//         Dictionary<string, int> data = new Dictionary<string, int> ( );
//         data.put (viaPointData.Title, viaPointData.ExhibitId);

//         if (viaPointData.ExhibitId > -1)
//         {
//             drawable = DBAdapter.getImage (viaPointData.getExhibitsId (), "image.jpg", 65);
//         }

//         marker = mMarker.addMarker (null, viaPointData.getTitle (), viaPointData.getDescription (),
//                                     viaPointData.getGeoPoint (), drawable, icon, data);

//         return marker;
//     }*/

//        /**
//     * setup map orientation if tracking mode is on
//     */

//        private void UpdateUiWithTrackingMode ()
//        {
//            if (TrackingMode)
//            {
//                TrackingModeButton.SetBackgroundResource (Resource.Drawable.btn_tracking_on);
//                if (MLocationOverlay.Enabled && MLocationOverlay.Location != null)
//                {
//                    MMap.Controller.AnimateTo (MLocationOverlay.Location);
//                }
//                MMap.MapOrientation = -MAzimuthAngleSpeed;
//                TrackingModeButton.KeepScreenOn = true;
//            }
//            else
//            {
//                TrackingModeButton.SetBackgroundResource (Resource.Drawable.btn_tracking_off);
//                MMap.MapOrientation = 0.0f;
//                TrackingModeButton.KeepScreenOn = false;
//            }
//        }

//        private bool StartLocationUpdates ()
//        {
//            var result = false;
//            foreach (var provider in MGpsTracker.LocationManager.GetProviders (true))
//            {
//                MGpsTracker.LocationManager.RequestLocationUpdates (
//                    provider,
//                    ExtendedLocationListener.MinTimeBwUpdates,
//                    ExtendedLocationListener.MinDistanceChangeForUpdates,
//                    this);
//                result = true;
//            }
//            return result;
//        }

//        /**
//     * show all nodes on map overlay
//     */

//        private void PutRoadNodes (Road road)
//        {
//            UpdateRoadNodes (road, 0);
//        }

//        /**
//     * update nodes on map overlay from specific one
//     */

//        private void UpdateRoadNodes (Road road, int index)
//        {
//            MRoadNodeMarkers.Items.Clear ();
//            var icon = ContextCompat.GetDrawable (this, Resource.Drawable.marker_node);
//            var n = road.MNodes.Count;

//            /*MarkerInfoWindow infoWindow = new MarkerInfoWindow (
//            Org.Osmdroid.Bonuspack.Resources.layout.bonuspack_bubble, mMap);*/
//            var iconIds = Resources.ObtainTypedArray (Resource.Array.direction_icons);

//            for (var i = index; i < n; i++)
//            {
//                var node = (RoadNode) road.MNodes [i];
//                var instructions = node.MInstructions == null ? "" : node.MInstructions;
//                var nodeMarker = new Marker (MMap);
//                nodeMarker.Title = Resources.GetString (Resource.String.step) + " " + (i + 1)
//                    ;
//                nodeMarker.Snippet = instructions;
//                nodeMarker.SubDescription = Road.GetLengthDurationText (node.MLength, node.MDuration);
//                nodeMarker.Position = node.MLocation;
//                nodeMarker.SetIcon (icon);
//                // nodeMarker.SetInfoWindow(infoWindow); //use a shared infowindow.
//                var iconId = iconIds.GetResourceId (node.MManeuverType, Resource.Drawable.ic_empty);
//                if (iconId != Resource.Drawable.ic_empty)
//                {
//                    var image = ContextCompat.GetDrawable (this, iconId);
//                    nodeMarker.Image = image;
//                }
//                MRoadNodeMarkers.Add (nodeMarker);
//            }
//            iconIds.Recycle ();
//        }

//        /**
//     * paint road lines with colors blue or red
//     */

//        private void SelectRoad (int roadIndex)
//        {
//            MSelectedRoad = roadIndex;
//            PutRoadNodes (MRoads [roadIndex]);

//            for (var i = 0; i < MRoadOverlays.Length; i++)
//            {
//                var p = MRoadOverlays [i].Paint;
//                if (i == roadIndex)
//                    p.Color = Resources.GetColor (Resource.Color.colorPrimaryDark); //blue
//                else
//                    p.Color = Resources.GetColor (Resource.Color.colorAccent); // red
//            }
//            MMap.Invalidate ();
//        }

//        private void UpdateUiWithRoads (Road[] roads)
//        {
//            MRoadNodeMarkers.Items.Clear ();
//            var mapOverlays = MMap.Overlays;
//            if (MRoadOverlays != null)
//            {
//                foreach (var mRoadOverlay in MRoadOverlays)
//                {
//                    mapOverlays.Remove (mRoadOverlay);
//                }
//                MRoadOverlays = null;
//            }
//            if (roads == null || roads [0] == null)
//            {
//                return;
//            }

//            if (roads [0].MStatus == Road.StatusTechnicalIssue)
//                Toast.MakeText (MMap.Context, Resource.String.technical_issue, ToastLength.Short).Show ();
//            else
//                if (roads [0].MStatus > Road.StatusTechnicalIssue) //functional issues
//                    Toast.MakeText (MMap.Context, Resource.String.no_route, ToastLength.Short).Show ();

//            MRoadOverlays = new Polyline[roads.Length];
//            for (var i = 0; i < roads.Length; i++)
//            {
//                var roadPolyline = RoadManager.BuildRoadOverlay (roads [i], this);
//                MRoadOverlays [i] = roadPolyline;

//                var routeDesc = roads [i].GetLengthDurationText (-1);
//                roadPolyline.Title = Resources.GetString (Resource.String.route) + " - " + routeDesc;
//                roadPolyline.InfoWindow = new BasicInfoWindow (Resource.Drawable.bonuspack_bubble, MMap);
//                //roadPolyline.RelatedObject (i);
//                mapOverlays.Add (roadPolyline);
//                //we insert the road overlays at the "bottom", just above the MapEventsOverlay,
//                //to avoid covering the other overlays.
//            }
//            SelectRoad (0);
//        }


//        /**
//     * Gets a road in the background and notifies the listener when its ready
//     *
//     * @param index The first waypoint of the road
//     */

//        public void GetRoadAsync (int index)
//        {
//            MRoads = null;
//            GeoPoint roadStartPoint = null;
//            if (MStartPoint != null)
//            {
//                roadStartPoint = MStartPoint;
//            }
//            else
//                if (MLocationOverlay.Enabled && MLocationOverlay.Location != null)
//                {
//                    //use my current location as itinerary start point:
//                    roadStartPoint = MLocationOverlay.Location;
//                }

//            if (roadStartPoint == null)
//            {
//                UpdateUiWithRoads (null);
//                UpdateUiWithPolygon (MViaPoints, "");
//                return;
//            }
//            IList<GeoPoint> waypoints = new List<GeoPoint> ();
//            waypoints.Add (roadStartPoint);

//            //add intermediate via points:
//            for (var i = index; i < MViaPoints.Count; i++)
//            {
//                waypoints.Add (new GeoPoint (MViaPoints.ElementAt (i).Location.Latitude, MViaPoints.ElementAt (i).Location.Longitude));
//            }

//            //waypoints.add(mDestinationPoint);
//            //  new UpdateRoadTask().Execute(waypoints);
//        }

//        /**
//     * add or replace the polygon overlay
//     */

//        public void UpdateUiWithPolygon (IList<ViaPointData> viaPoints, string name)
//        {
//            var mapOverlays = MMap.Overlays;
//            var location = -1;
//            if (MDestinationPolygon != null)
//                location = mapOverlays.IndexOf (MDestinationPolygon);
//            MDestinationPolygon = new Polygon (this);
//            MDestinationPolygon.FillColor = 0x15FF0080;
//            // mDestinationPolygon.StrokeColor = (2147483903);
//            MDestinationPolygon.StrokeWidth = 5.0f;
//            MDestinationPolygon.Title = name;

//            IList<GeoPoint> polygon = new List<GeoPoint> ();
//            foreach (var viaPoint in viaPoints)
//            {
//                polygon.Add (new GeoPoint (viaPoint.Location.Latitude, viaPoint.Location.Longitude));
//            }

//            if (polygon.Count > 0)
//            {
//                MDestinationPolygon.Points = polygon;
//            }

//            if (location != -1)
//            {
//                // mapOverlays.set(location, mDestinationPolygon);
//            }
//            else
//            {
//                mapOverlays.Insert (1, MDestinationPolygon); //insert just above the MapEventsOverlay.
//            }

//            MMap.Invalidate ();
//        }

//        /**
//     * Adds an alert that triggers when a user is within a defined range of a specific coordinate
//     *
//     * @param latitude  the latitude of the alert point
//     * @param longitude the longitude of the alert point
//     */

//        private void AddProximityAlert (double latitude, double longitude)
//        {
//            var intent = new Intent (ProxAlert);

//            MProximityIntent = PendingIntent.GetBroadcast (this, 0, intent,
//                                                           PendingIntentFlags.CancelCurrent);
//            MGpsTracker.LocationManager.AddProximityAlert (
//                // the latitude of the central point of the alert region
//                latitude,
//                // the longitude of the central point of the alert region
//                longitude,
//                // the radius of the central point of the alert region, in meters
//                PointRadius,
//                // time for this proximity alert, in milliseconds, or -1 to indicate no expiration
//                ProxAlertExpiration,
//                // will be used to generate an Intent to fire when entry to
//                // or exit from the alert region is detected
//                MProximityIntent
//                );
//        }

//        /**
//     * callback to store activity status before a restart (orientation change for instance)
//     */

//        protected override void OnSaveInstanceState (Bundle outState)
//        {
//            outState.PutParcelable (SavedstateLocation, MLocationOverlay.Location);
//            outState.PutBoolean (SavedstateTrackingMode, TrackingMode);
//            outState.PutParcelable (SavedstateStart, MStartPoint);
//            //outState.putParcelable(SAVEDSTATE_DESTINATION, mDestinationPoint);
//            //outState.putParcelableArrayList(SAVEDSTATE_VIAPOINTS, mViaPoints);
//            outState.PutInt (SavedstateReachedNode, MCurrentNodeIndex);
//            outState.PutInt (SavedstateNextNode, MNextNodeIndex);
//            outState.PutInt (SavedstateNextViaPoint, MNextViaPoint);
//        }


//        protected override void OnResume ()
//        {
//            var isOneProviderEnabled = StartLocationUpdates ();
//            MLocationOverlay.Enabled = isOneProviderEnabled;

//            var filter = new IntentFilter (ProxAlert);
//            RegisterReceiver (MProximityIntentReceiver, filter);

//            base.OnResume ();
//        }


//        protected override void OnPause ()
//        {
//            if (MProgressDialog.IsShowing)
//            {
//                MProgressDialog.Dismiss ();
//            }

//            MGpsTracker.LocationManager.RemoveUpdates (this);
//            UnregisterReceiver (MProximityIntentReceiver);

//            base.OnPause ();
//        }


//        protected override void OnDestroy ()
//        {
//            base.OnDestroy ();
//        }


//        public override void OnBackPressed ()
//        {
//            var mIntent = new Intent ();
//            mIntent.PutExtra ("onBackPressed", true);
//            SetResult (Result.Ok, mIntent);

//            base.OnBackPressed ();
//        }

//        //    if (entering)
//        //    Boolean entering = intent.getBooleanExtra(key, false);
//        //    String key = LocationManager.KEY_PROXIMITY_ENTERING;
//        //{

//        //    void onReceive(Context context, Intent intent)

//        //    extends BroadcastReceiver { @Override  public

//        //    public class ProximityIntentReceiver
//        //     */
//        //     * Gets called when the user approaches a node

//        //    /**
//        //    }
//        //}
//        //    }
//        //        }
//        //            }
//        //                setNextStepToAlert(mRoads[0].mNodes.get(0).mLocation);
//        //            {
//        //            if (mRoads[0] != null && mRoads[0].mNodes != null)
//        //        {
//        //            Build.MODEL.contains("Android SDK"))
//        //            Build.MODEL.contains("Emulator") ||
//        //        if (Build.MODEL.contains("google_sdk") ||
//        //        // needed to set route info and dismiss busy waiting dialog on emulator

//        //        // TODO Remove this as soon as no needs to run on emulator
//        //        updateUiWithRoads(result);
//        //    {
//        //    if (mRoads != null)
//        //    mRoads = result;
//        //    ;

//        //    string.creating_map))
//        //    mProgressDialog.setMessage(getString(R.
//        //{
//        //    result)
//        //    (Road[]
//        //    void onPostExecute

//        //protected
//        //        }

//        //            return roadManager.getRoads (waypoints);
//        //            roadManager.addRequestOption ("locale=de_DE");
//        //    roadManager.addRequestOption ("routeType=pedestrian");
//        //            ;
//        //            string.map_quest_key))
//        //            RoadManager roadManager = new MapQuestRoadManager(getString(R.
//        //            ;
//        //            0]
//        //            ArrayList<GeoPoint> waypoints = (ArrayList<GeoPoint>) params[
//        //        @SuppressWarnings("unchecked")
//        //    {
//        //        (Object...params)
//        //        Road[] doInBackground

//        //    protected
//        //    }
//        //        mProgressDialog.show();
//        //        ;

//        //        string.download_road))
//        //        mProgressDialog.setMessage(getString(R.
//        //    {
//        //        ()
//        //        void onPreExecute
//        //    protected

//        //    {
//        //    >

//        //    []

//        //    extends AsyncTask<Object, Void, Road

//        //    private class UpdateRoadTask
//        //     */
//        //     * Async task to get the road in a separate thread.

//        //    /**
//        //    {
//        //        //We entered near a node, count up the reached nodes
//        //        mReachedNode++;
//        //        mGpsTracker.getLocationManager().removeProximityAlert(mProximityIntent);
//        //    }
//        //}
//    }
//}

#endregion