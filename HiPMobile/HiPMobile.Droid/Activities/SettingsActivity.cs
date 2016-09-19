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
using Android.Support.V7.App;
using Android.Views;
using de.upb.hip.mobile.droid.fragments;

namespace de.upb.hip.mobile.droid.Activities {

    [Activity (Label = "@string/title_activity_settings", Theme = "@style/AppTheme.Settings")]
    public class SettingsActivity : AppCompatActivity {

        protected override void OnCreate (Bundle savedInstanceState)
        {
            base.OnCreate (savedInstanceState);

            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);

            // add settings fragment
            FragmentManager.BeginTransaction ()
                           .Replace (Android.Resource.Id.Content, new SettingsFragment ())
                           .Commit ();

            // add back button to action bar
            SupportActionBar?.SetDisplayHomeAsUpEnabled(true);

        }

        public override bool OnOptionsItemSelected (IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    SupportFinishAfterTransition();
                    break;
                default:
                    return base.OnOptionsItemSelected(item);
            }
 
            return true;
        }

    }
}