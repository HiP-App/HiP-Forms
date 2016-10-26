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

using Android.Media;
using Android.OS;
using Android.Views;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using Java.IO;
using Console = System.Console;
using Fragment = Android.Support.V4.App.Fragment;

namespace de.upb.hip.mobile.droid.fragments {
    /// <summary>
    /// Fragment displaying a map and a list for an ExhibitSet.
    /// </summary>
    public class ExhibitsOverviewFragment : Fragment {

        /// <summary>
        /// ExhibitSet containing the exhibit that should be displayed in the RecyclerView.
        /// </summary>
        public ExhibitSet ExhibitSet { get; set; }

        /// <summary>
        /// GeoLocation for the current position of the user.
        /// </summary>
        public GeoLocation GeoLocation { get; set; }

        public MapFragment MapFragment { get; private set; }

        public ExhibitListFragment ExhibitListFragment { get; private set; }

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
            this.RetainInstance = true;
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
            var view = inflater.Inflate (Resource.Layout.fragment_overview_exhibits, container, false);

            if (savedInstanceState == null)
            {
                MapFragment = new MapFragment
                {
                    ExhibitSet = ExhibitSet,
                    GeoLocation = GeoLocation
                };

                // remove old fragment and display new fragment
                if (view.FindViewById (Resource.Id.overview_map_fragment_container) != null)
                {
                    var transaction = FragmentManager.BeginTransaction ();
                    transaction.Add (Resource.Id.overview_map_fragment_container, MapFragment);
                    transaction.Commit ();
                }

                ExhibitListFragment = new ExhibitListFragment
                {
                    ExhibitSet = ExhibitSet,
                    GeoLocation = GeoLocation
                };

                // remove old fragment and display new fragment
                if (view.FindViewById (Resource.Id.overview_exhibitlist_fragment_container) != null)
                {
                    var transaction = FragmentManager.BeginTransaction ();
                    transaction.Add (Resource.Id.overview_exhibitlist_fragment_container, ExhibitListFragment);
                    transaction.Commit ();
                }
            }

            return view;
        }

    }
}