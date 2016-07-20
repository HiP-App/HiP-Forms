using System.Linq;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using de.upb.hip.mobile.droid.fragments;
using de.upb.hip.mobile.droid.Helpers;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using Java.Lang;
using Microsoft.Practices.ObjectBuilder2;
using Osmdroid.TileProvider.TileSource;
using Osmdroid.Util;
using Osmdroid.Views;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace de.upb.hip.mobile.droid.Activities
{
    [Activity(Theme = "@style/AppTheme.NoActionBar")]
    public class RouteDetailsActivity : AppCompatActivity
    {
        private Route route;
        public static readonly string KEY_ROUTE_ID = "route";

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

            if (route != null)
            {
                Title = route.Title;
                initRouteInfo();
                InitMap();
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

        private void initRouteInfo()
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

        private void InitMap()
        {
            var mapView = FindViewById<MapView>(Resource.Id.routedetails_mapview);
             mapView.SetTileSource(TileSourceFactory.DefaultTileSource);
            mapView.SetMultiTouchControls(true);

            /*mapView.SetTileSource(new XYTileSource("OSM", 0, 18, 1024, ".png",
                                                     new[] { "http://tile.openstreetmap.org/" }));*/

            var mapController = mapView.Controller;
            mapController.SetZoom(13);

            var centreOfMap = new GeoPoint(51.71352, 8.74021);
            //var centreOfMap = new GeoPoint(GeoLocation.Latitude, GeoLocation.Longitude);
            mapController.SetCenter(centreOfMap);
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