// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CoreLocation;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using MapKit;
using PaderbornUniversity.SILab.Hip.Mobile.Ios.Map;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Routing;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Map;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.JoinClasses;

[assembly: ExportRenderer(typeof(OsmMap), typeof(IosMapRenderer))]

namespace PaderbornUniversity.SILab.Hip.Mobile.Ios.Map
{
    class IosMapRenderer : ViewRenderer<OsmMap, MKMapView>
    {
        private OsmMap osmMap;
        private UserAnnotation userAnnotation;
        private RouteCalculator routeCalculator;
        private MKPolyline currentSectionPolyLine;
        private MKPolyline navigationPolyline;
        MKCoordinateRegion backupRegion;

        protected override void OnElementChanged(ElementChangedEventArgs<OsmMap> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                // set up the native control
                var mapView = new MKMapView();
                SetNativeControl(mapView);
                Control.ShowsCompass = true;

                mapView.OverlayRenderer = OverlayRenderer;
                mapView.RegionChanged += MapViewOnRegionChanged;
            }

            if (routeCalculator == null)
            {
                routeCalculator = RouteCalculator.Instance;
            }

            if (e.OldElement != null)
            {
                // remove connections to the old instance
                Control.GetViewForAnnotation = null;
                Control.CalloutAccessoryControlTapped -= OnCalloutAccessoryControlTapped;
                e.OldElement.GpsLocationChanged -= OnGpsLocationChanged;
                e.OldElement.ExhibitSetChanged -= OnExhibitSetChanged;
                e.OldElement.DetailsRouteChanged -= OnDetailsRouteChanged;
                e.OldElement.CenterLocationCalled -= CenterLocation;
            }

            if (e.NewElement != null)
            {
                // setup connections to the new instance
                osmMap = e.NewElement;
                InitAnnotations(e.NewElement.ExhibitSet, e.NewElement.DetailsRoute);
                Control.GetViewForAnnotation = GetViewForAnnotation;
                Control.CalloutAccessoryControlTapped += OnCalloutAccessoryControlTapped;
                e.NewElement.GpsLocationChanged += OnGpsLocationChanged;
                OnGpsLocationChanged(osmMap.GpsLocation);
                e.NewElement.ExhibitSetChanged += OnExhibitSetChanged;
                e.NewElement.DetailsRouteChanged += OnDetailsRouteChanged;
                OnDetailsRouteChanged(osmMap.DetailsRoute);
                e.NewElement.CenterLocationCalled += CenterLocation;
                InitMapPosition();
                if (e.NewElement.ShowNavigation)
                {
                    UpdateRoute();
                }
            }
        }

        private void MapViewOnRegionChanged(object sender, MKMapViewChangeEventArgs mkMapViewChangeEventArgs)
        {
            Console.WriteLine(GetZoomLevel());

            if (GetZoomLevel() > 17.3f)
            {
                Control.SetRegion(backupRegion, true);
            }
            else
            {
                backupRegion = Control.Region;
            }
        }

        /// <summary>
        /// Centers the map on a given lcoation if available. Otherwise the center of paderborn is centered.
        /// </summary>
        /// <param name="location">The location to center on.</param>
        private void CenterLocation(GeoLocation? location)
        {
            Control.CenterCoordinate = location != null
                ? new CLLocationCoordinate2D(location.Value.Latitude, location.Value.Longitude)
                : new CLLocationCoordinate2D(AppSharedData.PaderbornCenter.Latitude, AppSharedData.PaderbornCenter.Longitude);
        }

        /// <summary>
        /// React to changes in the route.
        /// </summary>
        /// <param name="route">The route that changed</param>
        private void OnDetailsRouteChanged(Route route)
        {
            if (route != null && osmMap.ShowDetailsRoute)
            {
                IList<CLLocationCoordinate2D> waypoints = new List<CLLocationCoordinate2D>();

                // add user location and route locations
                if (osmMap.GpsLocation != null)
                {
                    waypoints.Add(new CLLocationCoordinate2D(osmMap.GpsLocation.Value.Latitude, osmMap.GpsLocation.Value.Longitude));
                }
                waypoints = waypoints.Concat(route.Waypoints.Select(waypoint => new CLLocationCoordinate2D(waypoint.Location.Latitude, waypoint.Location.Longitude))).ToList();

                var polyline = MKPolyline.FromCoordinates(waypoints.ToArray());
                Control.AddOverlay(polyline);
            }
        }

        /// <summary>
        /// React to changes in the exhibitset.
        /// </summary>
        /// <param name="set">The exhibitset that changed.</param>
        private void OnExhibitSetChanged(IReadOnlyList<Exhibit> set)
        {
            InitAnnotations(set, osmMap.DetailsRoute);
        }

        /// <summary>
        /// React to changes of the gps position.
        /// </summary>
        /// <param name="location">The position that changed.</param>
        private void OnGpsLocationChanged(GeoLocation? location)
        {
            if (location != null)
            {
                // update user location
                if (userAnnotation != null)
                {
                    Control.RemoveAnnotation(userAnnotation);
                }
                userAnnotation = new UserAnnotation(location.Value.Latitude, location.Value.Longitude);
                Control.AddAnnotation(userAnnotation);

                if (osmMap.ShowNavigation)
                {
                    UpdateRoute();
                }
            }
        }

        /// <summary>
        /// Update the displayed route. Route calculation is done in the background thread.
        /// </summary>
        private void UpdateRoute()
        {
            var id = osmMap.DetailsRoute.Id;

            ThreadPool.QueueUserWorkItem(state =>
            {
                Action action;

                try
                {
                    var locations = routeCalculator.CreateOrderedRoute(id, osmMap.GpsLocation);

                    /*foreach (var w in locations)
                    {
                        var point = new CLLocationCoordinate2D(w.Latitude, w.Longitude);
                        geoPoints.Add(point);
                    }*/

                    action = () => DrawRoute(locations, osmMap.GpsLocation != null);
                }
                catch (Exception)
                {
                    action = () => { };
                }

                Device.BeginInvokeOnMainThread(() => { action.Invoke(); });
            });
        }

        /// <summary>
        /// Draw a route between the given geopoints.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="userLocationAvailable"></param>
        private void DrawRoute(OrderedRoute route, bool userLocationAvailable)
        {
            if (disposed)
                return;

            if (navigationPolyline != null)
            {
                Control.RemoveOverlay(navigationPolyline);
            }
            if (currentSectionPolyLine != null)
            {
                Control.RemoveOverlay(currentSectionPolyLine);
            }
            if (userLocationAvailable)
            {
                navigationPolyline = MKPolyline.FromCoordinates(route.NonFirstSections.Select(gl => new CLLocationCoordinate2D(gl.Latitude, gl.Longitude)).ToArray());
                Control.AddOverlay(navigationPolyline);
                currentSectionPolyLine = MKPolyline.FromCoordinates(route.FirstSection.Select(gl => new CLLocationCoordinate2D(gl.Latitude, gl.Longitude)).ToArray());
                Control.AddOverlay(currentSectionPolyLine);
            }
            else
            {
                navigationPolyline = MKPolyline.FromCoordinates(route.Locations.Select(gl => new CLLocationCoordinate2D(gl.Latitude, gl.Longitude)).ToArray());
                Control.AddOverlay(navigationPolyline);
            }
        }

        /// <summary>
        /// Init the map position by centering on teh user or paderborn center and setting the zoom level.
        /// </summary>
        private void InitMapPosition()
        {
            CLLocationCoordinate2D center;
            if (osmMap.GpsLocation != null)
            {
                center = new CLLocationCoordinate2D(osmMap.GpsLocation.Value.Latitude, osmMap.GpsLocation.Value.Longitude);
            }
            else
            {
                center = new CLLocationCoordinate2D(AppSharedData.PaderbornCenter.Latitude, AppSharedData.PaderbornCenter.Longitude);
            }
            Control.SetRegion(MKCoordinateRegion.FromDistance(center, 1000, 1000), true);
        }

        /// <summary>
        /// Gets the view for an annotation.
        /// </summary>
        /// <param name="mapView">The mapview instance.</param>
        /// <param name="annotation">The annotation to get the view for.</param>
        /// <returns>The annotation view.</returns>
        private MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
        {
            MKAnnotationView annotationView;
            MKAnnotationView dequedView;

            if (annotation is UserAnnotation) //(annotation is MKUserLocation) doesn't work
            {
                const string userAnnotationReusableId = "UserAnnotation";
                dequedView = mapView.DequeueReusableAnnotation(userAnnotationReusableId);
                if (dequedView != null)
                {
                    return dequedView;
                }

                annotationView = new UserAnnotationView(annotation, userAnnotationReusableId);
                return annotationView;
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
                annotationView = new ExhibitAnnotationView(annotation, annotationReusableId);
            }
            annotationView.CanShowCallout = true;
            return annotationView;
        }

        /// <summary>
        /// Gets the overlay renderer for the given overlay.
        /// </summary>
        /// <param name="mapView">The instance of the mapview.</param>
        /// <param name="overlay">The instance of the overlay.</param>
        /// <returns>The corresponding overlay renderer.</returns>
        private MKOverlayRenderer OverlayRenderer(MKMapView mapView, IMKOverlay overlay)
        {
            if (ObjCRuntime.Runtime.GetNSObject(overlay.Handle) is MKTileOverlay tileOverlay)
            {
                var renderer = new MKTileOverlayRenderer(tileOverlay);
                return renderer;
            }

            if (overlay is MKPolyline polyline)
            {
                var resources = IoCManager.Resolve<ApplicationResourcesProvider>();

                MKPolylineRenderer polylineRenderer;
                if (polyline.Equals(currentSectionPolyLine))
                {
                    var color = ((Color)resources.GetResourceValue("SecondaryColor")).ToUIColor();
                    polylineRenderer = new MKPolylineRenderer(polyline)
                    {
                        FillColor = color,
                        StrokeColor = color,
                        LineWidth = 2f
                    };
                }
                else
                {
                    var color = ((Color)resources.GetResourceValue("PrimaryColor")).ToUIColor();
                    polylineRenderer = new MKPolylineRenderer(polyline)
                    {
                        FillColor = color,
                        StrokeColor = color,
                        LineWidth = 2f
                    };
                }
                return polylineRenderer;
            }
            return null;
        }

        /// <summary>
        /// Callback for when the user taps a callout.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event parameters.</param>
        private void OnCalloutAccessoryControlTapped(object sender, MKMapViewAccessoryTappedEventArgs e)
        {
            var exhibitAnnotationView = e.View as ExhibitAnnotationView;
            if (exhibitAnnotationView?.Annotation is ExhibitAnnotation annotation)
            {
                var exhibitId = annotation.ExhibitId;
                IoCManager.Resolve<INavigationService>().PushAsync(new AppetizerPageViewModel(exhibitId));
            }
        }

        /// <summary>
        /// Init the annotations on the map from a set of exhibits anr/or a route.
        /// </summary>
        /// <param name="exhibitSet">The set of exhibits.</param>
        /// <param name="route">The route.</param>
        private void InitAnnotations(IReadOnlyList<Exhibit> exhibitSet, Route route)
        {
            if (exhibitSet != null)
            {
                foreach (var exhibit in exhibitSet)
                {
                    var annotation = new ExhibitAnnotation(
                        exhibit.Location.Latitude, exhibit.Location.Longitude, exhibit);
                    Control.AddAnnotation(annotation);
                }
            }

            if (route != null)
            {
                foreach (Waypoint routeWaypoint in route.Waypoints)
                {
                    if (exhibitSet == null || !exhibitSet.Contains(routeWaypoint.Exhibit))
                    {
                        var annotation = new ExhibitAnnotation(
                            routeWaypoint.Location.Latitude, routeWaypoint.Location.Longitude, routeWaypoint.Exhibit);
                        Control.AddAnnotation(annotation);
                    }
                }
            }

            if (osmMap.GpsLocation != null)
            {
                userAnnotation = new UserAnnotation(osmMap.GpsLocation.Value.Latitude, osmMap.GpsLocation.Value.Longitude);
            }
        }

        public double GetZoomLevel()
        {
            var longitudeDelta = Control.Region.Span.LongitudeDelta;
            var mapWidthInPixels = (float)Control.Bounds.Size.Width;
            var zoomScale = longitudeDelta * 85445659.44705395 * Math.PI / (180.0 * mapWidthInPixels);
            var zoomer = 20 - Math.Log(zoomScale);
            if (zoomer < 0)
                zoomer = 0;
            //  zoomer = round(zoomer);
            return zoomer;
        }

        private bool disposed;

        /// <summary>
        /// Disposes this view.
        /// </summary>
        /// <param name="disposing">Indicator if the view is actually disposing.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                disposed = true;

                Control.CalloutAccessoryControlTapped -= OnCalloutAccessoryControlTapped;
                osmMap.GpsLocationChanged -= OnGpsLocationChanged;
                osmMap.ExhibitSetChanged -= OnExhibitSetChanged;
                osmMap.DetailsRouteChanged -= OnDetailsRouteChanged;
                osmMap.CenterLocationCalled -= CenterLocation;
            }

            base.Dispose(disposing);
        }
    }
}