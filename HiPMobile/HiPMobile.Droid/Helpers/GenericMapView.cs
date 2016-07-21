using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Org.Osmdroid;
using Org.Osmdroid.Api;
using Org.Osmdroid.Tileprovider;
using Org.Osmdroid.Views;
using Org.Osmdroid.Views.Overlay;


namespace de.upb.hip.mobile.droid.Helpers
{
    class GenericMapView : FrameLayout
    {

    protected MapView mMapView;

    public GenericMapView(Context context, IAttributeSet attrs) : base(context,attrs)
    {

    }

    /**
     * The TileProvider is set here.
     * The Context is set externally by the NavigationDrawer / MainActivity
     *
     * @param aTileProvider
     */
    public void SetTileProvider(MapTileProviderBase aTileProvider)
    {
        if (mMapView != null)
        {
            this.RemoveView(mMapView);
        }

        IResourceProxy resourceProxy = new DefaultResourceProxyImpl(this.Context);
        MapView newMapView = new MapView(this.Context,1024, resourceProxy, aTileProvider);

        if (mMapView != null)
        {
            //restore as much parameters as possible from previous mMap:
            IMapController mapController = newMapView.Controller;
            mapController.SetZoom(mMapView.ZoomLevel);
            mapController.SetCenter(mMapView.MapCenter);
            newMapView.SetBuiltInZoomControls(true); //no way to get old setting
            newMapView.SetMultiTouchControls(true); //no way to get old setting
            newMapView.SetUseDataConnection(mMapView.UseDataConnection());
            newMapView.MapOrientation = (mMapView.MapOrientation);
            newMapView.ScrollableAreaLimit = (mMapView.ScrollableAreaLimit);
            IList<Overlay> overlays = mMapView.Overlays;
            foreach (Overlay o in overlays)
            {
                newMapView.Overlays.Add(o);
            }
        }

        mMapView = newMapView;
        this.AddView(mMapView);
    }

    public MapView GetMapView()
    {
        return mMapView;
    }
}

}