using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Gms.Maps;
using HipMobileUI.Droid.Map;
using HipMobileUI.Map;
using Xamarin.Forms;
using Xamarin.Forms.Maps.Android;
using Xamarin.Forms.Platform.Android;
using Org.Osmdroid.Views;

[assembly: ExportRenderer(typeof(CustomMap), typeof(DroidOsmMapRenderer))]
namespace HipMobileUI.Droid.Map
{
    class DroidOsmMapRenderer : MapRenderer
    {

        protected override void OnElementChanged (ElementChangedEventArgs<Xamarin.Forms.Maps.Map> e)
        {
            base.OnElementChanged (e);

            if (e.OldElement != null)
            {
                
            }

            if (e.NewElement != null)
            {
                var control = this.Control;
            }
        }

    }
}