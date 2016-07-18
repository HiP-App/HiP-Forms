using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Support.V4.Content;
using Android.Views;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using Osmdroid.TileProvider.TileSource;
using Osmdroid.Util;
using Osmdroid.Views;
using Osmdroid.Views.Overlay;

namespace de.upb.hip.mobile.droid.fragments {
    public class MapFragment : Fragment {

        /// <summary>
        /// ExhibitSet containing the exhibit that should be displayed in the RecyclerView.
        /// </summary>
        public ExhibitSet ExhibitSet { get; set; }

        /// <summary>
        /// GeoLocation for the current position of the user.
        /// </summary>
        public GeoLocation GeoLocation { get; set; }


        public override void OnCreate (Bundle savedInstanceState)
        {
            base.OnCreate (savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate (Resource.Layout.fragment_map, container, false);

            var mapView = view.FindViewById<MapView> (Resource.Id.mapview);
            // mapView.SetTileSource(TileSourceFactory.DefaultTileSource);
            mapView.SetBuiltInZoomControls (true);

            mapView.SetTileSource (new XYTileSource ("OSM", 0, 18, 1024, ".png",
                                                     new[] {"http://tile.openstreetmap.org/"}));

            var mapController = mapView.Controller;
            mapController.SetZoom (13);

            // var centreOfMap = new GeoPoint(51496994, -134733);
            var centreOfMap = new GeoPoint (GeoLocation.Latitude, GeoLocation.Longitude);


            mapController.SetCenter (centreOfMap);

            SetAllMarkers (mapView);

            return view;
        }

        private void SetAllMarkers (MapView mapView)
        {
            //SetUp Markers TODO rewrite with markers from bonuspack
            var mapMarkerArray = new List<OverlayItem> ();
            var myLocationOverlay = new MyLocationOverlay (this.Activity, mapView);
            var mapMarkerIcon = ContextCompat.GetDrawable (this.Activity, Resource.Drawable.marker_blue);
            var myScaleBarOverlay = new ScaleBarOverlay (this.Activity);

            foreach (var e in ExhibitSet.ActiveSet)
            {
                //One Marker Object
                var marker = new OverlayItem (e.Marker.Title, e.Marker.Text, new GeoPoint (e.Location.Latitude, e.Location.Longitude));
                marker.SetMarker (mapMarkerIcon);
                mapMarkerArray.Add (marker);
            }

            //Initialize this after markers are added to 
            var mapMarkerItemizedOverlay = new ItemizedIconOverlay (this.Activity, mapMarkerArray, null);
            mapView.OverlayManager.Add (mapMarkerItemizedOverlay);
            mapView.OverlayManager.Add (myScaleBarOverlay);
            mapView.OverlayManager.Add (myLocationOverlay);
            mapView.PostInvalidate ();
        }

    }
}