// /*
//  * Copyright (C) 2016 History in Paderborn App - Universität Paderborn
//  *
//  * Licensed under the Apache License, Version 2.0 (the "License");
//  * you may not use this file except in compliance with the License.
//  * You may obtain a copy of the License at
//  *
//  *      http://www.apache.org/licenses/LICENSE-2.0
//  *
//  * Unless required by applicable law or agreed to in writing, software
//  * distributed under the License is distributed on an "AS IS" BASIS,
//  * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  * See the License for the specific language governing permissions and
//  * limitations under the License.
//  */

using System.Linq;
using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using de.upb.hip.mobile.droid.fragments;
using de.upb.hip.mobile.droid.Helpers;
using de.upb.hip.mobile.droid.Listeners;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using de.upb.hip.mobile.pcl.Common;
using HockeyApp;
using ActionBarDrawerToggle = Android.Support.V7.App.ActionBarDrawerToggle;

namespace de.upb.hip.mobile.droid.Activities {
    [Activity (Theme = "@style/AppTheme",
        Label = "HiPMobile.Droid", MainLauncher = false, Icon = "@drawable/icon")]
    public class MainActivity : AppCompatActivity {

        private readonly string LogId = "MainActivity";

        private DrawerLayout drawerLayout;
        private ExhibitSet exhibitSet;
        private GeoLocation geoLocation;
        private ExtendedLocationListener extendedLocationListener;

        private string UpdateKey = "AskUpdates";
        public bool AskForUpdates = true;

        protected override void OnCreate (Bundle savedInstanceState)
        {
            base.OnCreate (savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);

            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);

            // Check if we have the necessary permissions and request them if we don't
            // Note that the app will still fail on first launch and needs to be restarted
            SetUpPermissions ();

            extendedLocationListener = ExtendedLocationListener.GetInstance();
            extendedLocationListener.SetContext(this);
            extendedLocationListener.EnableCheckForExhibits();

            geoLocation = new GeoLocation
            {
                //Latitude = 51.71352,
                //Longitude = 8.74021

                Latitude = extendedLocationListener.GetLocation().Latitude,
                Longitude = extendedLocationListener.GetLocation().Longitude
            };

            exhibitSet = ExhibitManager.GetExhibitSets ().First ();

            //Permissions
            SetUpPermissions ();

            // Navigation Drawer
            SetUpNavigationDrawer ();

            //FeedbackManager.Register(this);
            FeedbackManager.Register (this, KeyManager.Instance.GetKey ("hockeyapp.android"), new HipFeedbackListener ());

            if (savedInstanceState == null)
            {
                // Set overview fragment
                var fragment = new ExhibitsOverviewFragment
                {
                    ExhibitSet = exhibitSet,
                    GeoLocation = geoLocation
                };

                if (FindViewById (Resource.Id.main_fragment_container) != null)
                {
                    var transaction = SupportFragmentManager.BeginTransaction ();
                    transaction.Replace (Resource.Id.main_fragment_container, fragment);
                    transaction.Commit ();
                }

            }
            else
            {
                AskForUpdates = savedInstanceState.GetBoolean (UpdateKey);
            }


            // hockeyapp code
            if (AskForUpdates)
            {
                CheckForUpdates ();
            }
        }

        protected override void OnSaveInstanceState (Bundle outState)
        {
            base.OnSaveInstanceState (outState);

            outState.PutBoolean (UpdateKey, AskForUpdates);
        }


        private void SetUpNavigationDrawer ()
        {
            drawerLayout = FindViewById<DrawerLayout> (Resource.Id.mainActivityDrawerLayout);

            // Init toolbar
            var toolbar = FindViewById<Toolbar> (Resource.Id.toolbar);
            SetSupportActionBar (toolbar);
            SupportActionBar.Title = Resources.GetString (Resource.String.app_name);

            // Attach item selected handler to navigation view
            var navigationView = FindViewById<NavigationView> (Resource.Id.nav_view);
            navigationView.NavigationItemSelected += NavigationView_NavigationItemSelected;

            // Create ActionBarDrawerToggle button and add it to the toolbar
            var drawerToggle = new ActionBarDrawerToggle (this, drawerLayout, toolbar, Resource.String.drawer_open,
                                                          Resource.String.drawer_close);
            drawerLayout.SetDrawerListener (drawerToggle);
            drawerToggle.SyncState ();
        }

        private void SetUpPermissions ()
        {
            // Check if we have the necessary permissions and request them if we don't
            // Note that the app will still fail on first launch and needs to be restarted
            if (ContextCompat.CheckSelfPermission (this,
                                                   Manifest.Permission.AccessFineLocation)
                != Permission.Granted || ContextCompat.CheckSelfPermission (this,
                                                                            Manifest.Permission.ReadExternalStorage)
                != Permission.Granted || ContextCompat.CheckSelfPermission (this,
                                                                            Manifest.Permission.WriteExternalStorage)
                != Permission.Granted)
            {
                ActivityCompat.RequestPermissions (this,
                                                   new[]
                                                   {
                                                       Manifest.Permission.AccessFineLocation, Manifest.Permission.ReadExternalStorage,
                                                       Manifest.Permission.WriteExternalStorage
                                                   },
                                                   0);
            }
        }


        //handles the action when touching the menuitems in navigationview
        private void NavigationView_NavigationItemSelected (object sender,
                                                            NavigationView.NavigationItemSelectedEventArgs e)
        {
            switch (e.MenuItem.ItemId)
            {
                case Resource.Id.nav_home:
                    // React on 'Home' selection
                    break;
                case Resource.Id.nav_route:
                    StartActivity (typeof (RouteActivity));
                    break;
                case Resource.Id.nav_licenses:
                    StartActivity (typeof (LicensingActivity));
                    break;
                case Resource.Id.nav_feedback:
                    FeedbackManager.ShowFeedbackActivity (ApplicationContext);
                    break;
                case Resource.Id.nav_preferences:
                    StartActivity (typeof (SettingsActivity));
                    break;
            }

            // Close drawer
            drawerLayout.CloseDrawers ();
        }

        protected override void OnDestroy ()
        {


            base.OnDestroy ();
            extendedLocationListener.Unregister();
            // hockeyapp code
            UnregisterManagers ();
        }

        protected override void OnStop ()
        {
            base.OnStop();
            extendedLocationListener.Unregister();
            
        }

        protected override void OnResume ()
        {
            base.OnResume ();
            var navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.Menu.GetItem (0).SetChecked (true);

            extendedLocationListener = ExtendedLocationListener.GetInstance();
            extendedLocationListener.SetContext(this);
            extendedLocationListener.EnableCheckForExhibits();

            geoLocation = new GeoLocation
            {
                //Latitude = 51.71352,
                //Longitude = 8.74021

                Latitude = extendedLocationListener.GetLocation().Latitude,
                Longitude = extendedLocationListener.GetLocation().Longitude
            };


            // hockeyapp code
            CheckForCrashes ();
        }

        protected override void OnPause ()
        {
            base.OnPause ();

            // hockeyapp code
            UnregisterManagers ();
        }

        public override void OnBackPressed ()
        {
            if (drawerLayout.IsDrawerOpen (GravityCompat.Start))
            {
                drawerLayout.CloseDrawer (GravityCompat.Start);
            }
            else
            {
                base.OnBackPressed ();
            }
        }

        /// <summary>
        ///     Methods for hockeyapp
        /// </summary>

        #region
        private void CheckForCrashes ()
        {
            var key = KeyManager.Instance.GetKey ("hockeyapp.android");
            if (!string.IsNullOrEmpty (key))
            {
                CrashManager.Register (this, key);
            }
            else
            {
                Log.Error (LogId, "HockeyApp key is zero, cannot register CrashManager.");
            }
        }

        private void CheckForUpdates ()
        {
            var key = KeyManager.Instance.GetKey ("hockeyapp.android");
            if (!string.IsNullOrEmpty (key))
            {
                // Remove this for store builds! 
                UpdateManager.Register (this, key, new HockeyAppUpdateListener (this));
            }
            else
            {
                Log.Error (LogId, "HockeyApp key is zero, cannot register UpdateManager.");
            }
        }

        internal void UnregisterManagers ()
        {
            UpdateManager.Unregister ();
        }

        #endregion

        private class HockeyAppUpdateListener : UpdateManagerListener {

            private MainActivity parent;

            public HockeyAppUpdateListener (MainActivity parent)
            {
                this.parent = parent;
            }

            public override bool CanUpdateInMarket ()
            {
                return false;
            }

            public override void OnCancel ()
            {
                base.OnCancel ();

                parent.AskForUpdates = false;
            }

        }
    }
}