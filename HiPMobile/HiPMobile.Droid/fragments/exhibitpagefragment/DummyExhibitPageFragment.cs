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

using Android.OS;
using Android.Views;
using Android.Widget;
using de.upb.hip.mobile.droid.fragments.bottomsheetfragment;
using de.upb.hip.mobile.droid.Helpers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;

namespace de.upb.hip.mobile.droid.fragments.exhibitpagefragment {
    public class DummyExhibitPageFragment : ExhibitPageFragment {

        // for testing purposes
        private static int count = 0;

        public override BottomSheetConfig GetBottomSheetConfig ()
        {
            var bsFragment = new SimpleBottomSheetFragment ();
            //        bsFragment.setTitle("SimpleBottomSheetFragment #" + count);
            bsFragment.Title = "Außenansicht";
            bsFragment.Description =
                "Beschreibung zur Außenansicht (ist eigentlich der Abdinghof...). \n\n" +
                "(c) XYZ   \n\nyou cannot use getString(id) here because the fragment is not " +
                "yet attached to an Activity!...";
            var bottomSheetConfig = new BottomSheetConfig
            {
                BottomSheetFragment = bsFragment
            };
            return bottomSheetConfig;
        }

        public override void SetPage (Page page)
        {
            // intentionally left blank for this dummy implementation
        }

        public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Inflate the layout for this fragment
            var view = inflater.Inflate (Resource.Layout.fragment_exhibitpage_dummy, container, false);
            var mImageView = (ImageView) view.FindViewById (Resource.Id.imageView2);
            //new PhotoViewAttacher(mImageView); // TODO Replace this?
            return view;
        }

    }
}