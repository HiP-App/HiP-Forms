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


using Android.App;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Text.Method;
using Android.Views;
using Android.Widget;

namespace de.upb.hip.mobile.droid.Activities {
    [Activity (Theme = "@style/AppTheme", Label = "LicensingActivity", MainLauncher = false, Icon = "@drawable/icon")]
    public class LicensingActivity : AppCompatActivity {

        protected override void OnCreate (Bundle savedInstanceState)
        {
            base.OnCreate (savedInstanceState);
            SetContentView (Resource.Layout.activity_licensing);       

            MakeLinksClickable ();

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = Resources.GetString(Resource.String.license_name);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            //TODO: uncomment this when BaseActivity is ported
            // base.SetUpNavigationDrawer (this, mDrawerLayout);
        }


        /// <summary>
        ///     Make URL links from the licensing information clickable, so that they open in brower when the user link on them
        /// </summary>
        private void MakeLinksClickable ()
        {
            ((TextView) FindViewById (Resource.Id.licensingGoogleMaterialBody)).MovementMethod = LinkMovementMethod.Instance;
            ((TextView) FindViewById (Resource.Id.licensingOSMDroidBody)).MovementMethod = LinkMovementMethod.Instance;
            ((TextView) FindViewById (Resource.Id.licensingRealmBody)).MovementMethod = LinkMovementMethod.Instance;
            ((TextView) FindViewById (Resource.Id.licensingMapiconsBody)).MovementMethod = LinkMovementMethod.Instance;
            ((TextView) FindViewById (Resource.Id.licensingPhotoviewBody)).MovementMethod = LinkMovementMethod.Instance;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                //TODO: Comment this in again when RouteFilterActivity is ported
                case Android.Resource.Id.Home:
                    SupportFinishAfterTransition();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

    }
}