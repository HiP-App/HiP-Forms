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

namespace de.upb.hip.mobile.droid.Activities {
    [Activity (Theme = "@style/AppTheme", Label = "RouteActivity", MainLauncher = false, Icon = "@drawable/icon")]
    public class RouteActivity : AppCompatActivity, IRouteSelectedListener
    {
        public static final int ACTIVITY_FILTER_RESULT = 0;
        private final HashSet<String> activeTags = new HashSet<>();
        private DBAdapter mDatabase;
        private RouteSet mRouteSet;
        private RecyclerView mRecyclerView;
        private RecyclerView.Adapter mAdapter;
        private RecyclerView.LayoutManager mLayoutManager;
        private DrawerLayout mDrawerLayout;

        protected override void OnCreate (Bundle savedInstanceState)
        {
            base.OnCreate (savedInstanceState);
            SetContentView (Resource.Layout.activity_route);


            mDatabase = new DBAdapter(this);
            mRouteSet = new RouteSet(mDatabase.getView("routes"));

            // start with every tag allowed
            for (Route route : mRouteSet.getRoutes())
            {
                for (RouteTag tag : route.getTags())
                {
                    activeTags.add(tag.getTag());
                }
            }

            // Recyler View
            mRecyclerView = (RecyclerView)findViewById(R.id.routeRecyclerView);
            mRecyclerView.setHasFixedSize(true);

            // use a linear layout manager
            mLayoutManager = new LinearLayoutManager(this);
            mRecyclerView.setLayoutManager(mLayoutManager);

            // specify an adapter
            RouteRecyclerAdapter adapter =
                    new RouteRecyclerAdapter(this.mRouteSet, getApplicationContext(), activeTags);
            mAdapter = adapter;
            mRecyclerView.setAdapter(mAdapter);
            adapter.registerRouteSelectedListener(this);

            // setUp navigation drawer
            mDrawerLayout = (DrawerLayout)findViewById(R.id.routeActivityDrawerLayout);
            super.setUpNavigationDrawer(this, mDrawerLayout);
        }

        /**
 * Switch for the filtering of the routes
 *
 * @param item Menu item
 * @return boolean Return false to allow normal menu processing to
 * proceed, true to consume it here.
 */
        @Override
    public boolean onOptionsItemSelected(MenuItem item)
        {
            switch (item.getItemId())
            {
                case R.id.action_route_filter:
                    Intent intent = new Intent(getApplicationContext(), RouteFilterActivity.class);
                intent.putExtra("RouteSet", mRouteSet);
                intent.putExtra("activeTags", activeTags);
                startActivityForResult(intent, ACTIVITY_FILTER_RESULT);
                return true;
            default:
                return public void OnRouteSelected(Route route)
        {
            throw new NotImplementedException();
        }

        super.onOptionsItemSelected(item);
        }
}

/**
 * Method for saving the selected tags
 *
 * @param requestCode integer as request code
 * @param resultCode  integer as result code
 * @param data        Intent with data
 */
@Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data)
{
    super.onActivityResult(requestCode, resultCode, data);

    switch (requestCode)
    {
        case ACTIVITY_FILTER_RESULT:
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
            super.onActivityResult(requestCode, resultCode, data);
    }

}

/**
 * Starts the RouteDetailsActivity for a specific route
 *
 * @param route Route for which the DetailsActivity should be called
 */
@Override
    public void onRouteSelected(Route route)
{
    Intent intent = new Intent(getApplicationContext(), RouteDetailsActivity.class);
        intent.putExtra("route", route);
        startActivity(intent);
    }

    /**
     * Getter for RouteSet
     *
     * @return RouteSet
     */
    public RouteSet getRouteSet()
{
    return mRouteSet;
}

public void OnRouteSelected(Route route)
        {
            throw new NotImplementedException();
        }



    }
}