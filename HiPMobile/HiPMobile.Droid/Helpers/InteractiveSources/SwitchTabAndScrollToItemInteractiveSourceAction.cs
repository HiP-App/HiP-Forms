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

using System;
using Android.Support.V4.View;
using Android.Support.V7.Widget;

namespace de.upb.hip.mobile.droid.Helpers.InteractiveSources
{
    /// <summary>
    /// Implements IInteractiveSourceAction by smoothly scrolling to the specified tab
    /// and afterwards smoothly scrolling to the source inside a recycler view.
    /// </summary>
    public class SwitchTabAndScrollToItemInteractiveSourceAction : IInteractiveSourceAction
    {
        /// <summary>
        /// Index of the tab where the references are displayed
        /// </summary>
        public int TargetTabIndex { get; set; }

        /// <summary>
        /// Function for getting the tab viewpager used for scrolling to the specified tab
        /// </summary>
        public Func<ViewPager> GetTabsViewPagers { get; set; }

        /// <summary>
        /// Function for getting the recyclerview used for scrolling to the source
        /// </summary>
        public Func<RecyclerView> GetRecyclerView { get; set; }

        private Source source;

        public void Display(Source src)
        {
            source = src;

            GetTabsViewPagers().PageScrollStateChanged += TabsOnPageScrollStateChanged;
            GetTabsViewPagers().SetCurrentItem(TargetTabIndex, true);
        }

        /// <summary>
        /// Used to start scrolling to the recyclerview element after scrolling to the tab has finished
        /// </summary>
        private void TabsOnPageScrollStateChanged (object sender, ViewPager.PageScrollStateChangedEventArgs args)
        {
            if (args.State == ViewPager.ScrollStateIdle)
            {
                GetRecyclerView().SmoothScrollToPosition (source.NumberInSubtitles);
                GetTabsViewPagers().PageScrollStateChanged -= TabsOnPageScrollStateChanged;
            }
        }
    }
}