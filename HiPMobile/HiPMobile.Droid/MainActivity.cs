using System;
using Android.App;
using Android.OS;
using Android.Widget;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using Realms;

namespace de.upb.hip.mobile.droid {
    [Activity (Label = "HiPMobile.Droid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity {

        private int count = 1;

        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            var button = FindViewById<Button> (Resource.Id.myButton);

            //Realm.GetInstance ().RemoveAll ();

            button.Click += (sender, args) => {
                var exhibit = BusinessEntitiyFactory.CreateBusinessObject<Exhibit> ();
                button.Text = exhibit.Id;
            };
        }

    }
}