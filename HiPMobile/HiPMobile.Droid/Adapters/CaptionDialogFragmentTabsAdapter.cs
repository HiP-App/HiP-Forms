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

using System.Collections.Generic;
using Android.Support.V4.App;
using de.upb.hip.mobile.droid.fragments;
using Java.Lang;

namespace de.upb.hip.mobile.droid.Adapters
{
    public class CaptionDialogFragmentTabsAdapter : FragmentPagerAdapter
    {
        private readonly List<CaptionDialogFragment> fragments;

        public CaptionDialogFragmentTabsAdapter(FragmentManager fragmentManager, List<CaptionDialogFragment> fragments) : base(fragmentManager)
        {
            this.fragments = fragments;
        }

        public override int Count => fragments.Count;

        public override Fragment GetItem(int position)
        {
            return fragments[position];
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            return new String(fragments[position].Title);
        }

    }
}