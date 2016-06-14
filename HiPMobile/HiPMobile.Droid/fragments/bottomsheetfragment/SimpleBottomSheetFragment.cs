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

namespace de.upb.hip.mobile.droid.fragments.bottomsheetfragment
{
    public class SimpleBottomSheetFragment : BottomSheetFragment
    {
        /// <summary>
        /// Title displayed in the bottom sheet (should be ~30 characters long).
        /// </summary>
        public string Title { set; private get; } = "default title";

        /// <summary>
        /// Description displayed in the bottom sheet.
        /// </summary>
        public string Description { set; private get; } = "default description";

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Inflate the layout for this fragment
            View v = inflater.Inflate(Resource.Layout.fragment_bottom_sheet_simple, container, false);

            // set title and description
            TextView tv = (TextView)v.FindViewById(Resource.Id.tvBsTitle);
            tv.Text = Title;
            tv = (TextView)v.FindViewById(Resource.Id.tvBsDescription);
            tv.Text = Description;

            return v;
        }

        public override void OnBottomSheetExpand()
        {
            base.OnBottomSheetExpand();

            // TODO:  flash scrollbars of NestedScrollView if possible?
        }
    }
}