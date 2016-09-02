/*
 * Copyright (C) 2016 History in Paderborn App - Universität Paderborn
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Android.Content;
using Android.Util;
using Android.Widget;
using Org.Osmdroid;
using Org.Osmdroid.Tileprovider;
using Org.Osmdroid.Views;

namespace de.upb.hip.mobile.droid.Helpers {
    internal class GenericMapView : FrameLayout {

        protected MapView mMapView;

        public GenericMapView (Context context, IAttributeSet attrs) : base (context, attrs)
        {
        }

        /**
     * The TileProvider is set here.
     * The Context is set externally by the NavigationDrawer / MainActivity
     *
     * @param aTileProvider
     */

        public void SetTileProvider (MapTileProviderBase aTileProvider)
        {
            if (mMapView != null)
            {
                RemoveView (mMapView);
            }

            IResourceProxy resourceProxy = new DefaultResourceProxyImpl (Context);
            var newMapView = new MapView (Context, 1024, resourceProxy, aTileProvider);

            if (mMapView != null)
            {
                //restore as much parameters as possible from previous mMap:
                var mapController = newMapView.Controller;
                mapController.SetZoom (mMapView.ZoomLevel);
                mapController.SetCenter (mMapView.MapCenter);
                newMapView.SetBuiltInZoomControls (true); //no way to get old setting
                newMapView.SetMultiTouchControls (true); //no way to get old setting
                newMapView.SetUseDataConnection (mMapView.UseDataConnection ());
                newMapView.MapOrientation = mMapView.MapOrientation;
                newMapView.ScrollableAreaLimit = mMapView.ScrollableAreaLimit;
                var overlays = mMapView.Overlays;
                foreach (var o in overlays)
                {
                    newMapView.Overlays.Add (o);
                }
            }

            mMapView = newMapView;
            AddView (mMapView);
        }

        public MapView GetMapView ()
        {
            return mMapView;
        }

    }
}