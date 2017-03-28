// Copyright (C) 2016 History in Paderborn App - Universität Paderborn
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//       http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Android.Graphics.Drawables;
using Org.Osmdroid.Bonuspack.Overlays;
using Org.Osmdroid.Util;
using Org.Osmdroid.Views;
using Object = Java.Lang.Object;

namespace PaderbornUniversity.SILab.Hip.Mobile.Droid.Map
{
    class SetMarker
    {

        private readonly MapView mapView;
        private readonly ViaPointInfoWindow markerInfoWindow;

        public SetMarker(MapView mapView, ViaPointInfoWindow markerInfoWindow)
        {
            this.mapView = mapView;
            this.markerInfoWindow = markerInfoWindow;
        }

        public Marker AddMarker(Marker marker, string title, string description, GeoPoint geoLocation, Drawable icon,
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