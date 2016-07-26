//using System.Collections.Generic;
//using Android.App;
//using Android.OS;
//using Android.Support.V4.Content.Res;
//using Android.Widget;
//using de.upb.hip.mobile.droid.Helpers;
//using de.upb.hip.mobile.droid.Listeners;
//using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
//using de.upb.hip.mobile.pcl.BusinessLayer.Models;
//using Org.Osmdroid.Bonuspack.Overlays;
//using Org.Osmdroid.Bonuspack.Routing;
//using Org.Osmdroid.Tileprovider;
//using Org.Osmdroid.Tileprovider.Tilesource;
//using Org.Osmdroid.Util;
//using Org.Osmdroid.Views;
//using Org.Osmdroid.Views.Overlay;

//namespace de.upb.hip.mobile.droid.Activities
//{
//    [Activity(Theme = "@style/AppTheme", Label = "RouteNavigationActivity")]
//    public class RouteNavigationActivity : Activity
//    {

//        public const string IntentRoute = "route";
//        private const string SavedstateTrackingMode = "tracking_mode";
//        private GeoPoint geoLocation;
//        protected ExtendedLocationListener GpsTracker;

//        protected MapView MapView;
//        protected float AzimuthAngleSpeed = 90.0f;
//        private DirectedLocationOverlay locationOverlay;
//        protected bool TrackingMode;
//        protected Button TrackingModeButton;
//        private ProgressDialog progressDialog;
//        private Route route;

//        protected override void OnCreate(Bundle savedInstanceState)
//        {
//            base.OnCreate(savedInstanceState);
//            SetContentView(Resource.Layout.activity_route_navigation);


//            // getting location
//            GpsTracker = new ExtendedLocationListener(Application.Context);
//            geoLocation = new GeoPoint(GpsTracker.Latitude, GpsTracker.Longitude);

//            // TODO Remove this as soon as no needs to run in emulator
//            // set default coordinats for emulator
//            if (Build.Model.Contains("google_sdk") ||
//                Build.Model.Contains("Emulator") ||
//                Build.Model.Contains("Android SDK"))
//            {
//                geoLocation = new GeoPoint(ExtendedLocationListener.PADERBORN_HBF.Latitude,
//                                            ExtendedLocationListener.PADERBORN_HBF.Longitude);
//            }

//            if (geoLocation.Latitude == 0f && geoLocation.Longitude == 0f)
//                geoLocation = new GeoPoint(ExtendedLocationListener.PADERBORN_HBF.Latitude,
//                                            ExtendedLocationListener.PADERBORN_HBF.Longitude);


//            var extras = Intent.Extras;
//            var routeId = extras.GetString(IntentRoute);
//            route = RouteManager.GetRoute(routeId);

//            //Tracking system:
//            TrackingModeButton = (Button)FindViewById(Resource.Id.routeNavigationTrackingModeButton);
//            TrackingModeButton.Click += (sender, args) =>
//            {
//                TrackingMode = !TrackingMode;
//                UpdateUiWithTrackingMode();
//            };

//            if (savedInstanceState != null)
//            {
//                TrackingMode = savedInstanceState.GetBoolean(SavedstateTrackingMode);
//                UpdateUiWithTrackingMode();
//            }
//            else
//                TrackingMode = false;

//            SetUpMap();

//            ShowRoute();
//        }


//        private void SetUpMap()
//        {
//            // init progress dialog
//            progressDialog = new ProgressDialog(this);
//            progressDialog.SetCancelable(true);
//            locationOverlay = new DirectedLocationOverlay(this);

//            // getting the map
//            var genericMap = (GenericMapView)FindViewById(Resource.Id.routeNavigationMap);
//            var bitmapProvider = new MapTileProviderBasic(this);
//            genericMap.SetTileProvider(bitmapProvider);
//            MapView = genericMap.GetMapView();

//            MapView.Overlays.Add(locationOverlay);
//            MapView.SetBuiltInZoomControls(true);
//            MapView.SetMultiTouchControls(true);

//            MapView.SetTileSource(TileSourceFactory.Mapnik);
//            MapView.TilesScaledToDpi = true;
//            // mapView.SetMaxZoomLevel(RouteDetailsActivity.MAX_ZOOM_LEVEL);

//            // mMap prefs:
//            var mapController = MapView.Controller;
//            mapController.SetZoom(16);
//            mapController.SetCenter(geoLocation);
//        }

//        private void ShowRoute()
//        {
//            var policy = new StrictMode.ThreadPolicy.Builder().PermitAll().Build();
//            StrictMode.SetThreadPolicy(policy);


//            RoadManager roadManager = new MapQuestRoadManager("WRWdd9j02K8tGtERI2LtFiCLsRUKyJnJ");
//            roadManager.AddRequestOption("routeType=pedestrian");

//            var wayPoints = route.Waypoints;
//            var geoPoints = new List<GeoPoint>();

//            geoPoints.Add(new GeoPoint(geoLocation.Latitude, geoLocation.Longitude));

//            foreach (var w in wayPoints)
//            {
//                var point = new GeoPoint(w.Location.Latitude, w.Location.Longitude);
//                geoPoints.Add(point);
//            }


//            var road = roadManager.GetRoad(geoPoints);
//            if (road.MStatus != Road.StatusOk)
//                Toast.MakeText(Application.Context, "Error when loading the road - status=" + road.MStatus, ToastLength.Short).Show();

//            var roadOverlay = RoadManager.BuildRoadOverlay(road, Application.Context);
//            MapView.Overlays.Add(roadOverlay);


//            var roadMarkers = new FolderOverlay(Application.Context);
//            MapView.Overlays.Add(roadMarkers);
//            var nodeIcon = ResourcesCompat.GetDrawable(Resources, Resource.Drawable.marker_node, null);
//            for (var i = 0; i < road.MNodes.Count; i++)
//            {
//                var node = (RoadNode)road.MNodes[i];
//                var nodeMarker = new Marker(MapView);
//                nodeMarker.Position = node.MLocation;
//                nodeMarker.SetIcon(nodeIcon);


//                //4. Filling the bubbles
//                nodeMarker.Title = "Step " + i;
//                nodeMarker.Snippet = node.MInstructions;
//                nodeMarker.SubDescription = Road.GetLengthDurationText(node.MLength, node.MDuration);
//                var iconContinue = ResourcesCompat.GetDrawable(Resources, Resource.Drawable.ic_continue, null);
//                nodeMarker.Image = iconContinue;
//                //4. end

//                roadMarkers.Add(nodeMarker);
//            }
//        }

//        /**
//    * setup map orientation if tracking mode is on
//    */

//        private void UpdateUiWithTrackingMode()
//        {
//            if (TrackingMode)
//            {
//                TrackingModeButton.SetBackgroundResource(Resource.Drawable.btn_tracking_on);
//                if (locationOverlay.Enabled && locationOverlay.Location != null)
//                {
//                    MapView.Controller.AnimateTo(locationOverlay.Location);
//                }
//                MapView.MapOrientation = -AzimuthAngleSpeed;
//                TrackingModeButton.KeepScreenOn = true;
//            }
//            else
//            {
//                TrackingModeButton.SetBackgroundResource(Resource.Drawable.btn_tracking_off);
//                MapView.MapOrientation = 0.0f;
//                TrackingModeButton.KeepScreenOn = false;
//            }
//        }

//        protected override void OnSaveInstanceState(Bundle outState)
//        {
//            //outState.putParcelable(SAVEDSTATE_LOCATION, locationOverlay.getLocation());
//            outState.PutBoolean(SavedstateTrackingMode, TrackingMode);
//            //outState.putParcelable(SAVEDSTATE_START, mStartPoint);
//            //outState.putParcelable(SAVEDSTATE_DESTINATION, mDestinationPoint);
//            //outState.putParcelableArrayList(SAVEDSTATE_VIAPOINTS, mViaPoints);
//            //outState.putInt(SAVEDSTATE_REACHED_NODE, mReachedNode);
//            //outState.putInt(SAVEDSTATE_NEXT_NODE, mNextNode);
//            // outState.putInt(SAVEDSTATE_NEXT_VIA_POINT, mNextViaPoint);
//        }

//    }
//}

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


using System;
/**
* Create the route on the map and show step by step instruction for the navigation
*/
using System.Collections.Generic;
using System.Linq;
using Android.Annotation;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Hardware;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using de.upb.hip.mobile.droid;
using de.upb.hip.mobile.droid.Activities;
using de.upb.hip.mobile.droid.Helpers;
using de.upb.hip.mobile.droid.Listeners;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using Java.Lang;
using Org.Osmdroid.Api;
using Org.Osmdroid.Bonuspack.Location;
using Org.Osmdroid.Bonuspack.Overlays;
using Org.Osmdroid.Bonuspack.Routing;
using Org.Osmdroid.Tileprovider;
using Org.Osmdroid.Tileprovider.Tilesource;
using Org.Osmdroid.Util;
using Org.Osmdroid.Views;
using Org.Osmdroid.Views.Overlay;
using Marker = Org.Osmdroid.Bonuspack.Overlays.Marker;
using Polyline = Org.Osmdroid.Bonuspack.Overlays.Polyline;

namespace de.upb.hip.mobile.droid.Activities {
    [Activity (Theme = "@style/AppTheme", Label = "RouteNavigationActivity")]
    public class RouteNavigationActivity : Activity, ILocationListener, ISensorEventListener, IMapEventsReceiver {

        public const string INTENT_ROUTE = "route";


        //Constants for saving the instance state
        public const string SAVEDSTATE_LOCATION = "location";
        public const string SAVEDSTATE_TRACKING_MODE = "tracking_mode";
        public const string SAVEDSTATE_START = "start";
        public const string SAVEDSTATE_DESTINATION = "destination";
        public const string SAVEDSTATE_VIAPOINTS = "viapoints";
        public const string SAVEDSTATE_REACHED_NODE = "mReachedNode";
        public const string SAVEDSTATE_NEXT_NODE = "mNextNode";
        public const string SAVEDSTATE_NEXT_VIA_POINT = "mNextViaPoint";


        protected const int ROUTE_REJECT = 25; //in meters
        protected const string PROX_ALERT = "de.upb.hip.mobile.activities.PROX_ALERT";
        protected const long POINT_RADIUS = 5; // in Meters
        protected const long PROX_ALERT_EXPIRATION = -1; //indicate no expiration
        protected Road[] mRoads;

        protected MapView mMap;
        //protected SetMarker mMarker;
        protected GeoPoint mStartPoint;
        protected List<ViaPointData> mViaPoints;
        protected FolderOverlay mItineraryMarkers;
        //protected ViaPointInfoWindow mViaPointInfoWindow;
        protected DirectedLocationOverlay mLocationOverlay;
        protected Polygon mDestinationPolygon; //enclosing polygon of destination location
        protected Polyline[] mRoadOverlays;
        protected FolderOverlay mRoadNodeMarkers;
        protected Button TrackingModeButton;
        protected bool TrackingMode;
        protected float mAzimuthAngleSpeed = 0.0f;
        protected int mSelectedRoad;
        protected int mReachedNode = -1;
        protected int mNextNode = 0;
        protected int mNextViaPoint = 0;
        // setup the UI before the reaching the start point only once
        protected bool mUpdateStartPointOnce = true;
        // for location, location manager and setting dialog if no location
        protected ExtendedLocationListener mGpsTracker;
        // for the recalculation the route
        protected int mDistanceBetweenLoc = -1;
        // shows creation and loading route and map steps
        protected ProgressDialog mProgressDialog;
        // for the detecting that location is reached
        protected BroadcastReceiver mProximityIntentReceiver;
        protected PendingIntent mProximityIntent;

        private Route route;

        protected override void OnCreate (Bundle savedInstanceState)
        {
            base.OnCreate (savedInstanceState);
            SetContentView (Resource.Layout.activity_route_navigation);

            // init progress dialog
            mProgressDialog = new ProgressDialog (this);
            mProgressDialog.SetCancelable (true);

            //Get the Rout from NavigationDetailsActivity
            var extras = Intent.Extras;
            var routeId = extras.GetString (INTENT_ROUTE);
            route = RouteManager.GetRoute (routeId);

            // getting the map
            GenericMapView genericMap = (GenericMapView) FindViewById (Resource.Id.routeNavigationMap);
            MapTileProviderBasic bitmapProvider = new MapTileProviderBasic (this);
            genericMap.SetTileProvider (bitmapProvider);
            mMap = genericMap.GetMapView ();
            mMap.SetBuiltInZoomControls (true);
            mMap.SetMultiTouchControls (true);
            mMap.SetTileSource (TileSourceFactory.Mapnik);
            mMap.TilesScaledToDpi = true;
            //mMap.SetMaxZoomLevel(RouteDetailsActivity.MAX_ZOOM_LEVEL);

            // getting location
            mGpsTracker = new ExtendedLocationListener (Application.Context);
            GeoPoint geoLocation = new GeoPoint (mGpsTracker.Latitude, mGpsTracker.Longitude);

            // TODO Remove this as soon as no needs to run in emulator
            // set default coordinats for emulator
            if (Build.Model.Contains ("google_sdk") ||
                Build.Model.Contains ("Emulator") ||
                Build.Model.Contains ("Android SDK"))
            {
                geoLocation = new GeoPoint (ExtendedLocationListener.PADERBORN_HBF.Latitude,
                                            ExtendedLocationListener.PADERBORN_HBF.Longitude);
            }


            // mMap prefs:
            IMapController mapController = mMap.Controller;
            mapController.SetZoom (RouteDetailsActivity.ZOOM_LEVEL);
            mapController.SetCenter (geoLocation);

            // Itinerary markers:
            mItineraryMarkers = new FolderOverlay (this);
            mItineraryMarkers.Name = (Resource.String.itinerary_markers_title).ToString ();
            mMap.Overlays.Add (mItineraryMarkers);
            /*mViaPointInfoWindow = new ViaPointInfoWindow(R.layout.navigation_info_window, mMap, this);
            mMarker = new SetMarker(mMap, mItineraryMarkers, mViaPointInfoWindow);*/
            mLocationOverlay = new DirectedLocationOverlay (this);
            mMap.Overlays.Add (mLocationOverlay);

            var policy = new StrictMode.ThreadPolicy.Builder ().PermitAll ().Build ();
            StrictMode.SetThreadPolicy (policy);

            GenerateRoute (geoLocation);

           /* if (savedInstanceState == null)
            {
                GetRoadAsync (0);
            }
            else
            {
                mLocationOverlay.Location = ((GeoPoint) savedInstanceState
                    .GetParcelable (SAVEDSTATE_LOCATION));
                mStartPoint = (GeoPoint) savedInstanceState.GetParcelable (SAVEDSTATE_START);
                //mDestinationPoint = savedInstanceState.getParcelable(SAVEDSTATE_DESTINATION);
                //mViaPoints = savedInstanceState.getParcelableArrayList(SAVEDSTATE_VIAPOINTS);
                mReachedNode = savedInstanceState.GetInt (SAVEDSTATE_REACHED_NODE);
                mNextNode = savedInstanceState.GetInt (SAVEDSTATE_NEXT_NODE);
                mNextViaPoint = savedInstanceState.GetInt (SAVEDSTATE_NEXT_VIA_POINT);
            }*/

            // calculate distance between current location and start point
            // if the start location was not reached
            mDistanceBetweenLoc = geoLocation.DistanceTo (mStartPoint);

            // UpdateUIWithItineraryMarkers ();

            //Add the POIs around the starting point of the map
            if (route.Waypoints.Count > 0)
            {
                AddPoIsToMap (mMap, new GeoPoint (route.Waypoints.ElementAt (0).Location.Latitude,
                                                  route.Waypoints.ElementAt (0).Location.Longitude));
            }

            // a scale bar in the top-left corner of the screen
            ScaleBarOverlay scaleBarOverlay = new ScaleBarOverlay (mMap.Context);
            mMap.Overlays.Add (scaleBarOverlay);

            //Tracking system:
            TrackingModeButton = (Button) FindViewById (Resource.Id.routeNavigationTrackingModeButton);
            TrackingModeButton.Click += (sender, args) => {
                TrackingMode = !TrackingMode;
                UpdateUiWithTrackingMode ();
            };

            if (savedInstanceState != null)
            {
                TrackingMode = savedInstanceState.GetBoolean (SAVEDSTATE_TRACKING_MODE);
                UpdateUiWithTrackingMode ();
            }
            else
                TrackingMode = false;

            mRoadNodeMarkers = new FolderOverlay (this);
            //mRoadNodeMarkers.setName("Route Steps");
            mMap.Overlays.Add (mRoadNodeMarkers);

            if (savedInstanceState != null)
            {
                UpdateUiWithRoads (mRoads);
            }
        }

        private void GenerateRoute (GeoPoint geoLocation)
        {
            RoadManager roadManager = new MapQuestRoadManager("WRWdd9j02K8tGtERI2LtFiCLsRUKyJnJ");
            roadManager.AddRequestOption("routeType=pedestrian");

            var wayPoints = route.Waypoints;
            var geoPoints = new List<GeoPoint>();

            geoPoints.Add(new GeoPoint(geoLocation.Latitude, geoLocation.Longitude));

            foreach (var w in wayPoints)
            {
                var point = new GeoPoint(w.Location.Latitude, w.Location.Longitude);
                geoPoints.Add(point);
            }


            var road = roadManager.GetRoad(geoPoints);
            if (road.MStatus != Road.StatusOk)
                Toast.MakeText(Application.Context, "Error when loading the road - status=" + road.MStatus, ToastLength.Short).Show();

            var roadOverlay = RoadManager.BuildRoadOverlay(road, Application.Context);
            mMap.Overlays.Add(roadOverlay);

            // init start
            mStartPoint = geoLocation;

            // add viapoints
            mViaPoints = new List<ViaPointData>();

            for (int i = 0; i < route.Waypoints.Count; i++)
            {
                GeoPoint geoPoint = new GeoPoint(route.Waypoints.ElementAt(i).Location.Latitude,
                                                  route.Waypoints.ElementAt(i).Location.Longitude);

                var viaPointsData = BusinessEntitiyFactory.CreateBusinessObject<ViaPointData>();
                var position = BusinessEntitiyFactory.CreateBusinessObject<GeoLocation>();
                // add related data to marker if start point is first waypoint
                if (route.Waypoints.ElementAt(i).Exhibit.Id != null)
                {
                    Exhibit exhibit = route.Waypoints.ElementAt(i).Exhibit;

                    position.Longitude = geoPoint.Longitude;
                    position.Latitude = geoPoint.Latitude;

                    viaPointsData.Location = position;
                    viaPointsData.Title = exhibit.Name;
                    viaPointsData.Description = exhibit.Description;
                    viaPointsData.ExhibitId = exhibit.Id;
                }
                else
                {
                    if (i == route.Waypoints.Count - 1)
                    {
                        position.Longitude = geoPoint.Longitude;
                        position.Latitude = geoPoint.Latitude;

                        viaPointsData.Location = position;
                        viaPointsData.Title = Resources.GetString(Resource.String.destination);
                        viaPointsData.Description = "";
                        viaPointsData.ExhibitId = "-1";
                    }
                    else
                    {
                        position.Longitude = geoPoint.Longitude;
                        position.Latitude = geoPoint.Latitude;

                        viaPointsData.Location = position;
                        viaPointsData.Title = Resources.GetString(Resource.String.via_point);
                        viaPointsData.Description = "";
                        viaPointsData.ExhibitId = "-1";
                    }
                }
                mViaPoints.Add(viaPointsData);
            }
        }

        //TODO is not async not sure if still necessary
        private void AddPoIsToMap (MapView map, GeoPoint position)
        {
            NominatimPOIProvider poiProvider = new NominatimPOIProvider ();


            IList<POI> pois = poiProvider.GetPOICloseTo (position, "restaurant", 50, 0.05);

            FolderOverlay poiMarkers = new FolderOverlay (Application.Context);

            map.Overlays.Add (poiMarkers);

            Drawable poiIcon = ContextCompat.GetDrawable (this, Resource.Drawable.map_restaurant);
            foreach (POI poi in pois)
            {
                Marker poiMarker = new Marker (map);
                poiMarker.Title = (poi.MType);
                poiMarker.Snippet = (poi.MDescription);
                poiMarker.Position = (poi.MLocation);
                poiMarker.SetIcon (poiIcon);
                /*if (poi.mThumbnail != null){
                        poiItem.setImage(new BitmapDrawable(poi.mThumbnail));
                    }*/
                //  poiMarker.SetInfoWindow (new CustomInfoWindow (map));
                poiMarker.RelatedObject = (poi);
                poiMarkers.Add (poiMarker);
            }
        }



        /**
     * LocationListener implementation
     */


        public void OnLocationChanged (Location location)
        {
            GeoPoint newLocation = new GeoPoint (location);

            if (!mLocationOverlay.Enabled)
            {
                //we get the location for the first time:
                mLocationOverlay.Enabled = (true);
                mMap.Controller.AnimateTo (newLocation);
            }

            if (mReachedNode == mNextNode)
            {
                mNextNode += 1;
                SetNextStepToAlert (newLocation);
            }
            else
                if (mReachedNode == -1 && mUpdateStartPointOnce)
                {
                    // start node is not reached, update only once
                    SetNextStepToAlert (newLocation);
                }

            RecalculateRoute (newLocation);

            GeoPoint prevLocation = mLocationOverlay.Location;
            mLocationOverlay.Location = (newLocation);
            mLocationOverlay.SetAccuracy ((int) location.Accuracy);

            GeoPoint nextNearestLocation = GetNextNodeLocation ();
            if (nextNearestLocation != null && prevLocation != null)
            {
                if (location.Provider.Equals (LocationManager.GpsProvider))
                {
                    // setup next node(location) to the north
                    mAzimuthAngleSpeed = (float) prevLocation.BearingTo (nextNearestLocation);
                    mLocationOverlay.SetBearing (mAzimuthAngleSpeed);
                }
            }

            if (TrackingMode)
            {
                //keep the mMap view centered on current location:
                mMap.Controller.AnimateTo (newLocation);
                mMap.MapOrientation = (-mAzimuthAngleSpeed);
            }
            else
            {
                //just redraw the location overlay:
                mMap.Invalidate ();
            }
        }

        /**
     * Returns the geopoint of next node
     */

        private GeoPoint GetNextNodeLocation ()
        {
            if (mRoads == null)
            {
                return null;
            }

            GeoPoint nextNodeLocation = new GeoPoint (0, 0);

            if (mNextNode < mRoads [0].MNodes.Count)
            {
                // find next point
                var node = (RoadNode) mRoads [0].MNodes [mNextNode];
                nextNodeLocation = node.MLocation;
            }
            else
            {
                nextNodeLocation = new GeoPoint (mViaPoints.ElementAt (mViaPoints.Count - 1).Location.Latitude, mViaPoints.ElementAt (mViaPoints.Count - 1).Location.Longitude);
            }
            return nextNodeLocation;
        }

        /**
     * Recalculate the route if
     * distance between current location and next node >= 'ROUTE_REJECT' (20m)
     * distance between reached node and and next node
     */

        private void RecalculateRoute (GeoPoint currentLoc)
        {
            GeoPoint nextLoc = GetNextNodeLocation ();
            if (nextLoc == null)
            {
                return;
            }

            // distance from current loc to next node
            int distFromCurrent = currentLoc.DistanceTo (nextLoc);

            // update distance info on imageview
            UpdateDistanceInfo ((distFromCurrent).ToString () + " m");

            // check if difference >= 'ROUTE_REJECT' (20m)
            if ((distFromCurrent - mDistanceBetweenLoc) >= ROUTE_REJECT)
            {
                // set to default
                mReachedNode = -1;
                mNextNode = 0;
                mNextViaPoint = 0;
                mStartPoint = currentLoc;

                // get route
                GetRoadAsync (mNextViaPoint);
                // update markers on the map
                //  UpdateUIWithItineraryMarkers ();

                if (mProgressDialog.IsShowing)
                {
                    mProgressDialog.Dismiss ();
                }
            }
        }

        /**
     * Setup next node for the proximity alert
     * set instruction maneuver and distance in imageview
     */

        private void SetNextStepToAlert (GeoPoint geo)
        {
            if (mRoads == null || mReachedNode >= mRoads [0].MNodes.Count)
            {
                return;
            }

            // setup the nearest point from current point to ProximityAlert
            if (mReachedNode == -1)
            {
                var node = (RoadNode) mRoads [0].MNodes [mNextNode];

                GeoPoint nextLocation = new GeoPoint (node.MLocation.Latitude, node.MLocation.Longitude);
                AddProximityAlert (nextLocation.Latitude, nextLocation.Longitude);
                int distToStartLoc = geo.DistanceTo (nextLocation);

                DrawStepInfo (ContextCompat.GetDrawable (this, Resource.Drawable.marker_departure), Resources.GetString (Resource.String.start_point), distToStartLoc + " m");

                mUpdateStartPointOnce = false;
                mProgressDialog.Dismiss ();

                return;
            }

            // getting direction icon depending on maneuver

            TypedArray iconIds = Resources.ObtainTypedArray (Resource.Array.direction_icons);
            var n = (RoadNode) mRoads [0].MNodes [mReachedNode];
            int iconId = iconIds.GetResourceId (n.MManeuverType, Resource.Drawable.ic_empty);
            Drawable image = ContextCompat.GetDrawable (this, iconId);
            if (iconId != Resource.Drawable.ic_empty)
            {
                image = ContextCompat.GetDrawable (this, iconId);
            }

            // getting info from the current step to next.
            string instructions = "";

            n = (RoadNode) mRoads [0].MNodes [mReachedNode];
            if (n.MInstructions != null)
            {
                instructions = n.MInstructions;
            }
            string length = Integer.ValueOf ((int) (n.MLength * 1000)) + " m";
            DrawStepInfo (image, instructions, length);

            int type = -1;
            for (int iLeg = 0; iLeg < mRoads [0].MLegs.Count; iLeg++)
            {
                var l = (RoadLeg) mRoads [0].MLegs [iLeg];
                int mStartNodeIndex = l.MStartNodeIndex;
                int mEndNodeIndex = l.MEndNodeIndex;

                if (mReachedNode == mEndNodeIndex)
                {
                    mNextViaPoint += 1;
                    type = 0; // update via and node
                    break;
                }

                if (mReachedNode >= mStartNodeIndex && mReachedNode < mEndNodeIndex)
                {
                    mNextViaPoint = iLeg;
                    type = 1; // update node
                    break;
                }
            }

            // no via anymore --> destination point
            if (mNextViaPoint == mViaPoints.Count)
            {
                // delete last viaPoint from mapOverlay
                //UpdateUIWithItineraryMarkers(mNextViaPoint);
            }

            // no nodes anymore --> destination point
            if (mNextNode == mRoads [0].MNodes.Count)
            {
                // delete last node from mapOverlay
                UpdateRoadNodes (mRoads [0], mNextNode);

                // set distance between current node and destination point

                var n1 = (RoadNode) mRoads [0].MNodes [mReachedNode - 1];
                n = (RoadNode) mRoads [0].MNodes [mReachedNode];

                GeoPoint prevReachedNodeLoc = n1.MLocation;
                GeoPoint lastReachedNodeLoc = n.MLocation;
                mDistanceBetweenLoc = prevReachedNodeLoc.DistanceTo (lastReachedNodeLoc);

                // add alert for the last point
                AddProximityAlert (lastReachedNodeLoc.Latitude, lastReachedNodeLoc.Longitude);
            }

            switch (type)
            {
                case 0:
                    // update viaPoints and 1 node
                    //UpdateUIWithItineraryMarkers (mNextViaPoint);
                    break;

                case 1:
                    if (mReachedNode == 0)
                    {
                        //UpdateUIWithItineraryMarkers (mNextViaPoint);
                    }
                    // update nodes on map overlay
                    UpdateRoadNodes (mRoads [0], mNextNode);

                    // add alert for next location
                    n = (RoadNode) mRoads [0].MNodes [mNextNode];
                    GeoPoint nextNodeLocation = n.MLocation;
                    AddProximityAlert (nextNodeLocation.Latitude, nextNodeLocation.Longitude);


                    n = (RoadNode) mRoads [0].MNodes [mReachedNode];
                    // set new distance between current node and next node
                    GeoPoint reachedNodeLocation = n.MLocation;
                    mDistanceBetweenLoc = reachedNodeLocation.DistanceTo (nextNodeLocation);
                    break;
                default:
                    break;
            }

            if (mProgressDialog.IsShowing)
            {
                mProgressDialog.Dismiss ();
            }
        }

        /**
     * setup UI info about the route for the user
     */

        private void DrawStepInfo (Drawable drawable, string instructions, string length)
        {
            // set maneuver icon
            UpdateManeuverIcon (drawable);

            // set maneuver instruction
            UpdateInstructionInfo (instructions);

            // set maneuver distance
            UpdateDistanceInfo (length);
        }

        /**
     * update maneuver icon until the next step
     */

        private void UpdateManeuverIcon (Drawable drawable)
        {
            ImageView ivManeuverIcon = (ImageView) FindViewById (Resource.Id.routeNavigationManeuverIcon);
            ivManeuverIcon.SetImageBitmap (((BitmapDrawable) drawable).Bitmap);
        }

        /**
     * update textual instruction until the next step
     */
        //TODO calculate position for instruction if instruction too long for one line
        private void UpdateInstructionInfo (string instructions)
        {
            TextView ivManeuverInstruction = (TextView) FindViewById (Resource.Id.routeNavigationInstruction);
            ivManeuverInstruction.Text = instructions;
        }

        /**
     * update textual distance for user information
     */

        private void UpdateDistanceInfo (string length)
        {
            TextView ivManeuverDistance = (TextView) FindViewById (Resource.Id.routeNavigationDistance);
            ivManeuverDistance.Text = length;
        }

        /**
     * update all itinenary markers
     */

        /*public void UpdateUIWithItineraryMarkers ()
    {
        updateUIWithItineraryMarkers (0);
    }*/

        /**
     * update itinenary markers from specific one
     */

        /*public void UpdateUIWithItineraryMarkers (int iVia)
    {
        mItineraryMarkers.CloseAllInfoWindows ();
        mItineraryMarkers.Items.Clear();

        //Start marker:
        if (mStartPoint != null)
        {
            ViaPointData viaPointData = new ViaPointData ();
            viaPointData.Location.Latitude = mStartPoint.Latitude;
            viaPointData.Location.Longitude = mStartPoint.Longitude;
            viaPointData.Title = Resources.GetString (Resource.String.departure);
            viaPointData.Description = "";
            viaPointData.Id = "-1";

            if (mReachedNode >= 0)
            {
                // set start marker as visited if we reached the first node
                updateItineraryMarker (null, viaPointData, Resource.Drawable.marker_departure_visited);
            }
            else
            {
                updateItineraryMarker (null, viaPointData, Resource.Drawable.marker_departure);
            }
        }

        // update via markers before specific one as visited
        for (int index = 0; index < iVia; index++)
        {
            updateItineraryMarker (null, mViaPoints.ElementAt(index), Resource.Drawable.marker_via_visited);
        }
        // update via markers after specific one as non-visited
        for (int index = iVia; index < mViaPoints.Count - 1; index++)
        {
            updateItineraryMarker (null, mViaPoints.ElementAt(index), Resource.Drawable.marker_via);
        }

        // Destination marker: (as visited would be set
        updateItineraryMarker (null, mViaPoints.ElementAt (mViaPoints.Count - 1),
                               Resource.Drawable.marker_destination);

        if (mViaPoints.Count > 0 && mRoads != null)
        {
            if (mNextNode >= mRoads [0].MNodes.Count)
            {
                updateItineraryMarker (null, mViaPoints.ElementAt (mViaPoints.Count - 1),
                                       Resource.Drawable.marker_destination_visited);
            }
        }
    }*/

        /**
     * Update (or create if null) a marker in itineraryMarkers.
     */

        /* public Marker UpdateItineraryMarker (Marker marker, ViaPointData viaPointData, int markerResId)
     {
         Drawable icon = ContextCompat.GetDrawable (this, markerResId);
         Drawable drawable = null;

         Dictionary<string, int> data = new Dictionary<string, int> ( );
         data.put (viaPointData.Title, viaPointData.ExhibitId);

         if (viaPointData.ExhibitId > -1)
         {
             drawable = DBAdapter.getImage (viaPointData.getExhibitsId (), "image.jpg", 65);
         }

         marker = mMarker.addMarker (null, viaPointData.getTitle (), viaPointData.getDescription (),
                                     viaPointData.getGeoPoint (), drawable, icon, data);

         return marker;
     }*/

        /**
     * setup map orientation if tracking mode is on
     */

       private  void UpdateUiWithTrackingMode ()
        {
            if (TrackingMode)
            {
                TrackingModeButton.SetBackgroundResource (Resource.Drawable.btn_tracking_on);
                if (mLocationOverlay.Enabled && mLocationOverlay.Location != null)
                {
                    mMap.Controller.AnimateTo (mLocationOverlay.Location);
                }
                mMap.MapOrientation = (-mAzimuthAngleSpeed);
                TrackingModeButton.KeepScreenOn = (true);
            }
            else
            {
                TrackingModeButton.SetBackgroundResource (Resource.Drawable.btn_tracking_off);
                mMap.MapOrientation = (0.0f);
                TrackingModeButton.KeepScreenOn = (false);
            }
        }

        bool StartLocationUpdates ()
        {
            bool result = false;
            foreach (string provider in mGpsTracker.LocationManager.GetProviders (true))
            {
                mGpsTracker.LocationManager.RequestLocationUpdates (
                    provider,
                    ExtendedLocationListener.MIN_TIME_BW_UPDATES,
                    ExtendedLocationListener.MIN_DISTANCE_CHANGE_FOR_UPDATES,
                    this);
                result = true;
            }
            return result;
        }

        /**
     * show all nodes on map overlay
     */

        private void PutRoadNodes (Road road)
        {
            UpdateRoadNodes (road, 0);
        }

        /**
     * update nodes on map overlay from specific one
     */

        private void UpdateRoadNodes (Road road, int index)
        {
            mRoadNodeMarkers.Items.Clear ();
            Drawable icon = ContextCompat.GetDrawable (this, Resource.Drawable.marker_node);
            int n = road.MNodes.Count;

            /*MarkerInfoWindow infoWindow = new MarkerInfoWindow (
            Org.Osmdroid.Bonuspack.Resources.layout.bonuspack_bubble, mMap);*/
            TypedArray iconIds = Resources.ObtainTypedArray (Resource.Array.direction_icons);

            for (int i = index; i < n; i++)
            {
                RoadNode node = (RoadNode) road.MNodes [i];
                string instructions = (node.MInstructions == null ? "" : node.MInstructions);
                Marker nodeMarker = new Marker (mMap);
                nodeMarker.Title = (Resources.GetString (Resource.String.step) + " " + (i + 1))
                    ;
                nodeMarker.Snippet = (instructions);
                nodeMarker.SubDescription = (
                    Road.GetLengthDurationText (node.MLength, node.MDuration));
                nodeMarker.Position = (node.MLocation);
                nodeMarker.SetIcon (icon);
                // nodeMarker.SetInfoWindow(infoWindow); //use a shared infowindow.
                int iconId = iconIds.GetResourceId (node.MManeuverType, Resource.Drawable.ic_empty);
                if (iconId != Resource.Drawable.ic_empty)
                {
                    Drawable image = ContextCompat.GetDrawable (this, iconId);
                    nodeMarker.Image = (image);
                }
                mRoadNodeMarkers.Add (nodeMarker);
            }
            iconIds.Recycle ();
        }

        /**
     * paint road lines with colors blue or red
     */

        private void SelectRoad (int roadIndex)
        {
            mSelectedRoad = roadIndex;
            PutRoadNodes (mRoads [roadIndex]);

            for (int i = 0; i < mRoadOverlays.Length; i++)
            {
                Paint p = mRoadOverlays [i].Paint;
                if (i == roadIndex)
                    p.Color = (Resources.GetColor (Resource.Color.colorPrimaryDark)); //blue
                else
                    p.Color = (Resources.GetColor (Resource.Color.colorAccent)); // red
            }
            mMap.Invalidate ();
        }

       private void UpdateUiWithRoads (Road[] roads)
        {
            mRoadNodeMarkers.Items.Clear ();
            IList<Overlay> mapOverlays = mMap.Overlays;
            if (mRoadOverlays != null)
            {
                foreach (Polyline mRoadOverlay in mRoadOverlays)
                {
                    mapOverlays.Remove (mRoadOverlay);
                }
                mRoadOverlays = null;
            }
            if (roads == null || roads [0] == null)
            {
                return;
            }

            if (roads [0].MStatus == Road.StatusTechnicalIssue)
                Toast.MakeText (mMap.Context, Resource.String.technical_issue, ToastLength.Short).Show ();
            else
                if (roads [0].MStatus > Road.StatusTechnicalIssue) //functional issues
                    Toast.MakeText (mMap.Context, Resource.String.no_route, ToastLength.Short).Show ();

            mRoadOverlays = new Polyline[roads.Length];
            for (int i = 0; i < roads.Length; i++)
            {
                Polyline roadPolyline = RoadManager.BuildRoadOverlay (roads [i], this);
                mRoadOverlays [i] = roadPolyline;

                string routeDesc = roads [i].GetLengthDurationText (-1);
                roadPolyline.Title = (Resources.GetString (Resource.String.route) + " - " + routeDesc);
                roadPolyline.InfoWindow = (new BasicInfoWindow (Resource.Drawable.bonuspack_bubble, mMap));
                //roadPolyline.RelatedObject (i);
                mapOverlays.Add (roadPolyline);
                //we insert the road overlays at the "bottom", just above the MapEventsOverlay,
                //to avoid covering the other overlays.
            }
            SelectRoad (0);
        }


        /**
     * Gets a road in the background and notifies the listener when its ready
     *
     * @param index The first waypoint of the road
     */

        public void GetRoadAsync (int index)
        {
            mRoads = null;
            GeoPoint roadStartPoint = null;
            if (mStartPoint != null)
            {
                roadStartPoint = mStartPoint;
            }
            else
                if (mLocationOverlay.Enabled && mLocationOverlay.Location != null)
                {
                    //use my current location as itinerary start point:
                    roadStartPoint = mLocationOverlay.Location;
                }

            if (roadStartPoint == null)
            {
                UpdateUiWithRoads (null);
                UpdateUiWithPolygon (mViaPoints, "");
                return;
            }
            IList<GeoPoint> waypoints = new List<GeoPoint> ();
            waypoints.Add (roadStartPoint);

            //add intermediate via points:
            for (int i = index; i < mViaPoints.Count; i++)
            {
                waypoints.Add (new GeoPoint (mViaPoints.ElementAt (i).Location.Latitude, mViaPoints.ElementAt (i).Location.Longitude));
            }

            //waypoints.add(mDestinationPoint);
            //  new UpdateRoadTask().Execute(waypoints);
        }

        /**
     * add or replace the polygon overlay
     */

        public void UpdateUiWithPolygon (IList<ViaPointData> viaPoints, string name)
        {
            IList<Overlay> mapOverlays = mMap.Overlays;
            int location = -1;
            if (mDestinationPolygon != null)
                location = mapOverlays.IndexOf (mDestinationPolygon);
            mDestinationPolygon = new Polygon (this);
            mDestinationPolygon.FillColor = (0x15FF0080);
            // mDestinationPolygon.StrokeColor = (2147483903);
            mDestinationPolygon.StrokeWidth = (5.0f);
            mDestinationPolygon.Title = (name);

            IList<GeoPoint> polygon = new List<GeoPoint> ();
            foreach (ViaPointData viaPoint in viaPoints)
            {
                polygon.Add (new GeoPoint (viaPoint.Location.Latitude, viaPoint.Location.Longitude));
            }

            if (polygon.Count > 0)
            {
                mDestinationPolygon.Points = (polygon);
            }

            if (location != -1)
            {
                // mapOverlays.set(location, mDestinationPolygon);
            }
            else
            {
                mapOverlays.Insert (1, mDestinationPolygon); //insert just above the MapEventsOverlay.
            }

            mMap.Invalidate ();
        }

        /**
     * Adds an alert that triggers when a user is within a defined range of a specific coordinate
     *
     * @param latitude  the latitude of the alert point
     * @param longitude the longitude of the alert point
     */

        private void AddProximityAlert (double latitude, double longitude)
        {
            Intent intent = new Intent (PROX_ALERT);

            mProximityIntent = PendingIntent.GetBroadcast (this, 0, intent,
                                                           PendingIntentFlags.CancelCurrent);
            mGpsTracker.LocationManager.AddProximityAlert (
                // the latitude of the central point of the alert region
                latitude,
                // the longitude of the central point of the alert region
                longitude,
                // the radius of the central point of the alert region, in meters
                POINT_RADIUS,
                // time for this proximity alert, in milliseconds, or -1 to indicate no expiration
                PROX_ALERT_EXPIRATION,
                // will be used to generate an Intent to fire when entry to
                // or exit from the alert region is detected
                mProximityIntent
                );
        }

        /**
     * callback to store activity status before a restart (orientation change for instance)
     */

        protected override void OnSaveInstanceState (Bundle outState)
        {
            outState.PutParcelable (SAVEDSTATE_LOCATION, mLocationOverlay.Location);
            outState.PutBoolean (SAVEDSTATE_TRACKING_MODE, TrackingMode);
            outState.PutParcelable (SAVEDSTATE_START, mStartPoint);
            //outState.putParcelable(SAVEDSTATE_DESTINATION, mDestinationPoint);
            //outState.putParcelableArrayList(SAVEDSTATE_VIAPOINTS, mViaPoints);
            outState.PutInt (SAVEDSTATE_REACHED_NODE, mReachedNode);
            outState.PutInt (SAVEDSTATE_NEXT_NODE, mNextNode);
            outState.PutInt (SAVEDSTATE_NEXT_VIA_POINT, mNextViaPoint);
        }

        public void OnProviderDisabled (string provider)
        {
            throw new NotImplementedException ();
        }

        public void OnProviderEnabled (string provider)
        {
            throw new NotImplementedException ();
        }

        public void OnStatusChanged (string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            throw new NotImplementedException ();
        }

        public void OnAccuracyChanged (Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
            mLocationOverlay.SetAccuracy (2);
            //        mMap.invalidate();
        }

        public void OnSensorChanged (SensorEvent e)
        {
            throw new NotImplementedException ();
        }

        public bool LongPressHelper (GeoPoint p0)
        {
            return false;
        }

        public bool SingleTapConfirmedHelper (GeoPoint p0)
        {
            return false;
        }


        protected override void OnResume ()
        {
            bool isOneProviderEnabled = StartLocationUpdates ();
            mLocationOverlay.Enabled = (isOneProviderEnabled);

            IntentFilter filter = new IntentFilter (PROX_ALERT);
            RegisterReceiver (mProximityIntentReceiver, filter);

            base.OnResume ();
        }


        protected override void OnPause ()
        {
            if (mProgressDialog.IsShowing)
            {
                mProgressDialog.Dismiss ();
            }

            mGpsTracker.LocationManager.RemoveUpdates (this);
            UnregisterReceiver (mProximityIntentReceiver);

            base.OnPause ();
        }


        protected override void OnDestroy ()
        {
            base.OnDestroy ();
        }


        public override void OnBackPressed ()
        {
            Intent mIntent = new Intent ();
            mIntent.PutExtra ("onBackPressed", true);
            SetResult (Result.Ok, mIntent);

            base.OnBackPressed ();
        }

        //    /**
        //     * Async task to get the road in a separate thread.
        //     */

        //    private class UpdateRoadTask

        //    extends AsyncTask<Object, Void, Road

        //    []
        //    >

        //    {
        //    protected
        //        void onPreExecute
        //        ()
        //    {
        //        mProgressDialog.setMessage(getString(R.

        //        string.download_road))
        //        ;
        //        mProgressDialog.show();
        //    }

        //    protected
        //        Road[] doInBackground
        //        (Object...params)
        //    {
        //        @SuppressWarnings("unchecked")
        //            ArrayList<GeoPoint> waypoints = (ArrayList<GeoPoint>) params[
        //            0]
        //            ;
        //            RoadManager roadManager = new MapQuestRoadManager(getString(R.
        //            string.map_quest_key))
        //            ;
        //    roadManager.addRequestOption ("routeType=pedestrian");
        //            roadManager.addRequestOption ("locale=de_DE");

        //            return roadManager.getRoads (waypoints);
        //        }

        //protected
        //    void onPostExecute
        //    (Road[]
        //    result)
        //{
        //    mProgressDialog.setMessage(getString(R.

        //    string.creating_map))
        //    ;
        //    mRoads = result;
        //    if (mRoads != null)
        //    {
        //        updateUiWithRoads(result);

        //        // TODO Remove this as soon as no needs to run on emulator
        //        // needed to set route info and dismiss busy waiting dialog on emulator
        //        if (Build.MODEL.contains("google_sdk") ||
        //            Build.MODEL.contains("Emulator") ||
        //            Build.MODEL.contains("Android SDK"))
        //        {
        //            if (mRoads[0] != null && mRoads[0].mNodes != null)
        //            {
        //                setNextStepToAlert(mRoads[0].mNodes.get(0).mLocation);
        //            }
        //        }
        //    }
        //}
        //    }

        //    /**
        //     * Gets called when the user approaches a node
        //     */

        //    public class ProximityIntentReceiver

        //    extends BroadcastReceiver { @Override  public

        //    void onReceive(Context context, Intent intent)
        //{
        //    String key = LocationManager.KEY_PROXIMITY_ENTERING;
        //    Boolean entering = intent.getBooleanExtra(key, false);
        //    if (entering)
        //    {
        //        //We entered near a node, count up the reached nodes
        //        mReachedNode++;
        //        mGpsTracker.getLocationManager().removeProximityAlert(mProximityIntent);
        //    }
        //}
    }
}