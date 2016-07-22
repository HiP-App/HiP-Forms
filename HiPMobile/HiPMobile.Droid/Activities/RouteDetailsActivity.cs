using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using de.upb.hip.mobile.droid.fragments;
using de.upb.hip.mobile.droid.Helpers;
using de.upb.hip.mobile.droid.Listeners;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using Java.Lang;
using Microsoft.Practices.ObjectBuilder2;
using Osmdroid;
using Osmdroid.TileProvider.TileSource;
using Osmdroid.Util;
using Osmdroid.Views;
using Osmdroid.Views.Overlay;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace de.upb.hip.mobile.droid.Activities
{
    [Activity(Theme = "@style/AppTheme.NoActionBar")]
    public class RouteDetailsActivity : AppCompatActivity
    {
        private Route route;
        public static readonly string KEY_ROUTE_ID = "route";
        private GeoPoint currentUserLocation = new GeoPoint(51.71352, 8.74021);
        private MapView map;
        private ItemizedIconOverlay mapMarkerItemizedOverlay;
        private ExtendedLocationListener gpsTracker;
        public static readonly int MAX_ZOOM_LEVEL = 16;
        public static readonly int ZOOM_LEVEL = 16;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_route_details);

            var toolbar = (Toolbar)FindViewById(Resource.Id.toolbar);
            toolbar.SetNavigationIcon(Resource.Drawable.ic_clear_white_24dp);
            SetSupportActionBar(toolbar);

            var extras = Intent.Extras;
            string routeId = extras.GetString(KEY_ROUTE_ID);
            route = RouteManager.GetRoute(routeId);
            gpsTracker = new ExtendedLocationListener(this);

            if (route != null)
            {
                // getting location
                if (gpsTracker.CanGetLocation)
                {
                    currentUserLocation = new GeoPoint(
                            gpsTracker.Latitude, gpsTracker.Longitude);
                }

                // remove this for reals usage
                currentUserLocation = new GeoPoint(ExtendedLocationListener.PADERBORN_HBF.Latitude,
                    ExtendedLocationListener.PADERBORN_HBF.Longitude);

                Title = route.Title;
                InitRouteInfo();
                InitMap();

                AddStartPointOnMap();
                AddViaPointsOnMap();
                AddFinalPointOnMap();

                DrawPathOnMap();

                map.OverlayManager.Add(mapMarkerItemizedOverlay);
            }
            else
            {
                Toast.MakeText(this, Resource.String.empty_route, ToastLength.Short).Show();
            }

            Button button = (Button)this.FindViewById(Resource.Id.routeDetailsStartNavigationButton);
            button.Click += (sender, args) =>
            {
                // When clicking on button, check if GPS and internet is available, if both available
                // the routing is started.
                // Shows dialog for GPS settings if not available.
                // Shows no internet connection if internet not available.

            };
        }

        private void InitRouteInfo()
        {
            TextView descriptionView = (TextView)FindViewById(Resource.Id.routeDetailsDescription);
            TextView durationView = (TextView)FindViewById(Resource.Id.routeDetailsDuration);
            TextView distanceView = (TextView)FindViewById(Resource.Id.routeDetailsDistance);
            LinearLayout tagsLayout = (LinearLayout)FindViewById(Resource.Id.routeDetailsTagsLayout);

            descriptionView.Text = route.Description;
            int durationInMinutes = route.Duration / 60;
            durationView.Text = Resources.GetQuantityString(
                Resource.Plurals.route_activity_duration_minutes, durationInMinutes, durationInMinutes);
            distanceView.Text = String.Format(Resources.GetString(Resource.String.route_activity_distance_kilometer), route.Distance);

            //Add tags
            if (route.RouteTags.Any())
            {
                tagsLayout.RemoveAllViews();
                foreach (RouteTag tag in route.RouteTags)
                {
                    ImageView tagImageView = new ImageView(ApplicationContext);
                    tagImageView.SetImageDrawable(tag.Image.GetDrawable());
                    tagsLayout.AddView(tagImageView);
                }
            }
        }

        /// <summary>
        /// Init the map, set the tile source and set the zoom.
        /// </summary>
        private void InitMap()
        {
            map = FindViewById<MapView>(Resource.Id.routedetails_mapview);
            map.SetTileSource(TileSourceFactory.DefaultTileSource);
            map.SetBuiltInZoomControls(true);
            map.SetMultiTouchControls(true);
            map.TilesScaledToDpi = true;
            map.SetMaxZoomLevel(MAX_ZOOM_LEVEL);

            var mapController = map.Controller;
            
            mapController.SetZoom(ZOOM_LEVEL);
            mapController.SetCenter(currentUserLocation);

            // Initialize marker overlay
            mapMarkerItemizedOverlay = new ItemizedIconOverlay(this, new List<OverlayItem>(), null);
        }

        private void InitItineraryMarkers()
        {

            /*ViaPointInfoWindow mViaPointInfoWindow = new ViaPointInfoWindow(
                    R.layout.navigation_info_window, mMap, this);*/

            /*FolderOverlay mItineraryMarkers = new FolderOverlay(this);
            mItineraryMarkers.setName(getString(R.string.itinerary_markers_title));
            mMap.getOverlays().add(mItineraryMarkers);

            mMarker = new SetMarker(mMap, mItineraryMarkers, mViaPointInfoWindow);*/
            
        }

        /// <summary>
        /// Adds that start point to the map.
        /// If the current user location is known use it as start point.
        /// Else use the first way point as start point, but only if there are two or more way points
        /// (else there would be no route).
        /// </summary>
        private void AddStartPointOnMap()
        {
            GeoPoint geoLocation = null;
            string title = Resources.GetString(Resource.String.departure);
            string description = "";
            string exhibitId = "";
            Drawable drawable = null;

            if (this.currentUserLocation != null)
            {
                // setup our current location as start point
                geoLocation = this.currentUserLocation;
            }
            else if (route.Waypoints.Count() > 1)
            {
                // if no current location then use first waypoint as start point only if >=2 waypoints
                geoLocation = new GeoPoint(route.Waypoints[0].Location.Latitude,
                        route.Waypoints[0].Location.Longitude);

                // add related data to marker if start point is first waypoint
                if (route.Waypoints[0].Exhibit != null)
                {
                    Exhibit exhibit = route.Waypoints[0].Exhibit;
                    title = exhibit.Name;
                    description = exhibit.Description;
                    exhibitId = exhibit.Id;

                    drawable = exhibit.Image.GetDrawable();
                }
            }

            if (geoLocation != null)
            {
                if (drawable == null)
                {
                    // set default image in info window
                    drawable = ContextCompat.GetDrawable(this, Resource.Drawable.marker_departure);
                }

                // set and fill start point with data
                AddMarker(geoLocation, drawable, title, description,
                        exhibitId);
            }
        }

        /// <summary>
        /// Adds the way points between start or end point to the map (excluding start and end point).
        /// If the current user location is known start with the first way point.
        /// Else start with the second way point, but only if there are two or more way points
        /// (else there would be no route).
        /// Adds a single marker if the current user location is unknown and only one way point exits.
        /// </summary>
        private void AddViaPointsOnMap()
        {
            int waypointIndex = -1;

            if (this.currentUserLocation != null && route.Waypoints.Count() > 1)
            {
                waypointIndex = 0;
            }
            else if (this.currentUserLocation == null)
            {
                if (route.Waypoints.Count() > 2)
                {
                    waypointIndex = 1;
                }
                else if (route.Waypoints.Count() == 1)
                {
                    waypointIndex = 0;
                }
            }

            if (waypointIndex > -1)
            {
                //Add all waypoints to the map except the last one,
                // it would be marked as destination marker
                for (int index = waypointIndex; index < route.Waypoints.Count() - 1; index++)
                {
                    Waypoint waypoint = route.Waypoints[index];
                    if (waypoint.Exhibit != null)
                    {
                        GeoPoint geoPoint =
                                new GeoPoint(waypoint.Location.Latitude, waypoint.Location.Longitude);
                        Exhibit exhibit = waypoint.Exhibit;

                        Drawable drawable = exhibit.Image.GetDrawable();

                        // add marker on map for waypoint
                        AddMarker(geoPoint, Resources.GetDrawable(Resource.Drawable.marker_via), exhibit.Name,
                                exhibit.Description, exhibit.Id);
                    }
                }
            }
        }

        /// <summary>
        /// Adds the end point to the map.
        /// Takes the the last way point, if the current user location is known and way points exits,
        /// or if the current user location is unknown, but two or more way points exit.
        /// </summary>
        private void AddFinalPointOnMap()
        {
            GeoPoint geoLocation;
            string title = Resources.GetString(Resource.String.destination);
            string description = "";
            string exhibitId = "";
            Drawable drawable;

            int waypointIndex = -1;

            if ((this.currentUserLocation != null) && (route.Waypoints.Any()))
            {
                // if current location is not null and we have at least one waypoint, then
                // use last one as destination point
                waypointIndex = route.Waypoints.Count() - 1;
            }
            else
            {
                if (route.Waypoints.Count() > 1)
                {
                    // if current location is null and we have more then one waypoint, then
                    // use last waypoint as destination point
                    waypointIndex = route.Waypoints.Count() - 1;
                }
            }

            if (waypointIndex > -1)
            {
                geoLocation = new GeoPoint(route.Waypoints[waypointIndex].Location.Latitude,
                        route.Waypoints[waypointIndex].Location.Longitude);

                // add related data to marker
                if (route.Waypoints[waypointIndex].Exhibit != null)
                {
                    Exhibit exhibit = route.Waypoints[waypointIndex].Exhibit;
                    title = exhibit.Name;
                    description = exhibit.Description;
                    exhibitId = exhibit.Id;

                    drawable = exhibit.Image.GetDrawable();
                }
                else
                {
                    drawable = ContextCompat.GetDrawable(this, Resource.Drawable.marker_destination);
                }

                // set final point
                AddMarker(geoLocation, Resources.GetDrawable(Resource.Drawable.marker_destination), title, description,
                        exhibitId);
            }
        }

        /// <summary>
        /// Paint simple road lines with blue color. PathOverlay is deprecated, but for drawing simple
        /// path is perfect.
        /// The new, not deprecated class Polylines is more complex and needs a road from RoadManager
        /// </summary>
        private void DrawPathOnMap()
        {
            PathOverlay myPath = new PathOverlay(Resources.GetColor(Resource.Color.colorPrimaryDark),
                    5, new DefaultResourceProxyImpl(this));

            if (currentUserLocation != null)
            {
                myPath.AddPoint(currentUserLocation);
            }

            if (route != null && route.Waypoints.Any())
            {
                foreach (Waypoint waypoint in route.Waypoints)
                {
                    myPath.AddPoint(new GeoPoint(waypoint.Location.Latitude, waypoint.Location.Longitude));
                }
            }

            map.Overlays.Add(myPath);
            map.Invalidate();
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

        /// <summary>
        /// Adds the marker with the data of t and put on the map.
        /// </summary>
        /// <param name="geoLocation">GeoPoint of the created  marker</param>
        /// <param name="drawable">Drawable, image of the exhibit</param>
        /// <param name="markerImage">int, id from drawable</param>
        /// <param name="title">String, title of the exhibit</param>
        /// <param name="description">String, description of the exhibit</param>
        /// <param name="exhibitId">int, exhibit id</param>
        private void AddMarker(GeoPoint geoLocation, Drawable drawable, string title,
                           string description, string exhibitId)
        {
            var marker = new OverlayItem(title, description, geoLocation);
            marker.SetMarker(drawable);
            this.mapMarkerItemizedOverlay.AddItem(marker);
            /*Map<String, Integer> data = new HashMap<>();
            data.put(title, exhibitId);

            mMarker.addMarker(null, title, description, geoLocation, drawable, icon, data);*/
        }
    }
}