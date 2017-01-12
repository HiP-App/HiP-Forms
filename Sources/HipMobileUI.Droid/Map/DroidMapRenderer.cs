using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using de.upb.hip.mobile.droid.Map;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using HipMobileUI.Map;
using Org.Osmdroid.Tileprovider.Tilesource;
using Org.Osmdroid.Util;
using Org.Osmdroid.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer (typeof (OsmMap), typeof (DroidMapRenderer))]

namespace de.upb.hip.mobile.droid.Map {
    class DroidMapRenderer : Xamarin.Forms.Platform.Android.AppCompat.ViewRenderer<OsmMap, MapView> {

        private OsmMap osmMap;
        private MapView mapView;

        protected override void OnElementChanged (ElementChangedEventArgs<OsmMap> e)
        {
            base.OnElementChanged (e);


            if (Control == null)
            {
                mapView = new MapView (Forms.Context, 11);
                this.SetNativeControl (mapView);

                mapView.SetTileSource (TileSourceFactory.DefaultTileSource);
                /*mapView.SetTileSource(new XYTileSource("OSM", ResourceProxyString.OnlineMode, 0, 18, 1024, ".png",
                new[] {"http://tile.openstreetmap.org/"}));*/

                mapView.SetMultiTouchControls (true);
                mapView.TilesScaledToDpi = true;

                var controller = mapView.Controller;
                controller.SetCenter (new GeoPoint (51.7189205, 8.7575093));
                controller.SetZoom (10);
            }

            if (e.OldElement != null)
            {
                // Unsubscribe
                e.OldElement.ExhibitSetChanged -= NewElementOnExhibitSetChanged;
            }
            if (e.NewElement != null)
            {
                // Subscribe
                e.NewElement.ExhibitSetChanged += NewElementOnExhibitSetChanged;
                NewElementOnExhibitSetChanged (e.NewElement.ExhibitSet);
            }
        }

        private void NewElementOnExhibitSetChanged (ExhibitSet set)
        {
            Console.WriteLine("Test");
        }

    }
}