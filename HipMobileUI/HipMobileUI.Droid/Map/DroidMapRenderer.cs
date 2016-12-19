using System;
using System.ComponentModel;
using HipMobileUI.Droid.Map;
using HipMobileUI.Map;
using Org.Osmdroid;
using Org.Osmdroid.Tileprovider.Tilesource;
using Org.Osmdroid.Util;
using Org.Osmdroid.Views;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(OsmMap), typeof(DroidMapRenderer))]
namespace HipMobileUI.Droid.Map
{
    class DroidMapRenderer : Xamarin.Forms.Platform.Android.AppCompat.ViewRenderer<OsmMap, MapView> {

        private OsmMap osmmap;
        private MapView mapView;

        protected override void OnElementChanged (ElementChangedEventArgs<OsmMap> e)
        {
            base.OnElementChanged (e);

            if (Control == null)
            {
                mapView = new MapView (Forms.Context, 11);
                this.SetNativeControl (mapView);

                mapView.SetTileSource(TileSourceFactory.DefaultTileSource);
                /*mapView.SetTileSource(new XYTileSource("OSM", ResourceProxyString.OnlineMode, 0, 18, 1024, ".png",
                new[] {"http://tile.openstreetmap.org/"}));*/

                mapView.SetMultiTouchControls(true);
                mapView.TilesScaledToDpi = true;

                var controller = mapView.Controller;
                controller.SetCenter(new GeoPoint(51.7189205, 8.7575093));
                controller.SetZoom (10);
            }

            if (e.NewElement != null)
            {
                osmmap = e.NewElement;
            }

            if (e.OldElement != null)
            {
            }
        }
    }
}