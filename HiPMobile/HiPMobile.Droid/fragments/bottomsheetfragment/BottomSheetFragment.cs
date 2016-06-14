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
using de.upb.hip.mobile.droid.Activities;

namespace de.upb.hip.mobile.droid.fragments.bottomsheetfragment
{
    /// <summary>
    /// Abstract class for fragments that are included in the bottom sheet of <see cref="ExhibitDetailsActivity"/>
    /// </summary>
    public abstract class BottomSheetFragment : Fragment
    {
        /// <summary>
        /// Called by ExhibitDetailsActivity when the bottom sheet has been expanded.
        /// Subclasses should override this method if they require individual behaviour
        /// </summary>
        public virtual void OnBottomSheetExpand()
        {
            // intentionally left blank
        }

        /// <summary>
        /// Called by ExhibitDetailsActivity when the bottom sheet has been collapsed.
        /// Subclasses should override this method if they require individual behaviour.
        /// </summary>
        public virtual void OnBottomSheetCollapse()
        {
            // intentionally left blank
        }
    }
}