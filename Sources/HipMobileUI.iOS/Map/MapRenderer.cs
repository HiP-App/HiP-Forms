﻿using System;
using System.Collections.Generic;
using CoreGraphics;
using CoreLocation;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using de.upb.hip.mobile.pcl.BusinessLayer.Routing;
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

[assembly: ExportRenderer (typeof (OsmMap), typeof (MapRenderer))]

namespace HipMobileUI.iOS.Map {
    class MapRenderer : ViewRenderer<OsmMap, MKMapView> {


        private LocationManager locationManager;
        private MKMapView nativeMap;
        private OsmMap osmMap;
        private RouteCalculator routeCalculator;


        protected override void OnElementChanged(ElementChangedEventArgs<OsmMap> e)
        {
            base.OnElementChanged (e);


            if (Control == null)
            {

                var mapView = new MKMapView ();
                this.SetNativeControl (mapView);
                nativeMap = Control as MKMapView;
                routeCalculator = RouteCalculator.Instance;


                var overlay = new MKTileOverlay ("http://tile.openstreetmap.org/{z}/{x}/{y}.png") {CanReplaceMapContent = true};
                mapView.AddOverlay (overlay, MKOverlayLevel.AboveLabels);
                mapView.OverlayRenderer = (view, mkOverlay) => new MKTileOverlayRenderer (overlay);



                if (e.OldElement != null)
                {
                    Control.GetViewForAnnotation = null;
                    Control.CalloutAccessoryControlTapped -= OnCalloutAccessoryControlTapped;
                    e.OldElement.GpsLocationChanged -= OnGpsLocationChanged;
                    e.OldElement.ExhibitSetChanged -= OnExhibitSetChanged;
                }
                if (e.NewElement != null)
                {
					 osmMap = e.NewElement;
                    InitAnnotations(e.NewElement.ExhibitSet);
                    Control.GetViewForAnnotation = GetViewForAnnotation;
                    Control.CalloutAccessoryControlTapped += OnCalloutAccessoryControlTapped;
                    e.NewElement.GpsLocationChanged += OnGpsLocationChanged;
                    e.NewElement.ExhibitSetChanged += OnExhibitSetChanged;
                }

                locationManager = new LocationManager ();
                locationManager.StartLocationUpdates ();
                nativeMap.ShowsUserLocation = true;

                nativeMap.ShowsCompass = true;

                InitMapPosition (nativeMap);
            }
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
                var center = new CLLocationCoordinate2D (AppSharedData.PaderbornCenter.Latitude, AppSharedData.PaderbornCenter.Longitude);
                mapView.SetRegion (MKCoordinateRegion.FromDistance (center, 1000, 1000), true);
            }
        }


        private MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
        {
            MKAnnotationView annotationView = null;
            MKAnnotationView dequedView = null;
            if (annotation.Coordinate.Equals (mapView.UserLocation.Coordinate) && !(annotation is ExhibitAnnotation)) //(annotation is MKUserLocation) doesn't work
            {
                const string userAnnotationReusableId = "UserAnnotation";
                dequedView = mapView.DequeueReusableAnnotation (userAnnotationReusableId);
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
            dequedView = mapView.DequeueReusableAnnotation (annotationReusableId);
            if (dequedView != null)
            {
                dequedView.Annotation = annotation;
                annotationView = dequedView;
            }
            else
            {
                annotationView = new ExhibitAnnotationView (annotation, annotationReusableId);
                annotationView.CalloutOffset = new CGPoint (0, 0);
                annotationView.RightCalloutAccessoryView = UIButton.FromType (UIButtonType.DetailDisclosure);
            }
            annotationView.CanShowCallout = true;
            return annotationView;
        }

        private void OnCalloutAccessoryControlTapped (object sender, MKMapViewAccessoryTappedEventArgs e)
        {
            var exhibitAnnotationView = e.View as ExhibitAnnotationView;
            var annotation = exhibitAnnotationView?.Annotation as ExhibitAnnotation;
            if (annotation != null)
            {
                var exhibitId = annotation.ExhibitId;
                IoCManager.Resolve<INavigationService>().PushAsync(new ExhibitDetailsViewModel(exhibitId));
            }
        }


        private void InitAnnotations (MKMapView map, ExhibitSet exhibitSet)
        {
            foreach (var exhibit in exhibitSet.ActiveSet)
            {
                var annotation = new ExhibitAnnotation (exhibit.Location.Latitude, exhibit.Location.Longitude, exhibit.Id,
                                                        exhibit.Name, exhibit.Description);
                map.AddAnnotation (annotation);
            }
        }


        //TODO use this when everything else is done to perform navigation some things have to be adjusted
       /* private void CreateRoute ()
        {
            nativeMap.OverlayRenderer = OverlayRenderer;
            string template = "http://tile.openstreetmap.org/{z}/{x}/{y}.png";
            MKTileOverlay overlay = new MKTileOverlay (template);
            overlay.CanReplaceMapContent = true;
            nativeMap.AddOverlay (overlay);
            var id = osmMap.DetailsRoute.Id;
            // Center the map, for development purposes
            MKCoordinateRegion region = nativeMap.Region;
            region.Span.LatitudeDelta = 0.05;
            region.Span.LongitudeDelta = 0.05;
            region.Center = new CLLocationCoordinate2D (51.7166700, 8.7666700);
            nativeMap.Region = region;

            // Disable rotation programatically because value of designer is somehow ignored
            nativeMap.RotateEnabled = false;


            var locations = routeCalculator.CreateRouteWithSeveralWaypoints(new GeoLocation(nativeMap.UserLocation.Location.Coordinate.Latitude, nativeMap.UserLocation.Location.Coordinate.Longitude),id);
            var geoPoints = new List<CLLocationCoordinate2D> { new CLLocationCoordinate2D(nativeMap.UserLocation.Location.Coordinate.Latitude, nativeMap.UserLocation.Location.Coordinate.Longitude) };

            foreach (GeoLocation w in locations)
            {
                var point = new CLLocationCoordinate2D (w.Latitude, w.Longitude);
                geoPoints.Add (point);
            }

            var line = MKPolyline.FromCoordinates (geoPoints.ToArray ());
            nativeMap.AddOverlay (line);
            //map.SetVisibleMapRect(line.BoundingMapRect, false);

            if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0))
            {
                locationManager.RequestWhenInUseAuthorization ();
            }
        }*/


        private MKOverlayRenderer OverlayRenderer (MKMapView mapView, IMKOverlay overlay)
        {
            if (overlay is MKTileOverlay)
            {
                var renderer = new MKTileOverlayRenderer ((MKTileOverlay) overlay);
                return renderer;
            }
            else if (overlay is MKPolyline)
            {
                MKPolylineRenderer polylineRenderer = new MKPolylineRenderer ((MKPolyline) overlay);
                polylineRenderer.FillColor = UIColor.Blue;
                polylineRenderer.StrokeColor = UIColor.Blue;
                polylineRenderer.LineWidth = 5f;
                return polylineRenderer;
            }
            return null;
        }

    }
