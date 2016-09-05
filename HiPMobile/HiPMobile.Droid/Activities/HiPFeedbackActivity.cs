using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using HockeyApp;

namespace de.upb.hip.mobile.droid.Activities
{
    [Activity(Label = "Feedback Activity")]
    public class HiPFeedbackActivity : FeedbackActivity
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Toast.MakeText (this, "HiP Feedback Activity", ToastLength.Long).Show ();
;            // Create your application here
        }
    }
}