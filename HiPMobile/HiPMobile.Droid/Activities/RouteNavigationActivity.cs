using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content.Res;
using Android.Views;
using Android.Widget;
using de.upb.hip.mobile.droid.Helpers;
using de.upb.hip.mobile.droid.Listeners;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using Org.Osmdroid.Api;
using Org.Osmdroid.Bonuspack.Overlays;
using Org.Osmdroid.Bonuspack.Routing;
using Org.Osmdroid.Tileprovider;
using Org.Osmdroid.Tileprovider.Tilesource;
using Org.Osmdroid.Util;
using Org.Osmdroid.Views;
using Marker = Org.Osmdroid.Bonuspack.Overlays.Marker;
using Polyline = Org.Osmdroid.Bonuspack.Overlays.Polyline;

namespace de.upb.hip.mobile.droid.Activities {
    [Activity (Theme = "@style/AppTheme",Label = "RouteNavigationActivity")]
    public class RouteNavigationActivity : Activity {

        public const String INTENT_ROUTE = "route";

        protected MapView mapView;
        private ProgressDialog progressDialog;
        private GeoPoint geoLocation;
        protected ExtendedLocationListener mGpsTracker;
        private Route route;

        protected override void OnCreate (Bundle savedInstanceState)
        {
            base.OnCreate (savedInstanceState);
            SetContentView (Resource.Layout.activity_route_navigation);


            // getting location
            mGpsTracker = new ExtendedLocationListener(Application.Context);
            geoLocation = new GeoPoint(mGpsTracker.Latitude, mGpsTracker.Longitude);

            // TODO Remove this as soon as no needs to run in emulator
            // set default coordinats for emulator
            if (Build.Model.Contains("google_sdk") ||
                    Build.Model.Contains("Emulator") ||
                    Build.Model.Contains("Android SDK"))
            {
                geoLocation = new GeoPoint(ExtendedLocationListener.PADERBORN_HBF.Latitude,
                        ExtendedLocationListener.PADERBORN_HBF.Longitude);
            }

            if(geoLocation.Latitude == 0f && geoLocation.Longitude == 0f)
                geoLocation = new GeoPoint(ExtendedLocationListener.PADERBORN_HBF.Latitude,
                        ExtendedLocationListener.PADERBORN_HBF.Longitude);

            var extras = Intent.Extras;
            string routeId = extras.GetString(INTENT_ROUTE);
            route = RouteManager.GetRoute (routeId);

            SetUpMap ();

            ShowRoute ();
        }


        private void SetUpMap ()
        {
            // init progress dialog
            progressDialog = new ProgressDialog(this);
            progressDialog.SetCancelable(true);

            // getting the map
            GenericMapView genericMap = (GenericMapView)FindViewById(Resource.Id.routeNavigationMap);
            MapTileProviderBasic bitmapProvider = new MapTileProviderBasic(this);
            genericMap.SetTileProvider(bitmapProvider);
            mapView = genericMap.GetMapView();
            mapView.SetBuiltInZoomControls(true);
            mapView.SetMultiTouchControls(true);

            mapView.SetTileSource(TileSourceFactory.Mapnik);
            mapView.TilesScaledToDpi = (true);
           // mapView.SetMaxZoomLevel(RouteDetailsActivity.MAX_ZOOM_LEVEL);

            // mMap prefs:
            IMapController mapController = mapView.Controller;
            mapController.SetZoom(16);
            mapController.SetCenter(geoLocation);
        }

        private void ShowRoute ()
        {
            StrictMode.ThreadPolicy policy = new StrictMode.ThreadPolicy.Builder().PermitAll().Build();
            StrictMode.SetThreadPolicy(policy);


            RoadManager roadManager = new MapQuestRoadManager("WRWdd9j02K8tGtERI2LtFiCLsRUKyJnJ");
            roadManager.AddRequestOption("routeType=pedestrian");

            IList<Waypoint> wayPoints = route.Waypoints;
            List<GeoPoint> geoPoints = new List<GeoPoint>();

            geoPoints.Add(new GeoPoint(geoLocation.Latitude, geoLocation.Longitude));

            foreach (Waypoint w in wayPoints)
            {
                GeoPoint point = new GeoPoint (w.Location.Latitude,w.Location.Longitude);
                geoPoints.Add (point);
            }



            Road road = roadManager.GetRoad (geoPoints);
            if (road.MStatus != Road.StatusOk)
                Toast.MakeText(Application.Context, "Error when loading the road - status=" + road.MStatus, ToastLength.Short).Show();

            Polyline roadOverlay = RoadManager.BuildRoadOverlay(road, Application.Context);
            mapView.Overlays.Add(roadOverlay);



            FolderOverlay roadMarkers = new FolderOverlay(Application.Context);
            mapView.Overlays.Add(roadMarkers);
            Drawable nodeIcon = ResourcesCompat.GetDrawable(Resources, Resource.Drawable.marker_node, null);
            for (int i = 0; i < road.MNodes.Count; i++)
            {
                RoadNode node = (RoadNode)road.MNodes[i];
                Marker nodeMarker = new Marker(mapView);
                nodeMarker.Position = node.MLocation;
                nodeMarker.SetIcon(nodeIcon);
                

                //4. Filling the bubbles
                nodeMarker.Title = ("Step " + i);
                nodeMarker.Snippet = (node.MInstructions);
                nodeMarker.SubDescription = (Road.GetLengthDurationText(node.MLength, node.MDuration));
                Drawable iconContinue = ResourcesCompat.GetDrawable(Resources, Resource.Drawable.ic_continue, null);
                nodeMarker.Image = (iconContinue);
                //4. end

                roadMarkers.Add(nodeMarker);
            }
        }

    }
}