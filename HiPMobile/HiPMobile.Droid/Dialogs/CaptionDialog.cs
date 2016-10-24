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
using System.Collections.Generic;
using System.Linq;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using de.upb.hip.mobile.droid.Adapters;
using de.upb.hip.mobile.droid.fragments;
using de.upb.hip.mobile.droid.Helpers;
using de.upb.hip.mobile.droid.Helpers.InteractiveSources;

namespace de.upb.hip.mobile.droid.Dialogs
{
    public class CaptionDialog : DialogFragment
    {
        public Action<object, EventArgs> OnCloseAction { get; set; }
        public string Subtitles { get; set; }

        private List<Fragment> Fragments { get; set; }
        private List<string> Titles { get; set; }

        private bool referencesExisting;

        public ViewPager GetTabsViewPager()
        {
            return Dialog.FindViewById<ViewPager>(Resource.Id.captionDialogViewPager);
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var parser = new InteractiveSourcesParser(Subtitles,
                                                       new ConsecutiveNumberAndConstantInteractiveSourceSubstitute
                                                           (1, GetString(Resource.String.source_substitute_counter)));

            var interactiveSourceAction = new SwitchTabAndScrollToItemInteractiveSourceAction();
            var formattedSubtitles = parser.CreateSubtitlesText(interactiveSourceAction);

            var subtitlesFragment = new CaptionDialogSubtitlesFragment { Subtitles = formattedSubtitles };
            var referencesFragment = new CaptionDialogReferencesFragment { References = parser.Sources };

            referencesExisting = parser.Sources.Any();

            Fragments = new List<Fragment>
            {
                subtitlesFragment,
                referencesFragment
            };
            Titles = new List<string>
            {
                GetString(Resource.String.audio_toolbar_cc),
                GetString(Resource.String.audio_toolbar_references)
            };
            interactiveSourceAction.GetRecyclerView = referencesFragment.GetRecyclerView;
            interactiveSourceAction.GetTabsViewPagers = GetTabsViewPager;
            interactiveSourceAction.TargetTabIndex = 1;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);

            var view = inflater.Inflate(Resource.Layout.dialog_exhibit_details_caption, container);

            var viewPager = view.FindViewById<CustomViewPager>(Resource.Id.captionDialogViewPager);
            var adapter = new CaptionDialogFragmentTabsAdapter(ChildFragmentManager, Fragments, Titles);
            viewPager.Adapter = adapter;

            var tabLayout = view.FindViewById<TabLayout>(Resource.Id.captionDialogTabLayout);
            tabLayout.SetupWithViewPager(viewPager);

            if (!referencesExisting)
            {
                DisableTabs (tabLayout);
                viewPager.Enabled = false;
            }

            var closeBtn = view.FindViewById<Button>(Resource.Id.captionDialogCloseButton);
            closeBtn.Click += (sender, args) =>
            {
                OnCloseAction(sender, args);
                Dismiss();
            };

            RetainInstance = false;

            return view;
        }

        private void DisableTabs(TabLayout tabLayout)
        {
            ViewGroup viewGroup = GetTabViewGroup(tabLayout);
            if (viewGroup != null)
                for (int childIndex = 0; childIndex < viewGroup.ChildCount; childIndex++)
                {
                    View tabView = viewGroup.GetChildAt(childIndex);
                    if (tabView != null)
                    {
                        tabView.Enabled = false;
                    }
                }
        }

        private ViewGroup GetTabViewGroup(TabLayout tabLayout)
        {
            ViewGroup viewGroup = null;

            if (tabLayout != null && tabLayout.ChildCount > 0)
            {
                View view = tabLayout.GetChildAt(0);
                var group = view as ViewGroup;
                if (group != null)
                    viewGroup = group;
            }
            return viewGroup;
        }

        public override void OnResume()
        {
            // Prevent dialogue from being too small
            var metrics = Resources.DisplayMetrics;
            var width = metrics.WidthPixels;
            var height = metrics.HeightPixels;
            Dialog.Window.SetLayout((6 * width) / 7, (4 * height) / 5);

            base.OnResume();
        }

        public override void OnPause()
        {
            base.OnPause();
            Dismiss();
        }

    }
}