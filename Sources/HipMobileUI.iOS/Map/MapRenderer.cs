using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

[assembly: ExportRenderer(typeof(OsmMap), typeof(MapRenderer))]

namespace HipMobileUI.iOS.Map
{
    class MapRenderer : ViewRenderer<OsmMap, MKMapView> {

        private OsmMap osmMap;
        private UserAnnotation userAnnotation;
        private RouteCalculator routeCalculator;
        private MKPolyline navigationPolyline;

        protected override void OnElementChanged(ElementChangedEventArgs<OsmMap> e)
        {
            base.OnElementChanged(e);


            if (Control == null)
            {
                var mapView = new MKMapView ();
                this.SetNativeControl (mapView);

                var overlay = new MKTileOverlay ("http://tile.openstreetmap.org/{z}/{x}/{y}.png") {CanReplaceMapContent = true};
                mapView.AddOverlay (overlay, MKOverlayLevel.AboveLabels);
                mapView.OverlayRenderer = OverlayRenderer;

            }

            if (routeCalculator == null)
            {
                routeCalculator = RouteCalculator.Instance;
            }

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
                InitAnnotations(e.NewElement.ExhibitSet, e.NewElement.DetailsRoute);
                Control.GetViewForAnnotation = GetViewForAnnotation;
                Control.CalloutAccessoryControlTapped += OnCalloutAccessoryControlTapped;
                e.NewElement.GpsLocationChanged += OnGpsLocationChanged;
                OnGpsLocationChanged (osmMap.GpsLocation);
                e.NewElement.ExhibitSetChanged += OnExhibitSetChanged;
                e.NewElement.DetailsRouteChanged+=OnDetailsRouteChanged;
                OnDetailsRouteChanged (osmMap.DetailsRoute);
                e.NewElement.CenterLocationCalled+=CenterLocation;
            }

            Control.ShowsCompass = true;
            InitMapPosition (Control);
            
        }

        private void CenterLocation (GeoLocation location)
        {
            if (location != null)
            {
                Control.CenterCoordinate = new CLLocationCoordinate2D (location.Latitude, location.Longitude);
            }
        }


        private void OnDetailsRouteChanged (Route route)
        {
            if (route != null && osmMap.ShowDetailsRoute)
            {
                IList<CLLocationCoordinate2D> waypoints = new List<CLLocationCoordinate2D> ();

                // add user location and route locations
                if (osmMap.GpsLocation != null)
                {
                    waypoints.Add (new CLLocationCoordinate2D (osmMap.GpsLocation.Latitude, osmMap.GpsLocation.Longitude));
                }
                waypoints = waypoints.Concat(route.Waypoints.Select (waypoint => new CLLocationCoordinate2D (waypoint.Location.Latitude, waypoint.Location.Longitude))).ToList ();

                var polyline = MKPolyline.FromCoordinates(waypoints.ToArray ());
                Control.AddOverlay (polyline);
            }
        }

        private void OnExhibitSetChanged(ExhibitSet set)
        {
           InitAnnotations (set, osmMap.DetailsRoute);
        }

        private void OnGpsLocationChanged(GeoLocation location)
        {
            if (location != null)
            {
                Control.CenterCoordinate = new CLLocationCoordinate2D (location.Latitude, location.Longitude);

                // update user location
                if (userAnnotation != null)
                {
                    Control.RemoveAnnotation (userAnnotation);
                }
                userAnnotation = new UserAnnotation (location.Latitude, location.Longitude);
                Control.AddAnnotation (userAnnotation);

                if (osmMap.ShowNavigation)
                {
                    UpdateRoute (location);
                }
            }
        }

        private void UpdateRoute (GeoLocation location)
        {
            var id = osmMap.DetailsRoute.Id;

            ThreadPool.QueueUserWorkItem(state => {
                var geoPoints = new List<CLLocationCoordinate2D> { new CLLocationCoordinate2D(osmMap.GpsLocation.Latitude, osmMap.GpsLocation.Longitude) };

                Action action;

                try
                {
                    var locations = routeCalculator.CreateRouteWithSeveralWaypoints(new GeoLocation(osmMap.GpsLocation.Latitude, osmMap.GpsLocation.Longitude),
                                                                                     id);

                    foreach (var w in locations)
                    {
                        var point = new CLLocationCoordinate2D(w.Latitude, w.Longitude);
                        geoPoints.Add(point);
                    }

                    action = () => DrawRoute(geoPoints);
                }
                catch (Exception)
                {
                    action = ShowRouteCalculationError;
                }

                Device.BeginInvokeOnMainThread (() => {
                    action.Invoke ();
                });
            });
        }

        private void ShowRouteCalculationError ()
        {
            IoCManager.Resolve<INavigationService> ().DisplayAlert ("Fehler", "Sie befinden sich nicht innerhalb von Paderborn", "Ok");
        }

        private void DrawRoute (List<CLLocationCoordinate2D> geoPoints)
        {
            if (navigationPolyline != null)
            {
                Control.RemoveOverlay (navigationPolyline);
            }
            navigationPolyline = MKPolyline.FromCoordinates(geoPoints.ToArray ());
            Control.AddOverlay(navigationPolyline);
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
            if (annotation is UserAnnotation) //(annotation is MKUserLocation) doesn't work
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

        private MKOverlayRenderer OverlayRenderer(MKMapView mapView, IMKOverlay overlay)
        {
            var tileOverlay = ObjCRuntime.Runtime.GetNSObject(overlay.Handle) as MKTileOverlay;
            if (tileOverlay != null)
            {
                var renderer = new MKTileOverlayRenderer(tileOverlay);
                return renderer;
            }

            if (overlay is MKPolyline)
            {
                MKPolylineRenderer polylineRenderer = new MKPolylineRenderer((MKPolyline)overlay);
                polylineRenderer.FillColor = UIColor.Blue;
                polylineRenderer.StrokeColor = UIColor.Blue;
                polylineRenderer.LineWidth = 2f;
                return polylineRenderer;
            }
            return null;
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

        private void InitAnnotations(ExhibitSet exhibitSet, Route route)
        {
            if (exhibitSet != null)
            {
                foreach (var exhibit in exhibitSet)
                {
                    var annotation = new ExhibitAnnotation (exhibit.Location.Latitude, exhibit.Location.Longitude, exhibit.Id,
                                                            exhibit.Name, exhibit.Description);
                    Control.AddAnnotation (annotation);
                }
            }

            if (route != null)
            {
                foreach (Waypoint routeWaypoint in route.Waypoints)
                {
                    var annotation = new ExhibitAnnotation(routeWaypoint.Location.Latitude, routeWaypoint.Location.Longitude, routeWaypoint.Exhibit.Id,
                                                                                routeWaypoint.Exhibit.Name, routeWaypoint.Exhibit.Description);
                    Control.AddAnnotation(annotation);
                }
            }

            if (osmMap.GpsLocation != null)
            {
                userAnnotation = new UserAnnotation (osmMap.GpsLocation.Latitude, osmMap.GpsLocation.Longitude);
            }
        }

        protected override void Dispose (bool disposing)
        {
            if (disposing)
            {
                Control.CalloutAccessoryControlTapped -= OnCalloutAccessoryControlTapped;
                osmMap.GpsLocationChanged -= OnGpsLocationChanged;
                osmMap.ExhibitSetChanged -= OnExhibitSetChanged;
            }

            base.Dispose (disposing);
        }

    }
}