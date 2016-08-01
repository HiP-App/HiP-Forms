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

using Android.Support.Design.Widget;
using Android.Views;

namespace de.upb.hip.mobile.droid.Helpers.InteractiveSources {
    /// <summary>
    /// Implements IInteractiveSourceAction by displaying the source's text
    /// in a Snackbar.
    /// </summary>
    public class SnackbarInteractiveSourceAction : IInteractiveSourceAction {

        private readonly View view;

        /// <summary>
        /// Sets the view the Snackbar is associated with. An instance of 
        /// CoordinatorLayout is recommended.
        /// </summary>
        /// <param name="v">View the Snackbar is associated with.</param>
        public SnackbarInteractiveSourceAction (View v)
        {
            view = v;
        }

        public void Display (string src)
        {
            Snackbar.Make (view, src, Snackbar.LengthLong).Show ();
        }

    }
}