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
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Text.Method;
using Android.Widget;
using de.upb.hip.mobile.droid.Adapters;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using System.Collections.Generic;
using Android.Support.V7.Widget;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using Android.Views;
using Android.Content;

namespace de.upb.hip.mobile.droid.Activities {
    [Activity (Theme = "@style/AppTheme", Label = "RouteActivity", MainLauncher = false, Icon = "@drawable/icon")]
    public class RouteActivity : AppCompatActivity, IRouteSelectedListener {

        public const int ActivityFilterResult = 0;
        private ISet<string> ActiveTags = new HashSet<string> ();
        private readonly ISet<Route> Routes = new HashSet<Route> ();
        private RecyclerView mRecyclerView;
        private RecyclerView.Adapter mAdapter;
        private RecyclerView.LayoutManager mLayoutManager;

        protected override void OnCreate (Bundle savedInstanceState)
        {
            base.OnCreate (savedInstanceState);
            SetContentView (Resource.Layout.activity_route);

            //Init the available routes
            foreach (Route route in RouteManager.GetRoutes ())
            {
                Routes.Add (route);
            }


            // start with every tag allowed
            foreach (Route route in Routes)
            {
                foreach (RouteTag tag in route.RouteTags)
                {
                    ActiveTags.Add (tag.Tag);
                }
            }

            // Recyler View
            mRecyclerView = (RecyclerView) FindViewById (Resource.Id.routeRecyclerView);
            mRecyclerView.HasFixedSize = true;

            // use a linear layout manager
            mLayoutManager = new LinearLayoutManager (this);
            mRecyclerView.SetLayoutManager (mLayoutManager);

            // specify an adapter
            RouteRecyclerAdapter adapter =
                new RouteRecyclerAdapter (this.Routes, ApplicationContext, ActiveTags);
            mAdapter = adapter;
            mRecyclerView.SetAdapter (mAdapter);
            adapter.RegisterRouteSelectedListener (this);
        }


        public override bool OnOptionsItemSelected (IMenuItem item)
        {
            switch (item.ItemId)
            {
                //TODO: Comment this in again when RouteFilterActivity is ported
                /*case Resource.Id.action_route_filter:
                    Intent intent = new Intent(ApplicationContext, typeof(RouteFilterActivity));
                    intent.PutExtra("RouteSet", Routes);
                    intent.PutExtra("activeTags", activeTags);
                    StartActivityForResult(intent, ActivityFilterResult);
                    return true;*/
                default:
                    return Parent.OnOptionsItemSelected (item);
            }
        }

        protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult (requestCode, resultCode, data);
            //TODO: Comment this in again when Route Filter activity is ported
            /*
    switch (requestCode)
    {
        case ActivityFilterResult:
            if (resultCode == RouteFilterActivity.RETURN_NOSAVE)
            {
                // User choosed not to save changes, don't do anything
            }
            else if (resultCode == RouteFilterActivity.RETURN_SAVE)
            {
                HashSet<String> activeTags =
                        (HashSet<String>)data.getSerializableExtra("activeTags");
                this.activeTags.clear();
                this.activeTags.addAll(activeTags);
                mAdapter.notifyDataSetChanged();
            }
            break;
        default:
            Parent.OnActivityResult(requestCode, resultCode, data);
    }
    */
        }


        public void OnRouteSelected (Route route)
        {
            Intent intent = new Intent (ApplicationContext, typeof (RouteDetailsActivity));
            intent.PutExtra (RouteDetailsActivity.KEY_ROUTE_ID, route.Id);
            StartActivity (intent);
        }

    }
}