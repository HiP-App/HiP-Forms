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
using Android.Views;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;

namespace de.upb.hip.mobile.droid.fragments
{
    public class ExhibitsOverviewFragment : Fragment
    {

        /// <summary>
        /// ExhibitSet containing the exhibit that should be displayed in the RecyclerView.
        /// </summary>
        public ExhibitSet ExhibitSet { get; set; }

        /// <summary>
        /// GeoLocation for the current position of the user.
        /// </summary>
        public GeoLocation GeoLocation { get; set; }


        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_overview_exhibits, container, false);

            ExhibitListFragment exhibitListFragment = new ExhibitListFragment
            {
                ExhibitSet = ExhibitSet,
                GeoLocation = GeoLocation
            };

            // remove old fragment and display new fragment
            if (view.FindViewById(Resource.Id.overview_map_fragment_container) != null)
            {
                var transaction = FragmentManager.BeginTransaction();
                transaction.Add (Resource.Id.overview_map_fragment_container, exhibitListFragment);
                transaction.Commit();
            }

            MapFragment mapFragment = new MapFragment
            {
                ExhibitSet = ExhibitSet,
                GeoLocation = GeoLocation
            };

            // remove old fragment and display new fragment
            if (view.FindViewById(Resource.Id.overview_exhibitlist_fragment_container) != null)
            {
                var transaction = FragmentManager.BeginTransaction();
                transaction.Add(Resource.Id.overview_exhibitlist_fragment_container, mapFragment);
                transaction.Commit();
            }

            return view;
        }
    }
}