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
using System.Linq;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using de.upb.hip.mobile.droid.fragments.bottomsheetfragment;
using de.upb.hip.mobile.droid.fragments.exhibitpagefragment;
using de.upb.hip.mobile.droid.Helpers;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using Java.Lang;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace de.upb.hip.mobile.droid.Activities
{
    public class ExhibitDetailsActivity : AppCompatActivity
    {

        #region Fields
        /// <summary>
        /// The id of the displayed exhibit.
        /// </summary>
        private string exhibitId;

        /// <summary>
        /// The currently displayed exhibit.
        /// </summary>
        private Exhibit exhibit;

        /// <summary>
        /// The index of the currently displayed page.
        /// </summary>
        private int currentPageIndex = 0;

        /// <summary>
        /// Indicates whether the audio action in the toolbar should be shown (true) or not (false).
        /// </summary>
        private bool showAudioAction = false;

        /// <summary>
        /// Indicates whether audio is currently played (true) or not (false)
        /// </summary>
        private bool isAudioPlaying = false;

        /// <summary>
        /// Indicates whether the audio toolbar is currently displayed (true) or not (false)
        /// </summary>
        private bool isAudioToolbarHidden = true;

        /// <summary>
        /// Extras contained in the Intent that started this activity.
        /// </summary>
        private Bundle extras;

        /// <summary>
        /// Stores the current action associated with the FAB.
        /// </summary>
        private BottomSheetConfig.FabAction fabAction;

        /// <summary>
        /// Reference to the BottomSheetFragment currently displayed
        /// </summary>
        private BottomSheetFragment bottomSheetFragment = null;

        // logging
        private static readonly string Tag = "ExhibitDetailsActivity";

        // keys for saving/accessing the state
        private static readonly string INTENT_EXTRA_EXHIBIT_ID = "de.upb.hip.mobile.extra.exhibit_id";
        private static readonly string KEY_EXHIBIT_ID = "ExhibitDetailsActivity.ExhibitId";
        private static readonly string KEY_CURRENT_PAGE_INDEX = "ExhibitDetailsActivity.currentPageIndex";
        private static readonly string KEY_AUDIO_PLAYING = "ExhibitDetailsActivity.isAudioPlaying";
        private static readonly string KEY_AUDIO_TOOLBAR_HIDDEN = "ExhibitDetailsActivity.isAudioToolbarHidden";
        private static readonly string KEY_EXTRAS = "ExhibitDetailsActivity.extras";

        // ui elements
        private FloatingActionButton fab;
        private View bottomSheet;
        private BottomSheetBehavior bottomSheetBehavior;
        private LinearLayout revealView;
        private ImageButton btnPlayPause;
        private ImageButton btnPreviousPage;
        private ImageButton btnNextPage;
        #endregion

        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutString(KEY_EXHIBIT_ID, exhibit.Id);
            outState.PutInt(KEY_CURRENT_PAGE_INDEX, currentPageIndex);
            outState.PutBoolean(KEY_AUDIO_PLAYING, isAudioPlaying);
            outState.PutBoolean(KEY_AUDIO_TOOLBAR_HIDDEN, isAudioToolbarHidden);
            outState.PutBundle(KEY_EXTRAS, extras);

            base.OnSaveInstanceState(outState);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_exhibit_details);
            Toolbar toolbar = (Toolbar) FindViewById(Resource.Id.toolbar);
            toolbar.SetNavigationIcon(Resource.Drawable.ic_clear_white_24dp);
            SetSupportActionBar(toolbar);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            if (savedInstanceState != null)
            {
                // activity re-creation because of device rotation, instant run, ...
                exhibitId = savedInstanceState.GetString(KEY_EXHIBIT_ID);
                exhibit = ExhibitManager.GetExhibit(exhibitId);
                currentPageIndex = savedInstanceState.GetInt(KEY_CURRENT_PAGE_INDEX, 0);
                isAudioPlaying = savedInstanceState.GetBoolean(KEY_AUDIO_PLAYING, false);
                isAudioToolbarHidden = true;
                extras = savedInstanceState.GetBundle(KEY_EXTRAS);
            }
            else
            {
                // activity creation because of intent
                Intent intent = Intent;
                extras = intent.Extras;
                exhibitId = intent.GetStringExtra(INTENT_EXTRA_EXHIBIT_ID);
                exhibit = ExhibitManager.GetExhibit(exhibitId);
            }

            if (exhibit.Pages.Count == 0)
            {
                throw new NullPointerException("Cannot display exhibit with no pages.");
            }

            // set up bottom sheet behavior
            bottomSheet = FindViewById(Resource.Id.bottom_sheet);
            bottomSheetBehavior = BottomSheetBehavior.From(bottomSheet);
            bottomSheetBehavior.SetBottomSheetCallback(new CustomBottomSheetCallback(this));

            // audio toolbar
            revealView = (LinearLayout)FindViewById(Resource.Id.reveal_items);
            revealView.Visibility = ViewStates.Invisible;

            // display audio toolbar on savedInstanceState:
            // if (! isAudioToolbarHidden) showAudioToolbar();
            // does not work because activity creation has not been completed?!
            // see also: http://stackoverflow.com/questions/7289827/how-to-start-animation-immediately-after-oncreate

            //initialize media player
            DoBindService();
            // set up play / pause toggle
            btnPlayPause = (ImageButton)FindViewById(Resource.Id.btnPlayPause);
            btnPlayPause.Click += (sender, args) =>
            {
                if (isAudioPlaying)
                {
                    PauseAudioPlayback();
                    isAudioPlaying = false;
                }
                else
                {
                    StartAudioPlayback();
                    isAudioPlaying = true;
                    btnPlayPause.SetImageResource(Android.Resource.Color.Transparent);
                }
                UpdatePlayPauseButtonIcon();
            };

            // set up CC button
            ImageButton btnCaptions = (ImageButton)FindViewById(Resource.Id.btnCaptions);
            btnCaptions.Click += (sender, args) => { ShowCaptions(); };

            // set up previous / next button
            btnPreviousPage = (ImageButton)FindViewById(Resource.Id.buttonPrevious);
            btnPreviousPage.Click += (sender, args) => { DisplayPreviousExhibitPage(); };

            btnNextPage = (ImageButton)FindViewById(Resource.Id.buttonNext);
            btnNextPage.Click += (sender, args) => { DisplayNextExhibitPage(); };

            fab = (FloatingActionButton)FindViewById(Resource.Id.fab);
            fab.Click += (sender, args) =>
            {
                switch (fabAction)
                {
                    case BottomSheetConfig.FabAction.Next:
                        DisplayNextExhibitPage();
                        break;
                    case BottomSheetConfig.FabAction.Collapse:
                        SetFabAction(BottomSheetConfig.FabAction.Expand);
                        break;
                    case BottomSheetConfig.FabAction.Expand:
                        SetFabAction(BottomSheetConfig.FabAction.Collapse);
                        break;
                    default:
                        throw new IllegalArgumentException("Unsupported FAB action!");
                }
            };

            DisplayCurrenExhibitPage();
        }

        public void DisplayCurrenExhibitPage()
        {
            if (currentPageIndex >= exhibit.Pages.Count())
            {
                Log.Warn(Tag, "currentPageIndex >= exhibitPages.size() !");
                return;
            }

            if (!isAudioToolbarHidden)
            {
                HideAudioToolBar(); // TODO: generalize to audio playing
            }

            Page page = exhibit.Pages[currentPageIndex];

            // set previous & next button
            if (currentPageIndex == 0)
                btnPreviousPage.Visibility = ViewStates.Gone;
            else
                btnPreviousPage.Visibility = ViewStates.Visible;

            if (currentPageIndex >= exhibit.Pages.Count() - 1 || page.IsAppetizerPage())
                btnNextPage.Visibility = ViewStates.Gone;
            else
                btnNextPage.Visibility = ViewStates.Visible;

            // get ExhibitPageFragment for Page
            /*ExhibitPageFragment pageFragment =
                    ExhibitPageFragmentFactory.getFragmentForExhibitPage(page, exhibitName);*/
        }

        public void DisplayNextExhibitPage()
        {
            
        }

        public void DisplayPreviousExhibitPage()
        {
            
        }

        private void UpdateAudioFile()
        {
            
        }

        public void SetFabAction(BottomSheetConfig.FabAction action)
        {
            
        }

        private bool ShowAudioToolbar()
        {
            return false;
        }

        private bool HideAudioToolBar()
        {
            return false;
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            return base.OnPrepareOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            return base.OnOptionsItemSelected(item);
        }

        

        private void UpdatePlayPauseButtonIcon()
        {
            
        }

        private void DisplayAudioAction()
        {
            
        }

        private void ShowCaptions()
        {
            
        }

        public void DoBindService()
        {
            
        }

        protected override void OnStop()
        {
            base.OnStop();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        #region AudioControls
        private void StartAudioPlayback()
        {

        }

        private void PauseAudioPlayback()
        {

        }

        private void StopAudioPlayback()
        {

        }
        #endregion

        #region InnerClasses

        private class CustomBottomSheetCallback : BottomSheetBehavior.BottomSheetCallback
        {
            private ExhibitDetailsActivity parentActivity;

            public CustomBottomSheetCallback(ExhibitDetailsActivity parent)
            {
                this.parentActivity = parent;
            }
            public override void OnSlide(View bottomSheet, float slideOffset)
            {
                // intentionally left blank
            }

            public override void OnStateChanged(View bottomSheet, int newState)
            {
                
                // toggle between expand / collapse , inform fragment
                if (newState == BottomSheetBehavior.StateCollapsed)
                {
                    if (parentActivity.fabAction == BottomSheetConfig.FabAction.Collapse)
                        parentActivity.SetFabAction(BottomSheetConfig.FabAction.Expand);

                    if (parentActivity.bottomSheetFragment != null)
                        parentActivity.bottomSheetFragment.OnBottomSheetCollapse();

                }
                else if (newState == BottomSheetBehavior.StateExpanded)
                {
                    if (parentActivity.fabAction == BottomSheetConfig.FabAction.Expand)
                        parentActivity.SetFabAction(BottomSheetConfig.FabAction.Collapse);

                    if (parentActivity.bottomSheetFragment != null)
                        parentActivity.bottomSheetFragment.OnBottomSheetExpand();
                }
            }
        }
        #endregion
    }
}