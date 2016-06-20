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
using de.upb.hip.mobile.droid.Helpers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using Java.Lang;

namespace de.upb.hip.mobile.droid.fragments.exhibitpagefragment {
    public class TextExhibitPageFragment : ExhibitPageFragment {

        private TextPage page;

        public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Inflate the layout for this fragment
            var v = inflater.Inflate (Resource.Layout.fragment_exhibitpage_text, container, false);

            var textView = (TextView) v.FindViewById (Resource.Id.tvText);
            textView.Text = page.Text;

            return v;
        }

        public override BottomSheetConfig GetBottomSheetConfig ()
        {
            var bottomSheetConfig = new BottomSheetConfig {DisplayBottomSheet = false};
            return bottomSheetConfig;
        }

        public override void SetPage (Page page)
        {
            if (page.IsTextPage ())
                this.page = page.TextPage;
            else
                throw new IllegalArgumentException ("Page has to be an instance of TextPage!");
        }

    }
}