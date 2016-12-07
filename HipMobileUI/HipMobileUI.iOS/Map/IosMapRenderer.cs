using CoreLocation;
using HipMobileUI.iOS.Map;
using HipMobileUI.Map;
using MapKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(OsmMap), typeof(IosMapRenderer))]
namespace HipMobileUI.iOS.Map
{
    class IosMapRenderer : ViewRenderer<OsmMap, MKMapView>
    {

        protected override void OnElementChanged (ElementChangedEventArgs<OsmMap> e)
        {
            base.OnElementChanged (e);

            if (Control == null)
            {
                var mapView = new MKMapView ();
                this.SetNativeControl(mapView);
                var overlay = new MKTileOverlay("http://tile.openstreetmap.org/{z}/{x}/{y}.png");
                overlay.CanReplaceMapContent = true;
                mapView.AddOverlay(overlay, MKOverlayLevel.AboveLabels);
                mapView.OverlayRenderer = (view, mkOverlay) => new MKTileOverlayRenderer(overlay);
                MKCoordinateRegion region = mapView.Region;
                region.Span.LatitudeDelta = 0.05;
                region.Span.LongitudeDelta = 0.05;
                region.Center = new CLLocationCoordinate2D(51.7166700, 8.7666700);
                mapView.Region = region;
            }
        }

    }
}