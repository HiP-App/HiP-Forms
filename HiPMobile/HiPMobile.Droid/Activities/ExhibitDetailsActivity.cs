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
using Android.Animation;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Preferences;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Text.Method;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using de.upb.hip.mobile.droid.Dialogs;
using de.upb.hip.mobile.droid.fragments;
using de.upb.hip.mobile.droid.fragments.bottomsheetfragment;
using de.upb.hip.mobile.droid.fragments.exhibitpagefragment;
using de.upb.hip.mobile.droid.Helpers;
using de.upb.hip.mobile.droid.Helpers.InteractiveSources;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using Java.Lang;
using Exception = Java.Lang.Exception;
using Math = System.Math;
using Object = Java.Lang.Object;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace de.upb.hip.mobile.droid.Activities {
    [Activity (Theme = "@style/AppTheme.NoActionBar",
        Label = "HiPMobile.Droid", MainLauncher = false, Icon = "@drawable/icon")]
    public class ExhibitDetailsActivity : AppCompatActivity {

        public ExhibitDetailsActivity ()
        {
            
        }

        protected override void OnSaveInstanceState (Bundle outState)
        {
            outState.PutString (KEY_EXHIBIT_ID, exhibit.Id);
            outState.PutInt (KEY_CURRENT_PAGE_INDEX, currentPageIndex);
            outState.PutBoolean (KEY_AUDIO_PLAYING, isAudioPlaying);
            outState.PutBoolean (KEY_AUDIO_TOOLBAR_HIDDEN, isAudioToolbarHidden);
            outState.PutBoolean (KEY_CAPTION_SHOWN, isCaptionShown);
            outState.PutBundle (KEY_EXTRAS, extras);
            if (captionDialog != null)
            {
                outState.PutInt (KEY_CURRENT_CAPTION_TAB, captionDialog.CurrentTab);
                outState.PutInt (KEY_CURRENT_SOURCE, captionDialog.CurrentSource);
            }
            
            base.OnSaveInstanceState (outState);
        }

        protected override void OnCreate (Bundle savedInstanceState)
        {
            base.OnCreate (savedInstanceState);

            //initialize media player
            mediaPlayerConnection = new CustomServiceConnection(this);
            DoBindService();

            SetContentView (Resource.Layout.activity_exhibit_details);
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);

            var toolbar = (Toolbar) FindViewById (Resource.Id.toolbar);
            toolbar.SetNavigationIcon (Resource.Drawable.ic_clear_white_24dp);
            SetSupportActionBar (toolbar);
            audioSeekbar = FindViewById<SeekBar> (Resource.Id.audio_progress_bar);
            audioSeekbar.ProgressChanged += (sender, args) => {
                if (mediaPlayerService != null && args.FromUser)
                {
                    mediaPlayerService.SeekTo (args.Progress);
                }
            };

            SupportActionBar.SetDisplayHomeAsUpEnabled (true);

            sharedPreferences = PreferenceManager.GetDefaultSharedPreferences (this);

            if (savedInstanceState != null)
            {
                // activity re-creation because of device rotation, instant run, ...
                exhibitId = savedInstanceState.GetString (KEY_EXHIBIT_ID);
                exhibit = ExhibitManager.GetExhibit (exhibitId);
                currentPageIndex = savedInstanceState.GetInt (KEY_CURRENT_PAGE_INDEX, 0);
                isAudioPlaying = savedInstanceState.GetBoolean (KEY_AUDIO_PLAYING, false);
                if (!isAudioPlaying)
                {
                    pauseAudioPlaybackFlag = true;
                }
                isAudioToolbarHidden = savedInstanceState.GetBoolean(KEY_AUDIO_TOOLBAR_HIDDEN, true);
                if (!isAudioToolbarHidden)
                {
                    showAudioToolbarFlag = true;
                }
                extras = savedInstanceState.GetBundle (KEY_EXTRAS);
                isCaptionShown = savedInstanceState.GetBoolean (KEY_CAPTION_SHOWN);

                int selectedTab = savedInstanceState.GetInt(KEY_CURRENT_CAPTION_TAB);
                int currentSource = savedInstanceState.GetInt (KEY_CURRENT_SOURCE);
                
                if (isCaptionShown)
                {
                    ShowCaptions (selectedTab, currentSource);
                }
            }
            else
            {
                // activity creation because of intent
                var intent = Intent;
                extras = intent.Extras;
                exhibitId = intent.GetStringExtra (INTENT_EXTRA_EXHIBIT_ID);
                exhibit = ExhibitManager.GetExhibit (exhibitId);
            }
            Title = exhibit.Name;

            if (exhibit.Pages.Count == 0)
            {
                throw new NullPointerException ("Cannot display exhibit with no pages.");
            }

            // set up bottom sheet behavior
            bottomSheet = FindViewById (Resource.Id.bottom_sheet);
            bottomSheetBehavior = BottomSheetBehavior.From (bottomSheet);
            bottomSheetBehavior.SetBottomSheetCallback (new CustomBottomSheetCallback (this));

            // audio toolbar
            revealView = (LinearLayout) FindViewById (Resource.Id.reveal_items);
            revealView.Visibility = ViewStates.Invisible;

            // Does not work with animations:
            // see also: http://stackoverflow.com/questions/7289827/how-to-start-animation-immediately-after-oncreate

            // set up play / pause toggle
            btnPlayPause = (ImageButton) FindViewById (Resource.Id.btnPlayPause);
            btnPlayPause.Click += (sender, args) => {
                if (isAudioPlaying)
                {
                    PauseAudioPlayback ();
                    isAudioPlaying = false;
                }
                else
                {
                    StartAudioPlayback ();
                    isAudioPlaying = true;
                    btnPlayPause.SetImageResource (Android.Resource.Color.Transparent);
                }
                UpdatePlayPauseButtonIcon ();
            };

            // set up CC button
            var btnCaptions = (ImageButton) FindViewById (Resource.Id.btnCaptions);
            btnCaptions.Click += (sender, args) => { ShowCaptions (); };

            // set up previous / next button
            btnPreviousPage = (ImageButton) FindViewById (Resource.Id.buttonPrevious);
            btnPreviousPage.Click += (sender, args) => { DisplayPreviousExhibitPage (); };

            btnNextPage = (ImageButton) FindViewById (Resource.Id.buttonNext);
            btnNextPage.Click += (sender, args) => { DisplayNextExhibitPage (); };

            fab = (FloatingActionButton) FindViewById (Resource.Id.fab);
            fab.Click += (sender, args) => {
                switch (fabAction)
                {
                    case BottomSheetConfig.FabAction.Next:
                        DisplayNextExhibitPage ();
                        break;
                    case BottomSheetConfig.FabAction.Collapse:
                        SetFabAction (BottomSheetConfig.FabAction.Expand);
                        break;
                    case BottomSheetConfig.FabAction.Expand:
                        SetFabAction (BottomSheetConfig.FabAction.Collapse);
                        break;
                    default:
                        throw new IllegalArgumentException ("Unsupported FAB action!");
                }
            };
        }

        //Callback for when the MediaService is bound
        //This is required as all actions pertaining audio can only be performed
        //after the MediaService is bound.
        public void OnAudioServiceConnected ()
        {
            DisplayCurrenExhibitPage();
        }

        public void DisplayCurrenExhibitPage ()
        {
            if (currentPageIndex >= exhibit.Pages.Count ())
            {
                Log.Warn (Tag, "currentPageIndex >= exhibitPages.size() !");
                return;
            }
            
            if (!isAudioToolbarHidden)
            {
                if (showAudioToolbarFlag)
                {
                    ShowAudioToolbar ();
                    showAudioToolbarFlag = false;
                }
                else
                {
                    HideAudioToolBar(); // TODO: generalize to audio playing
                }
            }
            
            var page = exhibit.Pages [currentPageIndex];

            // set previous & next button
            if (currentPageIndex == 0)
                btnPreviousPage.Visibility = ViewStates.Gone;
            else
                btnPreviousPage.Visibility = ViewStates.Visible;

            if (currentPageIndex >= exhibit.Pages.Count () - 1 || page.IsAppetizerPage ())
                btnNextPage.Visibility = ViewStates.Gone;
            else
                btnNextPage.Visibility = ViewStates.Visible;

            // get ExhibitPageFragment for Page
            var pageFragment =
                ExhibitPageFragmentFactory.GetFragmentForExhibitPage (page, exhibit.Name);

            if (pageFragment == null)
            {
                Log.Error (Tag, "pageFragment is null!");
                return;
            }

            pageFragment.Arguments = extras;

            // TODO: this seems to take some time. would it help to do this in a separate thread?
            // remove old fragment and display new fragment
            if (FindViewById (Resource.Id.content_fragment_container) != null)
            {
                var transaction = SupportFragmentManager.BeginTransaction ();
                transaction.Replace (Resource.Id.content_fragment_container, pageFragment);
                transaction.Commit ();
            }

            // configure bottom sheet
            var config = pageFragment.GetBottomSheetConfig ();

            if (config == null)
            {
                Log.Error (Tag, "BottomSheetConfig cannot be null!");
                return;
            }

            if (config.DisplayBottomSheet)
            {
                bottomSheet.Visibility = ViewStates.Visible;

                // configure peek height and max height
                var peekHeightInPixels = (int) PixelDpConversion.ConvertDpToPixel (config.PeekHeight);
                bottomSheetBehavior.PeekHeight = peekHeightInPixels;

                var maxHeightInPixels = (int) PixelDpConversion.ConvertDpToPixel (config.MaxHeight);
                var parameters = bottomSheet.LayoutParameters;
                parameters.Height = maxHeightInPixels;
                bottomSheet.LayoutParameters = parameters;

                // set content
                bottomSheetFragment = config.BottomSheetFragment;

                if (bottomSheetFragment == null)
                {
                    Log.Error (Tag, "bottomSheetFragment is null!");
                }
                else
                {
                    // FIXME: adding the new fragment somehow fails if the BottomSheet is expanded
                    // TODO: this seems to take some time. would it help to do this in a separate thread?
                    // remove old fragment and display new fragment
                    if (FindViewById (Resource.Id.bottom_sheet_fragment_container) != null)
                    {
                        var transaction = SupportFragmentManager.BeginTransaction ();
                        transaction.Replace (Resource.Id.bottom_sheet_fragment_container, bottomSheetFragment);
                        transaction.Commit ();
                    }
                }

                // configure FAB (includes expanded/collapsed state)
                SetFabAction (config.fabAction);
            }
            else
            {
                // config.displayBottomSheet == false
                bottomSheet.Visibility = ViewStates.Gone;
            }

            // display audio action only if page provides audio
            if (page.Audio == null)
            {
                DisplayAudioAction (false);
                HideAudioToolBar();
            }
            else
            {
                DisplayAudioAction (true);
                // check is preference to automatically start audio is on
                if (sharedPreferences.GetBoolean (Resources.GetString (Resource.String.pref_auto_start_audio_key), false))
                {
                    // We might have resumed, but were already playing
                    // before the suspension, so just go on
                    if (!pauseAudioPlaybackFlag)
                    {
                        ShowAudioToolbar();
                        StartAudioPlayback();
                        isAudioPlaying = true;
                        UpdatePlayPauseButtonIcon();
                    }
                    else
                    {
                        // We resumed and were paused before the suspension
                        // Just update the buttons and progress bar
                        pauseAudioPlaybackFlag = false;
                        UpdatePlayPauseButtonIcon ();
                        audioSeekbar.Max = (int)mediaPlayerService.GetTimeTotal();
                        startTime = mediaPlayerService.GetTimeCurrent();
                        audioSeekbar.Progress = (int)startTime;
                    }
                }
                else if (!pauseAudioPlaybackFlag && mediaPlayerService.GetTimeCurrent () > 0)
                {
                    StartAudioPlayback();
                    isAudioPlaying = true;
                    UpdatePlayPauseButtonIcon();
                }
                else if (mediaPlayerService.GetTimeCurrent () > 0)
                {
                    audioSeekbar.Max = (int)mediaPlayerService.GetTimeTotal();
                    startTime = mediaPlayerService.GetTimeCurrent();
                    audioSeekbar.Progress = (int)startTime;
                }
            }
        }

        /// <summary>
        ///     Displays the next exhibit page.
        /// </summary>
        public void DisplayNextExhibitPage ()
        {
            currentPageIndex++;

            if (currentPageIndex >= exhibit.Pages.Count ())
            {
                Toast.MakeText (ApplicationContext,
                                Resource.String.currently_no_further_info, ToastLength.Long).Show ();
                currentPageIndex--;
                Log.Warn (Tag, "currentPageIndex >= exhibitPages.size()");
            }
            else
            {
                bottomSheetBehavior.State = BottomSheetBehavior.StateCollapsed;
                UpdateAudioFile ();

                DisplayCurrenExhibitPage ();
            }
        }

        /// <summary>
        ///     Displays the previous exhibit page (for currentPageIndex > 0).
        /// </summary>
        public void DisplayPreviousExhibitPage ()
        {
            currentPageIndex--;
            if (currentPageIndex < 0)
            {
                Log.Warn (Tag, "currentPageIndex < 0");
                currentPageIndex++;
                return;
            }
            UpdateAudioFile ();
            bottomSheetBehavior.State = BottomSheetBehavior.StateCollapsed;
            DisplayCurrenExhibitPage ();
        }

        /// <summary>
        ///     Everytime the page is changed, the audio file needs to be updated to the new page.
        /// </summary>
        private void UpdateAudioFile ()
        {
            StopAudioPlayback ();
            mediaPlayerService.SetAudioFile (exhibit.Pages [currentPageIndex].Audio);
            UpdatePlayPauseButtonIcon ();
        }

        /// <summary>
        ///     Sets the action of the FAB. Adjusts the appearance (visibility and icon) of the FAB
        ///     and the state of the bottom sheet accordingly.
        /// </summary>
        /// <param name="action">FAB action to set.</param>
        public void SetFabAction (BottomSheetConfig.FabAction action)
        {
            fabAction = action;
            fab.Visibility = ViewStates.Visible;

            switch (action)
            {
                case BottomSheetConfig.FabAction.None:
                    fab.Visibility = ViewStates.Gone;
                    break;
                case BottomSheetConfig.FabAction.Next:
                    fab.SetImageResource (Resource.Drawable.ic_arrow_forward_48dp);
                    break;
                case BottomSheetConfig.FabAction.Collapse:
                    bottomSheetBehavior.State = BottomSheetBehavior.StateExpanded;
                    fab.SetImageResource (Resource.Drawable.ic_expand_more_white_48dp);
                    break;
                case BottomSheetConfig.FabAction.Expand:
                    bottomSheetBehavior.State = BottomSheetBehavior.StateCollapsed;
                    fab.SetImageResource (Resource.Drawable.ic_expand_less_white_48dp);
                    break;
                default:
                    throw new IllegalArgumentException ("Unsupported FAB action!");
            }
        }

        /// <summary>
        ///     Shows the audio toolbar.
        /// </summary>
        /// <returns>true if the toolbar has been revealed, false otherwise.</returns>
        private bool ShowAudioToolbar ()
        {
            revealView.Visibility = ViewStates.Visible;
            isAudioToolbarHidden = false;
            return true;
            // check only if mRevealView != null. If isAudioToolbarHidden == true is also checked,
            // the toolbar cannot be displayed on savedInstanceState
            if (revealView != null)
            {
                var cx = revealView.Left + revealView.Right;
                var cy = revealView.Top;
                var radius = Math.Max (revealView.Width, revealView.Height);

                if (Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
                {
                    var animator =
                        ViewAnimationUtils.CreateCircularReveal (revealView, cx, cy, 0, radius);
                    animator.SetInterpolator (new AccelerateDecelerateInterpolator ());
                    animator.SetDuration (800);

                    revealView.Visibility = ViewStates.Invisible;
                    animator.Start ();
                }
                var anim = ViewAnimationUtils
                    .CreateCircularReveal (revealView, cx, cy, 0, radius);
                revealView.Visibility = ViewStates.Visible;
                anim.Start ();

                isAudioToolbarHidden = false;
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Hides the audio toolbar.
        /// </summary>
        /// <returns>If the audio toolbar was hidden, false otherwise</returns>
        private bool HideAudioToolBar ()
        {
            revealView.Visibility = ViewStates.Invisible;
            isAudioToolbarHidden = true;
            return true;
        }

        public override bool OnCreateOptionsMenu (IMenu menu)
        {
            // Inflate the menu; this adds items to the action bar if it is present.
            MenuInflater.Inflate (Resource.Menu.activity_exhibit_details_menu_main, menu);
            return true;
        }

        public override bool OnPrepareOptionsMenu (IMenu menu)
        {
            menu.FindItem (Resource.Id.action_audio).SetVisible (showAudioAction);
            return base.OnPrepareOptionsMenu (menu);
        }

        public override bool OnOptionsItemSelected (IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_audio:
                    if (isAudioToolbarHidden)
                        ShowAudioToolbar ();
                    else
                        HideAudioToolBar ();
                    return true;

                case Android.Resource.Id.Home:
                    SupportFinishAfterTransition ();
                    return true;
            }

            return base.OnOptionsItemSelected (item);
        }


        private void UpdatePlayPauseButtonIcon ()
        {
            // remove old image first
            btnPlayPause.SetImageResource (Android.Resource.Color.Transparent);

            if (isAudioPlaying)
                btnPlayPause.SetImageResource (Resource.Drawable.ic_pause_black_36dp);
            else
                btnPlayPause.SetImageResource (Resource.Drawable.ic_play_arrow_black_36dp);
        }

        private void DisplayAudioAction (bool visible)
        {
            showAudioAction = visible;
            InvalidateOptionsMenu ();
        }

        private void ShowCaptions (int tabToSelect = 0, int currentSource = 0)
        {
            isCaptionShown = true;
            var subtitles = exhibit.Pages [currentPageIndex].Audio.Caption;

            Action<object, EventArgs> onCloseAction = (sender, args) => {
                isCaptionShown = false;
                if (isAudioPlayingFinished)
                    SwitchToNextPageBasedOnSetting ();
            };

            captionDialog = new CaptionDialog {
                OnCloseAction = onCloseAction,
                Subtitles = subtitles,
                CurrentTab = tabToSelect,
                CurrentSource = currentSource
            };

            captionDialog.Show (SupportFragmentManager, "CaptionDialog");
        }

        /// <summary>
        ///     Initializes the service and binds it.
        /// </summary>
        public void DoBindService ()
        {
            var intent = new Intent (this, typeof (MediaPlayerService));
            StartService(new Intent(this, typeof(MediaPlayerService)));
            BindService (intent, mediaPlayerConnection, 0);
        }

        protected override void OnDestroy ()
        {
            base.OnDestroy ();
            if (IsFinishing)
            {
                //Only stop sound when activity is getting killed, not when rotated
                if (isBound)
                {
                    mediaPlayerService.StopSound();
                    StopService(new Intent(this, typeof(MediaPlayerService)));
                }
            }
            if (isBound)
            {
                mediaPlayerService.RemoveOnCompleteListener(ReactToAudioCompletion);
                UnbindService(mediaPlayerConnection);
                isBound = false;
            }
            
        }

        #region Fields

        /// <summary>
        ///     Preferences of the app.
        /// </summary>
        private ISharedPreferences sharedPreferences;

        /// <summary>
        ///     The id of the displayed exhibit.
        /// </summary>
        private string exhibitId;

        /// <summary>
        ///     The currently displayed exhibit.
        /// </summary>
        private Exhibit exhibit;

        /// <summary>
        ///     The index of the currently displayed page.
        /// </summary>
        private int currentPageIndex;

        /// <summary>
        ///     Indicates whether the audio action in the toolbar should be shown (true) or not (false).
        /// </summary>
        private bool showAudioAction;

        /// <summary>
        ///     Indicates whether audio is currently played (true) or not (false)
        /// </summary>
        private bool isAudioPlaying;

        /// <summary>
        ///     Indicates whether audio playing is finished
        /// </summary>
        private bool isAudioPlayingFinished;

        /// <summary>
        ///     Indicates whether the audio toolbar is currently displayed (true) or not (false)
        /// </summary>
        private bool isAudioToolbarHidden = true;

        /// <summary>
        ///     Extras contained in the Intent that started this activity.
        /// </summary>
        private Bundle extras;

        private MediaPlayerService mediaPlayerService;
        private bool isBound;


        /// <summary>
        ///     The progressbar in the audio menu.
        /// </summary>
        private SeekBar audioSeekbar;

        /// <summary>
        ///     Used for audio playing.
        /// </summary>
        private double startTime;

        /// <summary>
        ///     Flag for pausing the audio playback on activity start
        ///     This is required when the device is rotated while audio is paused
        ///     To store the pause state temporarily.
        /// </summary>
        private bool pauseAudioPlaybackFlag = false;

        /// <summary>
        ///     Flag for showing the audio toolbar on activity start
        ///     This is required when the device is rotated while audio is paused
        ///     To store the toolbar state temporarily.
        /// </summary>
        private bool showAudioToolbarFlag = false;

        /// <summary>
        ///     Used to indicate if the captions are currently displyed
        /// </summary>
        private bool isCaptionShown = false;

        /// <summary>
        ///     Handler is needed for UI updates (especially media player - audio progress bar)
        /// </summary>
        private readonly Handler handler = new Handler ();

        //Subclass for media player binding
        private IServiceConnection mediaPlayerConnection;

        /// <summary>
        ///     Stores the current action associated with the FAB.
        /// </summary>
        private BottomSheetConfig.FabAction fabAction;

        /// <summary>
        ///     Reference to the BottomSheetFragment currently displayed
        /// </summary>
        private BottomSheetFragment bottomSheetFragment;

        // logging
        private static readonly string Tag = "ExhibitDetailsActivity";

        // keys for saving/accessing the state
        public static readonly string INTENT_EXTRA_EXHIBIT_ID = "de.upb.hip.mobile.extra.exhibit_id";
        private static readonly string KEY_EXHIBIT_ID = "ExhibitDetailsActivity.ExhibitId";
        private static readonly string KEY_CURRENT_PAGE_INDEX = "ExhibitDetailsActivity.currentPageIndex";
        private static readonly string KEY_AUDIO_PLAYING = "ExhibitDetailsActivity.isAudioPlaying";
        private static readonly string KEY_CAPTION_SHOWN = "ExhibitDetailsActivity.isCaptionShown";
        private static readonly string KEY_AUDIO_TOOLBAR_HIDDEN = "ExhibitDetailsActivity.isAudioToolbarHidden";
        private static readonly string KEY_EXTRAS = "ExhibitDetailsActivity.extras";
        private static readonly string KEY_CURRENT_CAPTION_TAB = "ExhibitDetailsActivity.captionDialog.SelectedTab";
        private static readonly string KEY_CURRENT_SOURCE = "ExhibitDetailsActivity.captionDialog.CurrentSource";

        // ui elements
        private FloatingActionButton fab;
        private View bottomSheet;
        private BottomSheetBehavior bottomSheetBehavior;
        private LinearLayout revealView;
        private ImageButton btnPlayPause;
        private ImageButton btnPreviousPage;
        private ImageButton btnNextPage;

        private CaptionDialog captionDialog;

        #endregion

        #region AudioControls

        /// <summary>
        ///     Starts the playback of the audio associated with the page.
        /// </summary>
        private void StartAudioPlayback ()
        {
            try
            {
                if (mediaPlayerService == null)
                {
                    Log.Error ("ExhibitDetailsActivity", "MediaPlayer is null in StartAudio()");
                    return;
                }
                if (!mediaPlayerService.AudioFileIsSet)
                {
                    mediaPlayerService.SetAudioFile (exhibit.Pages [currentPageIndex].Audio);
                }
                mediaPlayerService.AddOnCompleteListener (ReactToAudioCompletion);
                mediaPlayerService.StartSound ();
                audioSeekbar.Max = (int) mediaPlayerService.GetTimeTotal ();
                handler.PostDelayed (UpdateProgressbar, 100);
            }
            catch (IllegalStateException e)
            {
                isAudioPlaying = false;
            }
            catch (NullPointerException e)
            {
                isAudioPlaying = false;
            }
            catch (Exception e)
            {
                isAudioPlaying = false;
            }
        }

        /// <summary>
        /// EventHandler that is executed when audio playback finishes.
        /// </summary>
        public void ReactToAudioCompletion(object sender, EventArgs args)
        {
            isAudioPlaying = false;
            isAudioPlayingFinished = true;
            UpdatePlayPauseButtonIcon();
            if(!isCaptionShown)
                SwitchToNextPageBasedOnSetting();
        }

        private void SwitchToNextPageBasedOnSetting ()
        {
            if (sharedPreferences.GetBoolean(Resources.GetString(Resource.String.pref_auto_page_switch_key), false))
                if (exhibit.Pages[currentPageIndex].Audio != null)
                    DisplayNextExhibitPage();
        }

        private void UpdateProgressbar ()
        {
            startTime = mediaPlayerService.GetTimeCurrent ();
            audioSeekbar.Progress = (int) startTime;
            handler.PostDelayed (UpdateProgressbar, 100);
        }

        /// <summary>
        ///     Pauses the playback of the audio.
        /// </summary>
        private void PauseAudioPlayback ()
        {
            try
            {
                mediaPlayerService.PauseSound ();
            }
            catch (IllegalStateException e)
            {
            }
            catch (NullPointerException e)
            {
            }
            catch (Exception e)
            {
            }
            isAudioPlaying = false;
        }

        /// <summary>
        ///     Stops the playback of audio. this is needed, when changing the page and therefore the
        ///     audio file
        /// </summary>
        private void StopAudioPlayback ()
        {
            try
            {
                mediaPlayerService.StopSound ();
            }
            catch (IllegalStateException e)
            {
            }
            catch (NullPointerException e)
            {
            }
            catch (System.Exception e)
            {
            }
            isAudioPlaying = false;
            isAudioPlayingFinished = false;
        }

        #endregion

        #region InnerClasses

        private class CustomBottomSheetCallback : BottomSheetBehavior.BottomSheetCallback {

            private readonly ExhibitDetailsActivity parentActivity;

            public CustomBottomSheetCallback (ExhibitDetailsActivity parent)
            {
                parentActivity = parent;
            }

            public override void OnSlide (View bottomSheet, float slideOffset)
            {
                // intentionally left blank
            }

            public override void OnStateChanged (View bottomSheet, int newState)
            {
                // toggle between expand / collapse , inform fragment
                if (newState == BottomSheetBehavior.StateCollapsed)
                {
                    if (parentActivity.fabAction == BottomSheetConfig.FabAction.Collapse)
                        parentActivity.SetFabAction (BottomSheetConfig.FabAction.Expand);

                    if (parentActivity.bottomSheetFragment != null)
                        parentActivity.bottomSheetFragment.OnBottomSheetCollapse ();
                }
                else if (newState == BottomSheetBehavior.StateExpanded)
                {
                    if (parentActivity.fabAction == BottomSheetConfig.FabAction.Expand)
                        parentActivity.SetFabAction (BottomSheetConfig.FabAction.Collapse);

                    if (parentActivity.bottomSheetFragment != null)
                        parentActivity.bottomSheetFragment.OnBottomSheetExpand ();
                }
            }

        }


        private class CustomServiceConnection : Object, IServiceConnection {

            private readonly ExhibitDetailsActivity parent;

            public CustomServiceConnection (ExhibitDetailsActivity parent)
            {
                this.parent = parent;
            }

            public void OnServiceConnected (ComponentName name, IBinder service)
            {
                var binder =
                    (MediaPlayerService.MediaPlayerBinder) service;
                parent.mediaPlayerService = binder.GetService ();
                if (parent.mediaPlayerService == null)
                {
                    Log.Error ("ExhibitDetailsActivity", "Could not create a media player serivce");
                    //this case should not happen. add error handling
                }
                else
                {
                    Log.Info("ExhibitDetailsActivity", "Bound MediePlayer service");
                    parent.isBound = true;
                    parent.OnAudioServiceConnected ();
                }
            }

            public void OnServiceDisconnected (ComponentName name)
            {
                parent.isBound = false;
                Log.Info ("ExhibitDetailsActivity", "Unbound MediePlayer service");
            }



        }

        #endregion
    }
}