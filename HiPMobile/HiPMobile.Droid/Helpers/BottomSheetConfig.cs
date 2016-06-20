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

using de.upb.hip.mobile.droid.Activities;
using de.upb.hip.mobile.droid.fragments.bottomsheetfragment;

namespace de.upb.hip.mobile.droid.Helpers {
    /// <summary>
    ///     Contains information for the <see cref="ExhibitDetailsActivity" /> on how to display the bottom sheet.
    /// </summary>
    public class BottomSheetConfig {

        /// <summary>
        ///     Describes the action the FAB should perform on click
        /// </summary>
        public enum FabAction {

            None,
            Expand,
            Collapse,
            Next

        }

        /// <summary>
        ///     Indicates whether the bottom sheet should be displayed (true) or not (false).
        /// </summary>
        public bool DisplayBottomSheet { get; set; } = true;

        /// <summary>
        ///     Fragment that is displayed in the bottom sheet.
        /// </summary>
        public BottomSheetFragment BottomSheetFragment { get; set; }

        /// <summary>
        ///     The maximum height of the bottom sheet (in dp).
        /// </summary>
        public int MaxHeight { get; set; } = 220;

        /// <summary>
        ///     The height of the bottom sheet when it is collapsed (in dp).
        /// </summary>
        public int PeekHeight { get; set; } = 80;

        /// <summary>
        ///     The action associated with the FAB.
        /// </summary>
        public FabAction fabAction { get; set; } = FabAction.Expand;

    }
}