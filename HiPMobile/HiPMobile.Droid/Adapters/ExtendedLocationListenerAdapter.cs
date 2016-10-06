using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace de.upb.hip.mobile.droid.Adapters
{
    public interface ExtendedLocationListenerAdapter
    {
        void LocationChanged(Location location);
    }
}