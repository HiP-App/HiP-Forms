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

using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using de.upb.hip.mobile.droid.Helpers;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using Java.Lang;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace de.upb.hip.mobile.droid.Activities {
    [Activity (Theme = "@style/AppTheme", Label = "Routen Filtern", MainLauncher = false, Icon = "@drawable/icon")]
    public class RouteFilterActivity : AppCompatActivity {

        public static readonly string IntentActiveTags = "activeTags";

        protected override void OnCreate (Bundle savedInstanceState)
        {
            base.OnCreate (savedInstanceState);
            SetContentView (Resource.Layout.activity_route_filter);
            var intent = Intent;

            var toolbar = (Toolbar) FindViewById (Resource.Id.toolbar);
            SetSupportActionBar (toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled (true);


            ISet<Route> routes = new HashSet<Route> ();
            //Init the available routes
            foreach (var route in RouteManager.GetRoutes ())
            {
                routes.Add (route);
            }

            ISet<string> activeTags = new HashSet<string> ();
            var tags = intent.GetStringArrayExtra (IntentActiveTags);
            foreach (var tag in tags)
            {
                activeTags.Add (tag);
            }

            //There will be duplicates in the route set so we have to remove them
            var uniqueTags = new Dictionary<string, RouteTagHolder> ();
            foreach (var route in routes)
            {
                foreach (var tag in route.RouteTags)
                {
                    if (!uniqueTags.ContainsKey (tag.Tag))
                    {
                        uniqueTags.Add (tag.Tag,
                                        new RouteTagHolder (activeTags.Contains (tag.Tag), tag));
                    }
                }
            }

            // Add tags
            var listView = (ListView) FindViewById (Resource.Id.routeFilterTagList);
            ArrayAdapter<RouteTagHolder> adapter =
                new RouteTagArrayAdapter (ApplicationContext,
                                          new List<RouteTagHolder> (uniqueTags.Values));
            listView.Adapter = adapter;

            // Add buttons
            var closeWithoutSave = (Button) FindViewById (Resource.Id.routeFilterCloseWithoutSaveButton);
            var closeWithSave = (Button) FindViewById (Resource.Id.routeFilterCloseWithSaveButton);

            closeWithoutSave.SetOnClickListener (new CloseWithoutSaveOnClickListener (this));

            closeWithSave.SetOnClickListener (new CloseWithSaveOnClickListener (this, adapter));
        }

        public override bool OnSupportNavigateUp ()
        {
            Finish ();
            return true;
        }

        public void FinishWithResult (Result result)
        {
            SetResult (result);
            Finish ();
        }

        public void FinishWithResultAndIntent (Result result, Intent intent)
        {
            SetResult (result, intent);
            Finish ();
        }


        /// <summary>
        ///    Helper class for one RouteTagView
        /// </summary>
        private class RouteTagViewHolder : Object {

            private readonly CheckBox mCheckBox;
            private readonly ImageView mImageView;

            /// <summary>
            ///    Constructor for RouteTagViewHolder
            ///     @param checkBox  The checkbox for this tag
            ///     @param imageView The image for this tag
            /// </summary>
            public RouteTagViewHolder (CheckBox checkBox, ImageView imageView)
            {
                mCheckBox = checkBox;
                mImageView = imageView;
            }

            public CheckBox getCheckBox ()
            {
                return mCheckBox;
            }

            public ImageView getImageView ()
            {
                return mImageView;
            }

        }


        /// <summary>
        ///    Helper class for a route tag
        /// </summary>
        private class RouteTagHolder : Object {

            private bool mIsSelected;
            private readonly RouteTag mRouteTag;

            /// <summary>
            ///    Constructor for a RouteTagHolder
            ///     @param isSelected if the tag is currently selected
            ///     @param routeTag   the tag
            /// </summary>
            public RouteTagHolder (bool isSelected, RouteTag routeTag)
            {
                mIsSelected = isSelected;
                mRouteTag = routeTag;
            }

            public bool isSelected ()
            {
                return mIsSelected;
            }

            public void setSelected (bool isSelected)
            {
                mIsSelected = isSelected;
            }

            public RouteTag getRouteTag ()
            {
                return mRouteTag;
            }

        }

        /// <summary>
        ///    Helper class for a route tag array
        /// </summary>
        private class RouteTagArrayAdapter : ArrayAdapter<RouteTagHolder> {

            private readonly LayoutInflater mInflater;

            /// <summary>
            ///  Constructor for RouteTagArrayAdapter
             ///
             /// @param context current context
             /// @param tags    list of tags as RouteTagHolder
            /// </summary>
            public RouteTagArrayAdapter (Context context, List<RouteTagHolder> tags) : base (context, Resource.Layout.activity_route_filter_row_item, tags)
            {
                mInflater = LayoutInflater.From (context);
            }

            public override View GetView (int position, View convertView, ViewGroup parent)
            {
                var tagHolder = GetItem (position);
                var tag = tagHolder.getRouteTag ();

                CheckBox checkBox;
                ImageView imageView;

                if (convertView == null)
                {
                    convertView = mInflater.Inflate (Resource.Layout.activity_route_filter_row_item, null);

                    checkBox = (CheckBox) convertView.FindViewById (Resource.Id.routeFilterRowItemCheckBox);
                    imageView = (ImageView) convertView.FindViewById (
                        Resource.Id.routeFilterRowItemImage);

                    convertView.Tag = new RouteTagViewHolder (checkBox, imageView);

                    checkBox.SetOnClickListener (new CheckBoxViewOnClickListener ());
                }
                else
                {
                    var viewHolder = (RouteTagViewHolder) convertView.Tag;
                    checkBox = viewHolder.getCheckBox ();
                    imageView = viewHolder.getImageView ();
                }

                checkBox.Tag = tagHolder;
                checkBox.Text = tag.Name;
                checkBox.Checked = tagHolder.isSelected ();
                imageView.SetImageDrawable (tag.Image.GetDrawable (Context));

                return convertView;
            }

        }


        private class CheckBoxViewOnClickListener : Object, View.IOnClickListener {

            public void OnClick (View v)
            {
                var cb = (CheckBox) v;
                var tagHolder = (RouteTagHolder) v.Tag;
                tagHolder.setSelected (cb.Checked);
            }

        }

        private class CloseWithoutSaveOnClickListener : Object, View.IOnClickListener {

            private readonly RouteFilterActivity routeFilterActivity;

            public CloseWithoutSaveOnClickListener (RouteFilterActivity routeFilterActivity)
            {
                this.routeFilterActivity = routeFilterActivity;
            }

            public void OnClick (View v)
            {
                routeFilterActivity.FinishWithResult (Result.Canceled);
            }

        }

        private class CloseWithSaveOnClickListener : Object, View.IOnClickListener {

            private readonly ArrayAdapter<RouteTagHolder> adapter;
            private readonly RouteFilterActivity routeFilterActivity;

            public CloseWithSaveOnClickListener (RouteFilterActivity routeFilterActivity, ArrayAdapter<RouteTagHolder> adapter)
            {
                this.routeFilterActivity = routeFilterActivity;
                this.adapter = adapter;
            }

            public void OnClick (View v)
            {
                IList<string> activeTags = new List<string> ();
                for (var i = 0; i < adapter.Count; i++)
                {
                    var tagHolder = adapter.GetItem (i);
                    if (tagHolder.isSelected ())
                    {
                        activeTags.Add (tagHolder.getRouteTag ().Tag);
                    }
                }
                var intent = new Intent ();
                intent.PutExtra (IntentActiveTags, activeTags.ToArray ());
                routeFilterActivity.FinishWithResultAndIntent (Result.Ok, intent);
            }

        }

    }
}