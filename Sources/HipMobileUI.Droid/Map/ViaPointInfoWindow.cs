using System;
using Android.Content;
using Android.Widget;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using Org.Osmdroid.Bonuspack.Overlays;
using Org.Osmdroid.Views;
using Object = Java.Lang.Object;

namespace de.upb.hip.mobile.droid.Map {
    class ViaPointInfoWindow : MarkerInfoWindow {

        private string markerId;

        public ViaPointInfoWindow (int layoutResId, MapView mapView, Context context) : base (layoutResId, mapView)
        {
            Button infoButton = this.View.FindViewById<Button> (Resource.Id.bubble_info);

            infoButton.Click += (sender, e) => {
                if (markerId != null)
                {
                   //TODO add connection to exhibitdetails
                    
                }
            };
        }

        public override void OnOpen (Object item)
        {
            Marker marker = (Marker) item;
            markerId
                = (
                string
                )
                marker.RelatedObject;

            base.
                OnOpen (item);
        }

    }
}