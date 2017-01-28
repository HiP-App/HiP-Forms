using System;
using System.Collections.Generic;
using CoreGraphics;
using CoreLocation;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using de.upb.hip.mobile.pcl.Common;
using de.upb.hip.mobile.pcl.Helpers;
using HipMobileUI.Helpers;
using HipMobileUI.iOS.Map;
using HipMobileUI.Map;
using HipMobileUI.Navigation;
using HipMobileUI.ViewModels.Pages;
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

                var overlay = new MKTileOverlay ("http://tile.openstreetmap.org/{z}/{x}/{y}.png") {CanReplaceMapContent = true};
                mapView.AddOverlay(overlay, MKOverlayLevel.AboveLabels);
                mapView.OverlayRenderer = (view, mkOverlay) => new MKTileOverlayRenderer(overlay);

                

                if (e.OldElement != null)
                {
                    Control.GetViewForAnnotation = null;
                    Control.CalloutAccessoryControlTapped -= OnCalloutAccessoryControlTapped;
                    e.OldElement.GpsLocationChanged -= OnGpsLocationChanged;
                    e.OldElement.ExhibitSetChanged -= OnExhibitSetChanged;
                }
                if (e.NewElement != null)
                {
                    InitAnnotations(e.NewElement.ExhibitSet);
                    Control.GetViewForAnnotation = GetViewForAnnotation;
                    Control.CalloutAccessoryControlTapped += OnCalloutAccessoryControlTapped;
                    e.NewElement.GpsLocationChanged += OnGpsLocationChanged;
                    e.NewElement.ExhibitSetChanged += OnExhibitSetChanged;
                }

                Control.ShowsUserLocation = true;
                Control.ShowsCompass = true;
                InitMapPosition (Control);
            }
        }

        private void OnExhibitSetChanged(ExhibitSet set)
        {
           InitAnnotations (set);
        }

        private void OnGpsLocationChanged(GeoLocation location)
        {
            Control.CenterCoordinate = new CLLocationCoordinate2D(location.Latitude, location.Longitude);
        }

        private void InitMapPosition (MKMapView mapView)
        {
            if (mapView.UserLocation.Coordinate.Equals (new CLLocationCoordinate2D (0, 0)) && CLLocationManager.LocationServicesEnabled)
            {
                MKCoordinateRegion region = mapView.Region;
                var center = new CLLocationCoordinate2D(AppSharedData.PaderbornCenter.Latitude, AppSharedData.PaderbornCenter.Longitude);
                mapView.SetRegion(MKCoordinateRegion.FromDistance(center, 1000, 1000), true);
            }
        }

        private MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
        {
            MKAnnotationView annotationView = null;
            MKAnnotationView dequedView = null;
            if (annotation.Coordinate.Equals (mapView.UserLocation.Coordinate) && !(annotation is ExhibitAnnotation)) //(annotation is MKUserLocation) doesn't work
            {
                const string userAnnotationReusableId = "UserAnnotation";
                dequedView = mapView.DequeueReusableAnnotation(userAnnotationReusableId);
                if (dequedView != null)
                {
                    return dequedView;
                }
                else
                {
                    annotationView = new UserAnnotationView (annotation, userAnnotationReusableId);
                    return annotationView;
                }
            }

            const string annotationReusableId = "ExhibitAnnotation";
            dequedView = mapView.DequeueReusableAnnotation(annotationReusableId);
            if (dequedView != null)
            {
                dequedView.Annotation = annotation;
                annotationView = dequedView;
            }
            else
            {
                annotationView = new ExhibitAnnotationView (annotation, annotationReusableId);
                annotationView.CalloutOffset = new CGPoint(0, 0);
                annotationView.RightCalloutAccessoryView = UIButton.FromType(UIButtonType.DetailDisclosure);
            }
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
                IoCManager.Resolve<INavigationService>().PushAsync(new ExhibitDetailsViewModel(exhibitId));
            }
        }

        private void InitAnnotations(ExhibitSet exhibitSet)
        {
            foreach (var exhibit in exhibitSet.ActiveSet)
            {
                var annotation = new ExhibitAnnotation(exhibit.Location.Latitude, exhibit.Location.Longitude, exhibit.Id,
                                                                                exhibit.Name, exhibit.Description);
                Control.AddAnnotation (annotation);
            }
        }
    }
}