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

using Android.Util;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;

namespace de.upb.hip.mobile.droid.fragments.exhibitpagefragment {
    public class ExhibitPageFragmentFactory {

        /// <summary>
        ///     Creates an ExhibitPageFragment for the specified Page.
        /// </summary>
        /// <param name="page">Page to create an ExhibitPageFragment for.</param>
        /// <param name="exhibitName">Name of the exhibit.</param>
        /// <returns>the created ExhibitPageFragment.</returns>
        public static ExhibitPageFragment GetFragmentForExhibitPage (Page page, string exhibitName)
        {
            // TODO: update this when new pages are available
            if (page.IsAppetizerPage ())
            {
                var fragment = new AppetizerExhibitPageFragment ();
                fragment.SetPage (page);
                if (!string.IsNullOrEmpty (exhibitName))
                    fragment.AppetizerTitle = exhibitName;
                return fragment;
            }
            if (page.IsTextPage ())
            {
                var fragment = new TextExhibitPageFragment ();
                fragment.SetPage (page);
                return fragment;
            }
            if (page.IsTimeSliderPage ())
            {
                var fragment = new TimeSliderExhibitPageFragment ();
                fragment.SetPage (page);
                return fragment;
            }
            if (page.IsImagePage ())
            {
                var fragment = new ImagePageExhibitFragment ();
                fragment.SetPage (page);
                return fragment;
            }
            Log.Info ("PageFragmentFactory", "Got unknown type of page: " + page
                                             + " for exhibit " + exhibitName);
            return new DummyExhibitPageFragment ();
        }

    }
}