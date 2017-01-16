using System;
using System.Collections.Generic;
using CoreGraphics;
using CoreLocation;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using de.upb.hip.mobile.pcl.Helpers;
using HipMobileUI.iOS.Map;
using HipMobileUI.Map;
using MapKit;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(OsmMap), typeof(MapRenderer))]

namespace HipMobileUI.iOS.Map
{
    class MapRenderer : ViewRenderer<OsmMap, MKMapView> {

        protected override void OnElementChanged(ElementChangedEventArgs<OsmMap> e)
        {
            base.OnElementChanged(e);
           

            if (Control == null)
            {
                var mapView = new MKMapView();
                this.SetNativeControl(mapView);
                var nativeMap = Control as MKMapView;

                var overlay = new MKTileOverlay ("http://tile.openstreetmap.org/{z}/{x}/{y}.png") {CanReplaceMapContent = true};
                mapView.AddOverlay(overlay, MKOverlayLevel.AboveLabels);
                mapView.OverlayRenderer = (view, mkOverlay) => new MKTileOverlayRenderer(overlay);

                MKCoordinateRegion region = nativeMap.Region;
                region.Span.LatitudeDelta = 0.05;
                region.Span.LongitudeDelta = 0.05;
                region.Center = new CLLocationCoordinate2D(AppSharedData.PaderbornMainStation.Latitude, AppSharedData.PaderbornMainStation.Longitude);
                nativeMap.Region = region;

                if (e.NewElement != null)
                {
                    InitAnnotations(nativeMap, e.NewElement.ExhibitSet);
                    nativeMap.GetViewForAnnotation = GetViewForAnnotation;
                    nativeMap.CalloutAccessoryControlTapped += OnCalloutAccessoryControlTapped;
                }

                if (e.OldElement != null)
                {
                    nativeMap.GetViewForAnnotation = null;
                    nativeMap.CalloutAccessoryControlTapped -= OnCalloutAccessoryControlTapped;
                }
            }
        }

        private MKAnnotationView GetViewForAnnotation(MKMapView mapview, IMKAnnotation annotation)
        {
            if (annotation is MKUserLocation)
            {
                return null;
            }

            const string annotationReusableId = "ExhibitAnnotation";
            MKAnnotationView annotationView = null;
            var dequedView = mapview.DequeueReusableAnnotation(annotationReusableId);
            if (dequedView != null)
            {
                dequedView.Annotation = annotation;
                annotationView = dequedView;
            }
            else
            {
                annotationView = new ExhibitAnnotationView (annotation, annotationReusableId);
            }
            annotationView.CalloutOffset = new CGPoint(0, 0);
            annotationView.RightCalloutAccessoryView = UIButton.FromType(UIButtonType.DetailDisclosure);
            annotationView.CanShowCallout = true;
            return annotationView;
        }

        private void OnCalloutAccessoryControlTapped(object sender, MKMapViewAccessoryTappedEventArgs e)
        {
            var exhibitAnnotationView = e.View as ExhibitAnnotationView;
            var annotation = exhibitAnnotationView?.Annotation as ExhibitAnnotation;
            if (annotation != null)
            {
                var exhibitId = annotation.ExhibitId;
                // open exhibit details
            }
        }

        private void InitAnnotations(MKMapView map, ExhibitSet exhibitSet)
        {
            foreach (var exhibit in exhibitSet.ActiveSet)
            {
                var annotation = new ExhibitAnnotation(exhibit.Location.Latitude, exhibit.Location.Longitude, exhibit.Id,
                                                                                exhibit.Name, exhibit.Description);
                map.AddAnnotation (annotation);
            }
        }
    }
}