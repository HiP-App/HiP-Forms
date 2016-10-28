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

        private ViewPager viewPager;

        private int currentTab;
        public int CurrentTab
        {
            get
            {
                currentTab = viewPager.CurrentItem;
                return currentTab;
            }
            set { currentTab = value; }
        }

        private int currentSource;

        public int CurrentSource {
            get
            {
                currentSource = referencesFragment.CurrentSource;
                return currentSource;
            }
            set { currentSource = value; }
        }

        private CaptionDialogSubtitlesFragment subtitlesFragment;
        private CaptionDialogReferencesFragment referencesFragment;

        private bool referencesExisting;

        public ViewPager GetTabsViewPager()
        {
            return viewPager;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var parser = new InteractiveSourcesParser(Subtitles,
                                                       new ConsecutiveNumberAndConstantInteractiveSourceSubstitute
                                                           (1, GetString(Resource.String.source_substitute_counter)));

            
            referencesFragment = new CaptionDialogReferencesFragment(GetString(Resource.String.audio_toolbar_references)) { References = parser.Sources };
            referencesExisting = parser.Sources.Any();

            var interactiveSourceAction = new SwitchTabAndScrollToItemInteractiveSourceAction
            {
                GetRecyclerView = referencesFragment.GetRecyclerView,
                GetTabsViewPagers = GetTabsViewPager,
                TargetTabIndex = 1
            };
            var formattedSubtitles = parser.CreateSubtitlesText(interactiveSourceAction);

            subtitlesFragment = new CaptionDialogSubtitlesFragment(GetString(Resource.String.audio_toolbar_cc)) { Subtitles = formattedSubtitles };
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);

            var view = inflater.Inflate(Resource.Layout.dialog_exhibit_details_caption, container);

            viewPager = view.FindViewById<CustomViewPager>(Resource.Id.captionDialogViewPager);

            var fragments = new List<CaptionDialogFragment>
            {
                subtitlesFragment,
                referencesFragment
            };

            var adapter = new CaptionDialogFragmentTabsAdapter(ChildFragmentManager, fragments);
            viewPager.Adapter = adapter;

            var tabLayout = view.FindViewById<TabLayout>(Resource.Id.captionDialogTabLayout);
            tabLayout.SetupWithViewPager(viewPager);

            viewPager.SetCurrentItem(currentTab, false);
            referencesFragment.CurrentSource = currentSource;

            if (!referencesExisting)
            {
                DisableTabs(tabLayout);
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