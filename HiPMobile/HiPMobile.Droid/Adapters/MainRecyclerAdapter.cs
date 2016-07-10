// Copyright (C) 2016 History in Paderborn App - Universität Paderborn
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using de.upb.hip.mobile.droid.Helpers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;

namespace de.upb.hip.mobile.droid.Adapters {
    internal class MainRecyclerAdapter : RecyclerView.Adapter {

        private readonly ExhibitSet displayedExhibitSet;
        private readonly GeoLocation location;

        private Context context;

        /// <summary>
        ///     Constructor for the MainRecyclerAdapter
        /// </summary>
        /// <param name="exhibitSet">The displayed exhibitSet.</param>
        /// <param name="location">The location of the device.</param>
        /// <param name="context"></param>
        public MainRecyclerAdapter (ExhibitSet exhibitSet, GeoLocation location, Context context)
        {
            displayedExhibitSet = exhibitSet;
            this.location = location;
            this.context = context;
        }


        /// <summary>
        ///     Calculates the size of displayedExhibitSet (invoked by the layout manager).
        /// </summary>
        public override int ItemCount {
            get { return displayedExhibitSet.ActiveSet.Count (); }
        }


        public override RecyclerView.ViewHolder OnCreateViewHolder (ViewGroup parent, int viewType)
        {
            // create a new view
            var v = LayoutInflater.From (parent.Context).Inflate (
                Resource.Layout.activity_main_row_item, parent, false);

            return new ViewHolder (v);
        }

        public override void OnBindViewHolder (RecyclerView.ViewHolder holder, int position)
        {
            var vh = holder as ViewHolder;

            // get Exhibit from displayedExhibitSet at position
            var exhibit = displayedExhibitSet.ActiveSet.ElementAt (position);

            // update the holder with new data
            vh.Name.SetText (exhibit.Name, TextView.BufferType.Normal);

            var doubleDistance = exhibit.GetDistance (location) * 1000;

            string distance;
            if (doubleDistance > 1000)
            {
                distance = Math.Round ((decimal) doubleDistance / 1000, 2) + "km";
            }
            else
            {
                distance = Math.Round ((decimal) doubleDistance, 2) + "m";
            }

            //Remove this if not needed
            vh.View.ContentDescription = exhibit.Id;

            vh.Distance.SetText (distance, TextView.BufferType.Normal);

            var d = exhibit.Image.Data;
            var bm = BitmapFactory.DecodeByteArray (d, 0, d.Length);
            var bitmapDrawable = new BitmapDrawable (bm);
            var bmp = bitmapDrawable.Bitmap;
            vh.Image.SetImageBitmap (ImageManipulation.GetCroppedImage (bmp, 100));
        }


        /// <summary>
        ///     Provide a reference to the views for each data item
        ///     Complex data items may need more than one view per item, and
        ///     you provide access to all the views for a data item in a view holder
        /// </summary>
        public class ViewHolder : RecyclerView.ViewHolder {

            public ViewHolder (View v) : base (v)
            {
                View = v;
                Image = (ImageView) v.FindViewById (Resource.Id.mainRowItemImage);
                Name = (TextView) v.FindViewById (Resource.Id.mainRowItemName);
                Distance = (TextView) v.FindViewById (Resource.Id.mainRowItemDistance);
            }

            // each data item is just a string in this case
            public View View { get; set; }
            public ImageView Image { get; set; }
            public TextView Name { get; set; }
            public TextView Distance { get; set; }

        }

    }
}