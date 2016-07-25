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
using Android.Support.V7.Widget;
using Android.Views;
using de.upb.hip.mobile.droid.Activities;
using de.upb.hip.mobile.droid.Adapters;
using de.upb.hip.mobile.droid.Listeners;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using Fragment = Android.Support.V4.App.Fragment;

namespace de.upb.hip.mobile.droid.fragments {
    /// <summary>
    /// Fragment displaying a list of exhibits contained in an ExhibitSet.
    /// </summary>
    public class ExhibitListFragment : Fragment {

        /// <summary>
        /// ExhibitSet containing the exhibit that should be displayed in the RecyclerView.
        /// </summary>
        public ExhibitSet ExhibitSet { get; set; }

        /// <summary>
        /// GeoLocation for the current position of the user.
        /// </summary>
        public GeoLocation GeoLocation { get; set; }

        #region

        // Keys to save/restore instance state.
        private const string KeyExhibitSetId = "ExhibitSetId";
        private const string KeyGeoLocationLatitude = "GeoLocation.Latitude";
        private const string KeyGeoLocationLongitude = "GeoLocation.Longitude";

        #endregion

        public override void OnSaveInstanceState (Bundle outState)
        {
            base.OnSaveInstanceState (outState);

            outState.PutString (KeyExhibitSetId, ExhibitSet.Id);
            outState.PutDouble (KeyGeoLocationLatitude, GeoLocation.Latitude);
            outState.PutDouble (KeyGeoLocationLongitude, GeoLocation.Longitude);
        }

        public override void OnCreate (Bundle savedInstanceState)
        {
            base.OnCreate (savedInstanceState);

            if (savedInstanceState != null)
            {
                var latitude = savedInstanceState.GetDouble (KeyGeoLocationLatitude);
                var longitude = savedInstanceState.GetDouble (KeyGeoLocationLongitude);
                GeoLocation = new GeoLocation
                {
                    Latitude = latitude,
                    Longitude = longitude
                };

                var exhibitId = savedInstanceState.GetString (KeyExhibitSetId);
                ExhibitSet = ExhibitManager.GetExhibitSet (exhibitId);
            }
        }

        public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate (Resource.Layout.fragment_exhibitlist, container, false);

            var recyclerView = (RecyclerView) view.FindViewById (Resource.Id.exhibitListRecyclerView);

            // use a linear layout manager
            RecyclerView.LayoutManager layoutManager = new LinearLayoutManager (this.Activity);
            recyclerView.SetLayoutManager (layoutManager);

            //RecycleAdapter
            var adapter = new MainRecyclerAdapter (ExhibitSet, GeoLocation, Application.Context);
            recyclerView.SetAdapter (adapter);

            recyclerView.AddOnItemTouchListener (new RecyclerItemClickListener ((MainActivity) this.Activity, ExhibitSet));

            // Disable refreshing
            var swipeRefreshLayout = view.FindViewById<SwipeRefreshLayout> (Resource.Id.exhibitListSwipeContainer);
            swipeRefreshLayout.Enabled = false;

            return view;
        }

    }
}