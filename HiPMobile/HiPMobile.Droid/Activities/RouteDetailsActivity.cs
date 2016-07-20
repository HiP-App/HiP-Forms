using System.Linq;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using de.upb.hip.mobile.droid.fragments;
using de.upb.hip.mobile.droid.Helpers;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using Microsoft.Practices.ObjectBuilder2;

namespace de.upb.hip.mobile.droid.Activities
{
    [Activity(Theme = "@style/AppTheme")]
    public class RouteDetailsActivity : AppCompatActivity
    {
        private Route route;
        public static readonly string KEY_ROUTE_ID = "route";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_route_details);

            var extras = Intent.Extras;
            string routeId = extras.GetString(KEY_ROUTE_ID);
            route = RouteManager.GetRoute(routeId);

            if (route != null)
            {
                initRouteInfo();
            }
            else
            {
                Toast.MakeText(this, Resource.String.empty_route, ToastLength.Short).Show();
            }

            /*if (FindViewById(Resource.Id.route_map_container) != null)
            {
                // create temporary exhibitset
                ExhibitSet set = new ExhibitSet();
                route.Waypoints.ForEach(wp => set.ActiveSet.Add(wp.Exhibit));

                MapFragment fragment = new MapFragment
                {
                    ExhibitSet = set,
                    GeoLocation = new GeoLocation()
                    {
                        Latitude = 51.71352,
                        Longitude = 8.74021
                    }
                };
                var transaction = FragmentManager.BeginTransaction();
                transaction.Replace(Resource.Id.route_map_container, fragment);
                transaction.Commit();
            }*/

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
            distanceView.Text = string.Format(Resources.GetString(Resource.String.route_activity_distance_kilometer), route.Distance);

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
    }
}