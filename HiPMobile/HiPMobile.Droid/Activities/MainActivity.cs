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
using Android.App;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Widget;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using HockeyApp;
using Osmdroid.TileProvider;
using Osmdroid.TileProvider.TileSource;
using Osmdroid.Util;
using Osmdroid.Views;
using Realms;

namespace de.upb.hip.mobile.droid.Activities {
    [Activity(Theme = "@style/AppTheme.WithActionBar",
          Label = "HiPMobile.Droid", MainLauncher = false, Icon = "@drawable/icon")]

    public class MainActivity : Activity {

        // Recycler View: MainList
        private RecyclerView mRecyclerView;
        private RecyclerView.Adapter mAdapter;

        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);


            //Delete current database to avoid migration issues, remove this when wanting persistent database usage
            Realm.DeleteRealm (new RealmConfiguration ());


            var mapView = FindViewById<MapView>(Resource.Id.mapview);
           // mapView.SetTileSource(TileSourceFactory.DefaultTileSource);
            mapView.SetBuiltInZoomControls(true);

            mapView.SetTileSource(new XYTileSource("OSM", 0, 18, 1024, ".png", 
              new string[] { "http://tile.openstreetmap.org/"})); 



            var mapController = mapView.Controller;
            mapController.SetZoom(25);

            //Geopoint of osmdroid has conlfict with geopoint from model classe
            //rename geopoint from model classes for testing
            // var centreOfMap = new GeoPoint(51496994, -134733);
            var centreOfMap = new GeoPoint(51.716819, 8.764343);
            
            mapController.SetCenter(centreOfMap);

            // Recyler View
            mRecyclerView = (RecyclerView)FindViewById(Resource.Id.mainRecyclerView);

            // use a linear layout manager
            RecyclerView.LayoutManager mLayoutManager = new LinearLayoutManager(this);
            mRecyclerView.SetLayoutManager(mLayoutManager);

            // hockeyapp code
            CheckForUpdates ();
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
        private void CheckForCrashes()
        {
            CrashManager.Register(this);
        } 

        private void CheckForUpdates()
        {
            // Remove this for store builds! 
            UpdateManager.Register(this);
        }  
 
         private void UnregisterManagers()
        {
            UpdateManager.Unregister();
        }
        #endregion
    }
}