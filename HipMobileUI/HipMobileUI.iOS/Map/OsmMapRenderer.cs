// Copyright (C) 2016 History in Paderborn App - Universität Paderborn
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

using HipMobileUI.iOS.Map;
using HipMobileUI.Map;
using MapKit;
using Xamarin.Forms;
using Xamarin.Forms.Maps.iOS;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomMap), typeof(OsmMapRenderer))]
namespace HipMobileUI.iOS.Map {
    public class OsmMapRenderer : MapRenderer{
        
        protected override void OnElementChanged (ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged (e);

            if (e.OldElement != null)
            {
            }

            if (e.NewElement != null)
            {
                var formsMap = (CustomMap)e.NewElement;
                MKMapView nativeMap = (MKMapView)Control;
                var overlay = new MKTileOverlay ("http://tile.openstreetmap.org/{z}/{x}/{y}.png");
                overlay.CanReplaceMapContent = true;
                nativeMap.AddOverlay(overlay, MKOverlayLevel.AboveLabels);
                nativeMap.OverlayRenderer = (view, mkOverlay) => new MKTileOverlayRenderer (overlay);
            }
        }

    }
}