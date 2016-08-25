using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using de.upb.hip.mobile.droid.Activities;
using Java.Math;
using Org.Osmdroid.Bonuspack.Overlays;
using Org.Osmdroid.Util;
using Org.Osmdroid.Views;
using Object = Java.Lang.Object;

namespace de.upb.hip.mobile.droid.Helpers
{
    class SetMarker
    {

        private MapView mapView;
        private ViaPointInfoWindow markerInfoWindow;

        public SetMarker(MapView mapView, ViaPointInfoWindow markerInfoWindow)
        {
            this.mapView = mapView;
            this.markerInfoWindow = markerInfoWindow;
        }

        public Marker AddMarker(Marker marker, String title, String description, GeoPoint geoLocation, Drawable icon,
                                 Object markerId)   
        {
            if (marker == null)
            {

                marker = new Marker(mapView);
                marker.SetAnchor(Marker.AnchorCenter, Marker.AnchorBottom);
                marker.SetInfoWindow(markerInfoWindow);
                marker.Draggable = true;          
            }

            marker.Title = title;
            marker.Snippet = description;
            marker.Position = geoLocation;
            marker.SetIcon(icon);

            if (markerId != null)
            {
                marker.RelatedObject = markerId;
            }
            
            mapView.Invalidate();
            return marker;
        }
    }
}