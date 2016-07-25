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

//namespace de.upb.hip.mobile.droid.Activities {
//    [Activity (Theme = "@style/AppTheme", Label = "RouteNavigationActivity")]
//    public class RouteNavigationActivity : Activity {

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

//        protected override void OnCreate (Bundle savedInstanceState)
//        {
//            base.OnCreate (savedInstanceState);
//            SetContentView (Resource.Layout.activity_route_navigation);


//            // getting location
//            GpsTracker = new ExtendedLocationListener (Application.Context);
//            geoLocation = new GeoPoint (GpsTracker.Latitude, GpsTracker.Longitude);

//            // TODO Remove this as soon as no needs to run in emulator
//            // set default coordinats for emulator
//            if (Build.Model.Contains ("google_sdk") ||
//                Build.Model.Contains ("Emulator") ||
//                Build.Model.Contains ("Android SDK"))
//            {
//                geoLocation = new GeoPoint (ExtendedLocationListener.PADERBORN_HBF.Latitude,
//                                            ExtendedLocationListener.PADERBORN_HBF.Longitude);
//            }

//            if (geoLocation.Latitude == 0f && geoLocation.Longitude == 0f)
//                geoLocation = new GeoPoint (ExtendedLocationListener.PADERBORN_HBF.Latitude,
//                                            ExtendedLocationListener.PADERBORN_HBF.Longitude);


//            var extras = Intent.Extras;
//            var routeId = extras.GetString (IntentRoute);
//            route = RouteManager.GetRoute (routeId);

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

//            SetUpMap ();

//            ShowRoute ();
//        }


//        private void SetUpMap ()
//        {
//            // init progress dialog
//            progressDialog = new ProgressDialog (this);
//            progressDialog.SetCancelable (true);
//            locationOverlay = new DirectedLocationOverlay (this);

//            // getting the map
//            var genericMap = (GenericMapView) FindViewById (Resource.Id.routeNavigationMap);
//            var bitmapProvider = new MapTileProviderBasic (this);
//            genericMap.SetTileProvider (bitmapProvider);
//            MapView = genericMap.GetMapView ();

//            MapView.Overlays.Add (locationOverlay);
//            MapView.SetBuiltInZoomControls (true);
//            MapView.SetMultiTouchControls (true);

//            MapView.SetTileSource (TileSourceFactory.Mapnik);
//            MapView.TilesScaledToDpi = true;
//            // mapView.SetMaxZoomLevel(RouteDetailsActivity.MAX_ZOOM_LEVEL);

//            // mMap prefs:
//            var mapController = MapView.Controller;
//            mapController.SetZoom (16);
//            mapController.SetCenter (geoLocation);
//        }

//        private void ShowRoute ()
//        {
//            var policy = new StrictMode.ThreadPolicy.Builder ().PermitAll ().Build ();
//            StrictMode.SetThreadPolicy (policy);


//            RoadManager roadManager = new MapQuestRoadManager ("WRWdd9j02K8tGtERI2LtFiCLsRUKyJnJ");
//            roadManager.AddRequestOption ("routeType=pedestrian");

//            var wayPoints = route.Waypoints;
//            var geoPoints = new List<GeoPoint> ();

//            geoPoints.Add (new GeoPoint (geoLocation.Latitude, geoLocation.Longitude));

//            foreach (var w in wayPoints)
//            {
//                var point = new GeoPoint (w.Location.Latitude, w.Location.Longitude);
//                geoPoints.Add (point);
//            }


//            var road = roadManager.GetRoad (geoPoints);
//            if (road.MStatus != Road.StatusOk)
//                Toast.MakeText (Application.Context, "Error when loading the road - status=" + road.MStatus, ToastLength.Short).Show ();

//            var roadOverlay = RoadManager.BuildRoadOverlay (road, Application.Context);
//            MapView.Overlays.Add (roadOverlay);


//            var roadMarkers = new FolderOverlay (Application.Context);
//            MapView.Overlays.Add (roadMarkers);
//            var nodeIcon = ResourcesCompat.GetDrawable (Resources, Resource.Drawable.marker_node, null);
//            for (var i = 0; i < road.MNodes.Count; i++)
//            {
//                var node = (RoadNode) road.MNodes [i];
//                var nodeMarker = new Marker (MapView);
//                nodeMarker.Position = node.MLocation;
//                nodeMarker.SetIcon (nodeIcon);


//                //4. Filling the bubbles
//                nodeMarker.Title = "Step " + i;
//                nodeMarker.Snippet = node.MInstructions;
//                nodeMarker.SubDescription = Road.GetLengthDurationText (node.MLength, node.MDuration);
//                var iconContinue = ResourcesCompat.GetDrawable (Resources, Resource.Drawable.ic_continue, null);
//                nodeMarker.Image = iconContinue;
//                //4. end

//                roadMarkers.Add (nodeMarker);
//            }
//        }

//        /**
//    * setup map orientation if tracking mode is on
//    */

//        private void UpdateUiWithTrackingMode ()
//        {
//            if (TrackingMode)
//            {
//                TrackingModeButton.SetBackgroundResource (Resource.Drawable.btn_tracking_on);
//                if (locationOverlay.Enabled && locationOverlay.Location != null)
//                {
//                    MapView.Controller.AnimateTo (locationOverlay.Location);
//                }
//                MapView.MapOrientation = -AzimuthAngleSpeed;
//                TrackingModeButton.KeepScreenOn = true;
//            }
//            else
//            {
//                TrackingModeButton.SetBackgroundResource (Resource.Drawable.btn_tracking_off);
//                MapView.MapOrientation = 0.0f;
//                TrackingModeButton.KeepScreenOn = false;
//            }
//        }

//        protected override void OnSaveInstanceState (Bundle outState)
//        {
//            //outState.putParcelable(SAVEDSTATE_LOCATION, locationOverlay.getLocation());
//            outState.PutBoolean (SavedstateTrackingMode, TrackingMode);
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
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using de.upb.hip.mobile.droid;
using de.upb.hip.mobile.droid.Activities;
using de.upb.hip.mobile.droid.Helpers;
using de.upb.hip.mobile.droid.Listeners;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using Java.Lang;
using Org.Osmdroid.Api;
using Org.Osmdroid.Bonuspack.Overlays;
using Org.Osmdroid.Bonuspack.Routing;
using Org.Osmdroid.Tileprovider;
using Org.Osmdroid.Tileprovider.Tilesource;
using Org.Osmdroid.Util;
using Org.Osmdroid.Views;
using Org.Osmdroid.Views.Overlay;
using Marker = Org.Osmdroid.Bonuspack.Overlays.Marker;
using Polyline = Org.Osmdroid.Bonuspack.Overlays.Polyline;

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
    protected IList<ViaPointData> mViaPoints;
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
    protected ProximityIntentReceiver mProximityIntentReceiver = new ProximityIntentReceiver ();
    protected PendingIntent mProximityIntent;

    private Route route;

    protected override void OnCreate (Bundle savedInstanceState)
    {
        base.OnCreate (savedInstanceState);
        SetContentView (Resource.Layout.activity_route_navigation);

        // init progress dialog
        mProgressDialog = new ProgressDialog (this);
        mProgressDialog.SetCancelable (true);

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


        RoadManager roadManager = new MapQuestRoadManager ("WRWdd9j02K8tGtERI2LtFiCLsRUKyJnJ");
        roadManager.AddRequestOption ("routeType=pedestrian");

        var wayPoints = route.Waypoints;
        var geoPoints = new List<GeoPoint> ();

        geoPoints.Add (new GeoPoint (geoLocation.Latitude, geoLocation.Longitude));

        foreach (var w in wayPoints)
        {
            var point = new GeoPoint (w.Location.Latitude, w.Location.Longitude);
            geoPoints.Add (point);
        }


        var road = roadManager.GetRoad (geoPoints);
        if (road.MStatus != Road.StatusOk)
            Toast.MakeText (Application.Context, "Error when loading the road - status=" + road.MStatus, ToastLength.Short).Show ();

        var roadOverlay = RoadManager.BuildRoadOverlay (road, Application.Context);
        mMap.Overlays.Add (roadOverlay);

        // init start
        mStartPoint = geoLocation;

        // add viapoints
        mViaPoints = new List<ViaPointData> ();
        for (int i = 0; i < route.Waypoints.Count; i++)
        {
            GeoPoint geoPoint = new GeoPoint (route.Waypoints.ElementAt (i).Location.Latitude ,
                                              route.Waypoints. ElementAt(i).Location.Longitude );

            ViaPointData viaPointsData = new ViaPointData ();
            // add related data to marker if start point is first waypoint
            if (route.Waypoints.ElementAt(i).Exhibit.Id != null)
            {
                Exhibit exhibit = route.Waypoints.ElementAt (i).Exhibit;

                viaPointsData.Location.Longitude = geoPoint.Longitude;
                viaPointsData.Location.Latitude = geoPoint.Latitude;
                viaPointsData.Title = exhibit.Name;
                viaPointsData.Description = exhibit.Description;
                viaPointsData.Id = exhibit.Id;

            }
            else
            {
                if (i == route.Waypoints.Count - 1)
                {
               


                    viaPointsData.Location.Longitude = geoPoint.Longitude;
                    viaPointsData.Location.Latitude = geoPoint.Latitude;
                    viaPointsData.Title = Resource.String.destination.ToString();
                    viaPointsData.Description = "";
                    viaPointsData.Id = "-1";
                }
                else
                {
                    viaPointsData.Location.Longitude = geoPoint.Longitude;
                    viaPointsData.Location.Latitude = geoPoint.Latitude;
                    viaPointsData.Title = Resource.String.via_point.ToString();
                    viaPointsData.Description = "";
                    viaPointsData.Id = "-1";
                }
            }
            mViaPoints.Add (viaPointsData);
        }

        if (savedInstanceState == null)
        {
            GetRoadAsync (0);
        }
        else
        {
            mLocationOverlay.Location = ((GeoPoint) savedInstanceState
                                              .GetParcelable (SAVEDSTATE_LOCATION));
            mStartPoint = (GeoPoint)savedInstanceState.GetParcelable (SAVEDSTATE_START);
            //mDestinationPoint = savedInstanceState.getParcelable(SAVEDSTATE_DESTINATION);
            //mViaPoints = savedInstanceState.getParcelableArrayList(SAVEDSTATE_VIAPOINTS);
            mReachedNode = savedInstanceState.GetInt (SAVEDSTATE_REACHED_NODE);
            mNextNode = savedInstanceState.GetInt (SAVEDSTATE_NEXT_NODE);
            mNextViaPoint = savedInstanceState.GetInt (SAVEDSTATE_NEXT_VIA_POINT);
        }

        // calculate distance between current location and start point
        // if the start location was not reached
        mDistanceBetweenLoc = geoLocation.DistanceTo (mStartPoint);

        UpdateUIWithItineraryMarkers ();

        //Add the POIs around the starting point of the map
        if (route.Waypoints.Count > 0)
        {
            AddPOIsToMap (mMap, new GeoPoint (route.Waypoints.ElementAt(0).Location.Latitude,
                                              route.Waypoints.ElementAt(0).Location.Longitude));
        }

        // a scale bar in the top-left corner of the screen
        ScaleBarOverlay scaleBarOverlay = new ScaleBarOverlay (mMap);
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
        mMap.Overlays .Add (mRoadNodeMarkers);

        if (savedInstanceState != null)
        {
            UpdateUiWithRoads (mRoads);
        }
    }

    private void AddPOIsToMap (MapView map,  GeoPoint position)
    {
//        new AsyncTask ()
//        {
//            @Override
//        protected Object doInBackground(Object[] params)
//{
//    NominatimPOIProvider poiProvider = new NominatimPOIProvider("Uni-Paderborn HiP App");
//    final ArrayList< POI > pois = poiProvider.getPOICloseTo(position,
//            "restaurant",
//            50,
//            0.05);

//    final FolderOverlay poiMarkers = new FolderOverlay(RouteNavigationActivity.this);
//    RouteNavigationActivity.this.runOnUiThread(new Runnable() {
//    @Override
//    public void run()
//{
//    map.getOverlays().add(poiMarkers);

//    Drawable poiIcon = getResources().getDrawable(R.drawable.map_restaurant);
//    for (POI poi : pois)
//{
//    Marker poiMarker = new Marker(map);
//    poiMarker.setTitle(poi.mType);
//    poiMarker.setSnippet(poi.mDescription);
//    poiMarker.setPosition(poi.mLocation);
//    poiMarker.setIcon(poiIcon);
//    /*if (poi.mThumbnail != null){
//            poiItem.setImage(new BitmapDrawable(poi.mThumbnail));
//        }*/
//    poiMarker.setInfoWindow(new CustomInfoWindow(map));
//    poiMarker.setRelatedObject(poi);
//    poiMarkers.add(poiMarker);
//        }
//    }
//    }
//    )
//        ;


//        return null;
//    }
//    }
//    .
//        execute ();
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

        recalculateRoute (newLocation);

        GeoPoint prevLocation = mLocationOverlay.Location;
        mLocationOverlay.Location =  (newLocation);
        mLocationOverlay.SetAccuracy ((int) location.Accuracy);

        GeoPoint nextNearestLocation = getNextNodeLocation ();
        if (nextNearestLocation != null && prevLocation != null)
        {
            if (location.Provider .Equals (LocationManager.GpsProvider))
            {
                // setup next node(location) to the north
                mAzimuthAngleSpeed = (float) prevLocation.BearingTo (nextNearestLocation);
                mLocationOverlay.SetBearing (mAzimuthAngleSpeed);
            }
        }

        if (TrackingMode)
        {
            //keep the mMap view centered on current location:
            mMap.Controller .AnimateTo (newLocation);
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

    private GeoPoint getNextNodeLocation ()
    {
        if (mRoads == null)
        {
            return null;
        }

        GeoPoint nextNodeLocation = new GeoPoint (0,0);

        if (mNextNode < mRoads [0].MNodes.Count)
        {
            // find next point
            var node = (RoadNode) mRoads [0].MNodes [mNextNode];
            nextNodeLocation = node.MLocation;
        }
        else
        {
            nextNodeLocation = new GeoPoint(mViaPoints.ElementAt(mViaPoints.Count - 1).Location.Latitude, mViaPoints.ElementAt(mViaPoints.Count - 1).Location.Longitude);



        }
        return nextNodeLocation;
    }

/**
 * Recalculate the route if
 * distance between current location and next node >= 'ROUTE_REJECT' (20m)
 * distance between reached node and and next node
 */

    private void recalculateRoute (GeoPoint currentLoc)
    {
        GeoPoint nextLoc = getNextNodeLocation ();
        if (nextLoc == null)
        {
            return;
        }

        // distance from current loc to next node
        int distFromCurrent = currentLoc.DistanceTo (nextLoc);

        // update distance info on imageview
        updateDistanceInfo  ((distFromCurrent).ToString() + " m");

        // check if difference >= 'ROUTE_REJECT' (20m)
        if ((distFromCurrent - mDistanceBetweenLoc) >= ROUTE_REJECT)
        {
            // set to default
            mReachedNode = -1;
            mNextNode = 0;
            mNextViaPoint = 0;
            mStartPoint = currentLoc;

            // get route
            getRoadAsync (mNextViaPoint);
            // update markers on the map
            updateUIWithItineraryMarkers ();

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
            var node = (RoadNode)mRoads[0].MNodes[mNextNode];

            GeoPoint nextLocation = new GeoPoint (node.MLocation.Latitude,node.MLocation.Longitude);
            addProximityAlert (nextLocation.Latitude , nextLocation.Longitude );
            int distToStartLoc = geo.DistanceTo (nextLocation);

            DrawStepInfo (ContextCompat.GetDrawable (this, Resource.Drawable.marker_departure),Resources.GetString (Resource.String.start_point),distToStartLoc + " m");

            mUpdateStartPointOnce = false;
            mProgressDialog.Dismiss ();

            return;
        }

        // getting direction icon depending on maneuver

        TypedArray iconIds = Resources.ObtainTypedArray (Resource.Array.direction_icons);
        var n = (RoadNode)mRoads [0].MNodes[mReachedNode];
        int iconId = iconIds.GetResourceId (n.MManeuverType,Resource.Drawable.ic_empty);
        Drawable image = ContextCompat.GetDrawable (this, iconId);
        if (iconId != Resource.Drawable.ic_empty)
        {
            image = ContextCompat.GetDrawable (this, iconId);
        }

        // getting info from the current step to next.
        string instructions = "";

        n = (RoadNode)mRoads[0].MNodes[mReachedNode];
        if (n.MInstructions != null)
        {
            instructions =n.MInstructions;
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
            updateUIWithItineraryMarkers (mNextViaPoint);
        }

        // no nodes anymore --> destination point
        if (mNextNode == mRoads [0].MNodes.Count)
        {
            // delete last node from mapOverlay
            updateRoadNodes (mRoads [0], mNextNode);

            // set distance between current node and destination point

            var n1 = (RoadNode)mRoads [0].MNodes [mReachedNode - 1];
            n = (RoadNode)mRoads[0].MNodes[mReachedNode];

            GeoPoint prevReachedNodeLoc = n1.MLocation;
            GeoPoint lastReachedNodeLoc = n.MLocation;
            mDistanceBetweenLoc = prevReachedNodeLoc.DistanceTo (lastReachedNodeLoc);

            // add alert for the last point
            addProximityAlert (lastReachedNodeLoc.Latitude, lastReachedNodeLoc.Longitude);
        }

        switch (type)
        {
            case 0:
                // update viaPoints and 1 node
                updateUIWithItineraryMarkers (mNextViaPoint);

            case 1:
                if (mReachedNode == 0)
                {
                    updateUIWithItineraryMarkers (mNextViaPoint);
                }
                // update nodes on map overlay
                updateRoadNodes (mRoads [0], mNextNode);

                // add alert for next location
                GeoPoint nextNodeLocation = mRoads [0].mNodes.get (mNextNode).mLocation;
                addProximityAlert (nextNodeLocation.getLatitude (), nextNodeLocation.getLongitude ());

                // set new distance between current node and next node
                GeoPoint reachedNodeLocation = mRoads [0].mNodes.get (mReachedNode).mLocation;
                mDistanceBetweenLoc = reachedNodeLocation.distanceTo (nextNodeLocation);
                break;
            default:
                break;
        }

        if (mProgressDialog.isShowing ())
        {
            mProgressDialog.dismiss ();
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

    public void UpdateUIWithItineraryMarkers ()
    {
        updateUIWithItineraryMarkers (0);
    }

/**
 * update itinenary markers from specific one
 */

    public void UpdateUIWithItineraryMarkers (int iVia)
    {
        mItineraryMarkers.CloseAllInfoWindows ();
        mItineraryMarkers.Items.Clear();

        //Start marker:
        if (mStartPoint != null)
        {
            ViaPointData viaPointData = new ViaPointData ();
            viaPointData.setViaPointData (mStartPoint,
                                          getResources ().getString (R.
            string.departure),
            "",
            -1)
            ;
            if (mReachedNode >= 0)
            {
                // set start marker as visited if we reached the first node
                updateItineraryMarker (null, viaPointData, R.drawable.marker_departure_visited);
            }
            else
            {
                updateItineraryMarker (null, viaPointData, R.drawable.marker_departure);
            }
        }

        // update via markers before specific one as visited
        for (int index = 0; index < iVia; index++)
        {
            updateItineraryMarker (null, mViaPoints.get (index), R.drawable.marker_via_visited);
        }
        // update via markers after specific one as non-visited
        for (int index = iVia; index < mViaPoints.size () - 1; index++)
        {
            updateItineraryMarker (null, mViaPoints.get (index), R.drawable.marker_via);
        }

        // Destination marker: (as visited would be set
        updateItineraryMarker (null, mViaPoints.get (mViaPoints.size () - 1),
                               R.drawable.marker_destination);

        if (mViaPoints.size () > 0 && mRoads != null)
        {
            if (mNextNode >= mRoads [0].mNodes.size ())
            {
                updateItineraryMarker (null, mViaPoints.get (mViaPoints.size () - 1),
                                       R.drawable.marker_destination_visited);
            }
        }
    }

/**
 * Update (or create if null) a marker in itineraryMarkers.
 */

    public Marker updateItineraryMarker (Marker marker, ViaPointData viaPointData, int markerResId)
    {
        Drawable icon = ContextCompat.getDrawable (this, markerResId);
        Drawable drawable = null;

        Map<String, Integer> data = new HashMap<> ();
        data.put (viaPointData.getTitle (), viaPointData.getExhibitsId ());

        if (viaPointData.getExhibitsId () > -1)
        {
            drawable = DBAdapter.getImage (viaPointData.getExhibitsId (), "image.jpg", 65);
        }

        marker = mMarker.addMarker (null, viaPointData.getTitle (), viaPointData.getDescription (),
                                    viaPointData.getGeoPoint (), drawable, icon, data);

        return marker;
    }

/**
 * setup map orientation if tracking mode is on
 */

    void updateUIWithTrackingMode ()
    {
        if (mTrackingMode)
        {
            mTrackingModeButton.setBackgroundResource (R.drawable.btn_tracking_on);
            if (mLocationOverlay.isEnabled () && mLocationOverlay.getLocation () != null)
            {
                mMap.getController ().animateTo (mLocationOverlay.getLocation ());
            }
            mMap.setMapOrientation (-mAzimuthAngleSpeed);
            mTrackingModeButton.setKeepScreenOn (true);
        }
        else
        {
            mTrackingModeButton.setBackgroundResource (R.drawable.btn_tracking_off);
            mMap.setMapOrientation (0.0f);
            mTrackingModeButton.setKeepScreenOn (false);
        }
    }

    boolean startLocationUpdates ()
    {
        boolean result = false;
        for (final String 
            provider :
        mGpsTracker.getLocationManager ().getProviders (true))
        {
            mGpsTracker.getLocationManager ().requestLocationUpdates (
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

    private void putRoadNodes (Road road)
    {
        updateRoadNodes (road, 0);
    }

/**
 * update nodes on map overlay from specific one
 */

    private void updateRoadNodes (Road road, int index)
    {
        mRoadNodeMarkers.getItems ().clear ();
        Drawable icon = ContextCompat.getDrawable (this, R.drawable.marker_node);
        int n = road.mNodes.size ();

        MarkerInfoWindow infoWindow = new MarkerInfoWindow (
            org.osmdroid.bonuspack.R.layout.bonuspack_bubble, mMap);
        TypedArray iconIds = getResources ().obtainTypedArray (R.array.direction_icons);

        for (int i = index; i < n; i++)
        {
            RoadNode node = road.mNodes.get (i);
            String instructions = (node.mInstructions == null ? "" : node.mInstructions);
            Marker nodeMarker = new Marker (mMap);
            nodeMarker.setTitle (getString (R.
            string.step)
            +" " + (i + 1))
            ;
            nodeMarker.setSnippet (instructions);
            nodeMarker.setSubDescription (
                Road.getLengthDurationText (this, node.mLength, node.mDuration));
            nodeMarker.setPosition (node.mLocation);
            nodeMarker.setIcon (icon);
            nodeMarker.setInfoWindow (infoWindow); //use a shared infowindow.
            int iconId = iconIds.getResourceId (node.mManeuverType, R.drawable.ic_empty);
            if (iconId != R.drawable.ic_empty)
            {
                Drawable image = ContextCompat.getDrawable (this, iconId);
                nodeMarker.setImage (image);
            }
            mRoadNodeMarkers.add (nodeMarker);
        }
        iconIds.recycle ();
    }

/**
 * paint road lines with colors blue or red
 */

    void selectRoad (int roadIndex)
    {
        mSelectedRoad = roadIndex;
        putRoadNodes (mRoads [roadIndex]);

        for (int i = 0; i < mRoadOverlays.length; i++)
        {
            Paint p = mRoadOverlays [i].getPaint ();
            if (i == roadIndex)
                p.setColor (getResources ().getColor (R.color.colorPrimaryDark)); //blue
            else
                p.setColor (getResources ().getColor (R.color.colorAccent)); // red
        }
        mMap.invalidate ();
    }

    void updateUiWithRoads (Road[] roads)
    {
        mRoadNodeMarkers.getItems ().clear ();
        List<Overlay> mapOverlays = mMap.getOverlays ();
        if (mRoadOverlays != null)
        {
            for (Polyline
                mRoadOverlay :
            mRoadOverlays)
            {
                mapOverlays.remove (mRoadOverlay);
            }
            mRoadOverlays = null;
        }
        if (roads == null || roads [0] == null)
        {
            return;
        }

        if (roads [0].mStatus == Road.STATUS_TECHNICAL_ISSUE)
            Toast.makeText (mMap.getContext (), R.
        string.technical_issue,
        Toast.LENGTH_SHORT).
        show ();
    else
        if (roads [0].mStatus > Road.STATUS_TECHNICAL_ISSUE) //functional issues
            Toast.makeText (mMap.getContext (), R.
        string.no_route,
        Toast.LENGTH_SHORT).
        show ();
        mRoadOverlays = new Polyline[roads.length];
        for (int i = 0; i < roads.length; i++)
        {
            Polyline roadPolyline = RoadManager.buildRoadOverlay (roads [i], this);
            mRoadOverlays [i] = roadPolyline;

            String routeDesc = roads [i].getLengthDurationText (this, -1);
            roadPolyline.setTitle (getString (R.
            string.route)
            +" - " + routeDesc)
            ;
            roadPolyline.setInfoWindow (
                new BasicInfoWindow (org.osmdroid.bonuspack.R.layout.bonuspack_bubble, mMap));
            roadPolyline.setRelatedObject (i);
            mapOverlays.add (1, roadPolyline);
            //we insert the road overlays at the "bottom", just above the MapEventsOverlay,
            //to avoid covering the other overlays.
        }
        selectRoad (0);
    }


/**
 * Gets a road in the background and notifies the listener when its ready
 *
 * @param index The first waypoint of the road
 */

    public void getRoadAsync (int index)
    {
        mRoads = null;
        GeoPoint roadStartPoint = null;
        if (mStartPoint != null)
        {
            roadStartPoint = mStartPoint;
        }
        else
            if (mLocationOverlay.isEnabled () && mLocationOverlay.getLocation () != null)
            {
                //use my current location as itinerary start point:
                roadStartPoint = mLocationOverlay.getLocation ();
            }

        if (roadStartPoint == null)
        {
            updateUiWithRoads (null);
            updateUIWithPolygon (mViaPoints, "");
            return;
        }
        ArrayList<GeoPoint> waypoints = new ArrayList<> (2);
        waypoints.add (roadStartPoint);

        //add intermediate via points:
        for (int i = index; i < mViaPoints.size (); i++)
        {
            waypoints.add (mViaPoints.get (i).getGeoPoint ());
        }

        //waypoints.add(mDestinationPoint);
        new UpdateRoadTask ().execute (waypoints);
    }

/**
 * add or replace the polygon overlay
 */

    public void updateUIWithPolygon (ArrayList<ViaPointData> viaPoints, String name)
    {
        List<Overlay> mapOverlays = mMap.getOverlays ();
        int location = -1;
        if (mDestinationPolygon != null)
            location = mapOverlays.indexOf (mDestinationPolygon);
        mDestinationPolygon = new Polygon (this);
        mDestinationPolygon.setFillColor (0x15FF0080);
        mDestinationPolygon.setStrokeColor (0x800000FF);
        mDestinationPolygon.setStrokeWidth (5.0f);
        mDestinationPolygon.setTitle (name);

        ArrayList<GeoPoint> polygon = new ArrayList<> ();
        for (ViaPointData
            viaPoint :
        viaPoints)
        {
            polygon.add (viaPoint.getGeoPoint ());
        }

        if (polygon.size () > 0)
        {
            mDestinationPolygon.setPoints (polygon);
        }

        if (location != -1)
        {
            mapOverlays.set (location, mDestinationPolygon);
        }
        else
        {
            mapOverlays.add (1, mDestinationPolygon); //insert just above the MapEventsOverlay.
        }

        mMap.invalidate ();
    }

/**
 * Adds an alert that triggers when a user is within a defined range of a specific coordinate
 *
 * @param latitude  the latitude of the alert point
 * @param longitude the longitude of the alert point
 */

    private void addProximityAlert (double latitude, double longitude)
    {
        Intent intent = new Intent (PROX_ALERT);

        mProximityIntent = PendingIntent.getBroadcast (this, 0, intent,
                                                       PendingIntent.FLAG_CANCEL_CURRENT);
        mGpsTracker.getLocationManager ().addProximityAlert (
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
    @Override

    protected void onSaveInstanceState (Bundle outState)
    {
        outState.putParcelable (SAVEDSTATE_LOCATION, mLocationOverlay.getLocation ());
        outState.putBoolean (SAVEDSTATE_TRACKING_MODE, mTrackingMode);
        outState.putParcelable (SAVEDSTATE_START, mStartPoint);
        //outState.putParcelable(SAVEDSTATE_DESTINATION, mDestinationPoint);
        //outState.putParcelableArrayList(SAVEDSTATE_VIAPOINTS, mViaPoints);
        outState.putInt (SAVEDSTATE_REACHED_NODE, mReachedNode);
        outState.putInt (SAVEDSTATE_NEXT_NODE, mNextNode);
        outState.putInt (SAVEDSTATE_NEXT_VIA_POINT, mNextViaPoint);
    }

    @Override

    public void onStatusChanged (String provider, int status, Bundle extras)
    {
    }

    @Override

    public void onProviderEnabled (String provider)
    {
    }

    @Override

    public void onProviderDisabled (String provider)
    {
    }

    @Override

    public boolean singleTapConfirmedHelper (GeoPoint p)
    {
        return false;
    }

    @Override

    public boolean longPressHelper (GeoPoint p)
    {
        return false;
    }

    @Override
    public void onSensorChanged (SensorEvent 
    event)
    {
    }

    @Override

    public void onAccuracyChanged (Sensor sensor, int accuracy)
    {
        mLocationOverlay.setAccuracy (accuracy);
        mMap.invalidate ();
    }

    @Override

    protected void onResume ()
    {
        boolean isOneProviderEnabled = startLocationUpdates ();
        mLocationOverlay.setEnabled (isOneProviderEnabled);

        IntentFilter filter = new IntentFilter (PROX_ALERT);
        registerReceiver (mProximityIntentReceiver, filter);

        super.onResume ();
    }

    @Override

    protected void onPause ()
    {
        if (mProgressDialog.isShowing ())
        {
            mProgressDialog.dismiss ();
        }

        mGpsTracker.getLocationManager ().removeUpdates (this);
        unregisterReceiver (mProximityIntentReceiver);

        super.onPause ();
    }

    @Override

    protected void onDestroy ()
    {
        super.onDestroy ();
    }

    @Override

    public void onBackPressed ()
    {
        Intent mIntent = new Intent ();
        mIntent.putExtra ("onBackPressed", true);
        setResult (RESULT_OK, mIntent);

        super.onBackPressed ();
    }

/**
 * Async task to get the road in a separate thread.
 */

    private class UpdateRoadTask 

    extends AsyncTask<Object, Void, Road 

    []
    >

    {
    protected
        void onPreExecute 
        ()
        {
            mProgressDialog.setMessage (getString (R.
            string.download_road))
            ;
            mProgressDialog.show ();
        }

    protected
        Road[] doInBackground 
        (Object...params)
        {
            @SuppressWarnings ("unchecked")
            ArrayList<GeoPoint> waypoints = (ArrayList<GeoPoint>) params[
            0]
            ;
            RoadManager roadManager = new MapQuestRoadManager (getString (R.
            string.map_quest_key))
            ;
            roadManager.addRequestOption ("routeType=pedestrian");
            roadManager.addRequestOption ("locale=de_DE");

            return roadManager.getRoads (waypoints);
        }

    protected
        void onPostExecute 
        (Road []
        result)
        {
            mProgressDialog.setMessage (getString (R.
            string.creating_map))
            ;
            mRoads = result;
            if (mRoads != null)
            {
                updateUiWithRoads (result);

                // TODO Remove this as soon as no needs to run on emulator
                // needed to set route info and dismiss busy waiting dialog on emulator
                if (Build.MODEL.contains ("google_sdk") ||
                    Build.MODEL.contains ("Emulator") ||
                    Build.MODEL.contains ("Android SDK"))
                {
                    if (mRoads [0] != null && mRoads [0].mNodes != null)
                    {
                        setNextStepToAlert (mRoads [0].mNodes.get (0).mLocation);
                    }
                }
            }
        }
    }

    /**
     * Gets called when the user approaches a node
     */

    public class ProximityIntentReceiver 

    extends BroadcastReceiver { @Override  public  

    void onReceive (Context context, Intent intent)
    {
        String key = LocationManager.KEY_PROXIMITY_ENTERING;
        Boolean entering = intent.getBooleanExtra (key, false);
        if (entering)
        {
            //We entered near a node, count up the reached nodes
            mReachedNode++;
            mGpsTracker.getLocationManager ().removeProximityAlert (mProximityIntent);
        }
    }

}

}