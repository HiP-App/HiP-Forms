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

using System;
using System.Linq;
using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using de.upb.hip.mobile.droid.Adapters;
using de.upb.hip.mobile.droid.Helpers;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using HockeyApp;
using Osmdroid.TileProvider.TileSource;
using Osmdroid.Util;
using Osmdroid.Views;
using Realms;

namespace de.upb.hip.mobile.droid.Activities {
    [Activity (Theme = "@style/AppTheme",
        Label = "HiPMobile.Droid", MainLauncher = false, Icon = "@drawable/icon")]
    public class MainActivity : AppCompatActivity {

        // Recycler View: MainList
        private RecyclerView recyclerView;
        private RecyclerView.Adapter adapter;
        private ExhibitSet exhibitSet;
        private GeoLocation geoLocation;
        private DrawerLayout drawerLayout;

        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);

            //Delete current database to avoid migration issues, remove this when wanting persistent database usage
            Realm.DeleteRealm (new RealmConfiguration ());

            geoLocation = new GeoLocation ();
            geoLocation.Latitude = 51.71352;
            geoLocation.Longitude = 8.74021;

            DbDummyDataFiller filler = new DbDummyDataFiller (this.Assets);
            filler.InsertData ();
            this.exhibitSet = ExhibitManager.GetExhibitSets ().First ();

            //Permissions
            SetUpPermissions ();

            //Map
            SetUpMap ();

            SetUpNavigationDrawer ();

            // Recyler View
            SetUpRecycleView ();

            // hockeyapp code
            CheckForUpdates ();
        }


        private void SetUpNavigationDrawer ()
        {
            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.mainActivityDrawerLayout);

            // Init toolbar
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            // Attach item selected handler to navigation view
            var navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.NavigationItemSelected += NavigationView_NavigationItemSelected;

            // Create ActionBarDrawerToggle button and add it to the toolbar
            var drawerToggle = new Android.Support.V7.App.ActionBarDrawerToggle(this, drawerLayout, toolbar, Resource.String.drawer_open, Resource.String.drawer_close);
            drawerLayout.SetDrawerListener(drawerToggle);
            drawerToggle.SyncState();
        }

        private void SetUpPermissions ()
        {
            // Check if we have the necessary permissions and request them if we don't
            // Note that the app will still fail on first launch and needs to be restarted
            if (ContextCompat.CheckSelfPermission(this,
                                                   Manifest.Permission.AccessFineLocation)
                != Permission.Granted || ContextCompat.CheckSelfPermission(this,
                                                                            Manifest.Permission.ReadExternalStorage)
                != Permission.Granted || ContextCompat.CheckSelfPermission(this,
                                                                            Manifest.Permission.WriteExternalStorage)
                != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this,
                                                   new String[]
                                                   {Manifest.Permission.AccessFineLocation, Manifest.Permission.ReadExternalStorage, Manifest.Permission.WriteExternalStorage},
                                                   0);
            }
        }

        private void SetUpRecycleView ()
        {
            recyclerView = (RecyclerView)FindViewById(Resource.Id.mainRecyclerView);

            // use a linear layout manager
            RecyclerView.LayoutManager mLayoutManager = new LinearLayoutManager(this);
            recyclerView.SetLayoutManager(mLayoutManager);


            //RecycleAdapter
            adapter = new MainRecyclerAdapter(this.exhibitSet, geoLocation, Android.App.Application.Context);
            recyclerView.SetAdapter(adapter);

            // recyclerView.AddOnItemTouchListener(new RecyclerItemClickListener(this));
        }

        private void SetUpMap ()
        {
            var mapView = FindViewById<MapView> (Resource.Id.mapview);
            // mapView.SetTileSource(TileSourceFactory.DefaultTileSource);
            mapView.SetBuiltInZoomControls (true);

            mapView.SetTileSource (new XYTileSource ("OSM", 0, 18, 1024, ".png",
                                                     new string[] {"http://tile.openstreetmap.org/"}));


            var mapController = mapView.Controller;
            mapController.SetZoom (25);

            // var centreOfMap = new GeoPoint(51496994, -134733);
            var centreOfMap = new GeoPoint (geoLocation.Latitude, geoLocation.Longitude);


            mapController.SetCenter (centreOfMap);
        }

        //handles the action when touching the menuitems in navigationview
        void NavigationView_NavigationItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e)
        {
            switch (e.MenuItem.ItemId)
            {
                case (Resource.Id.nav_home):
                    // React on 'Home' selection
                    break;
                case (Resource.Id.nav_route):
                    // React on 'Messages' selection
                    break;
                case (Resource.Id.nav_licenses):
                    // React on 'Friends' selection
                    break;
            }

            // Close drawer
            drawerLayout.CloseDrawers();
        }

        protected override void OnDestroy ()
        {
            base.OnDestroy ();

            // hockeyapp code
            UnregisterManagers ();
        }

        protected override void OnResume ()
        {
            base.OnResume ();

            // hockeyapp code
            CheckForCrashes ();
        }

        protected override void OnPause ()
        {
            base.OnPause ();

            // hockeyapp code
            UnregisterManagers ();
        }

        /// <summary>
        /// Methods for hockeyapp
        /// </summary>
        #region
        private void CheckForCrashes ()
        {
            CrashManager.Register (this);
        }

        private void CheckForUpdates ()
        {
            // Remove this for store builds! 
            UpdateManager.Register (this);
        }

        private void UnregisterManagers ()
        {
            UpdateManager.Unregister ();
        }

        #endregion
    }
}