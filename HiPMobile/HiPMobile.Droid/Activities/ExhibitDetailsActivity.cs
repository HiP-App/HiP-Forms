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

using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using de.upb.hip.mobile.droid.fragments.bottomsheetfragment;
using de.upb.hip.mobile.droid.Helpers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;

namespace de.upb.hip.mobile.droid.Activities
{
    public class ExhibitDetailsActivity : AppCompatActivity
    {
        

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
        private Bundle extra;

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
        private static readonly string KEY_EXHIBIT_NAME = "ExhibitDetailsActivity.ExhibitId";
        private static readonly string KEY_CURRENT_PAGE_INDEX = "ExhibitDetailsActivity.currentPageIndex";
        private static readonly string KEY_AUDIO_PLAYING = "ExhibitDetailsActivity.isAudioPlaying";
        private static readonly string KEY_AUDIO_TOOLBAR_HIDDEN = "ExhibitDetailsActivity.isAudioToolbarHidden";
        private static readonly string KEY_EXTRAS = "ExhibitDetailsActivity.extras";

        // ui elements
        private FloatingActionButton fab;
        private View bottomSheet;
        private BottomSheetBehavior bottomSheetBehavior;
        private LinearLayout mRevealView;
        private ImageButton btnPlayPause;
        private ImageButton btnPreviousPage;
        private ImageButton btnNextPage;

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public void DisplayCurrenExhibitPage()
        {
            
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

        public void SetFabAction()
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
    }
}