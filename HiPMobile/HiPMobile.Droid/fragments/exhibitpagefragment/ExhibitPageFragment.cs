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


using Android.Support.V4.App;
using de.upb.hip.mobile.droid.Helpers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;

namespace de.upb.hip.mobile.droid.fragments.exhibitpagefragment {
    /// <summary>
    ///     Abstract superclass for Fragments that are displayed as pages in the ExhibitDetailsActivity.
    /// </summary>
    public abstract class ExhibitPageFragment : Fragment {

        /// <summary>
        ///     Returns the BottomSheetConfig for the PageFragment.
        /// </summary>
        /// <returns></returns>
        public abstract BottomSheetConfig GetBottomSheetConfig ();

        /// <summary>
        ///     Sets the instance of the model class.
        /// </summary>
        /// <param name="page"></param>
        public abstract void SetPage (Page page);

    }
}