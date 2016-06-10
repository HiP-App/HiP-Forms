using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using de.upb.hip.mobile.droid.Helpers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using de.upb.hip.mobile.pcl.DataLayer;
using Java.Lang;
using String = System.String;

namespace de.upb.hip.mobile.droid.Adapters {
    class MainRecyclerAdapter : RecyclerView.Adapter {

        private ExhibitSet mExhibitSet;
        private GeoLocation location;
        private Context mContext;




        /**
     * Constructor for the MainRecyclerAdapter
     *
     * @param exhibitSet set of Exhibits to be shown
     */

        public MainRecyclerAdapter (ExhibitSet exhibitSet, GeoLocation location,Context context)
        {
            this.mExhibitSet = exhibitSet;
            this.location = location;
            this.mContext = context;
        }



        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // create a new view
            View v = LayoutInflater.From(parent.Context).Inflate(
                Resource.Layout.activity_main_row_item, parent, false);

            return new ViewHolder(v);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {

            MainRecyclerAdapter.ViewHolder vh = holder as MainRecyclerAdapter.ViewHolder;

            // get Exhibit from mExhibitSet at position
            Exhibit exhibit = this.mExhibitSet.ActiveSet.ElementAt(position);

            // update the holder with new data
            vh.mName.SetText(exhibit.Name, TextView.BufferType.Normal);

            double doubleDistance = exhibit.GetDistance(location);

            int intDistance;
            String distance;
            if (doubleDistance > 1000)
            {
                if (doubleDistance < 10000)
                {
                    intDistance = (int)(doubleDistance / 100);
                    distance = (double)(intDistance) / 10 + "km";
                }
                else
                {
                    distance = (int)doubleDistance / 1000 + "km";
                }
            }
            else
            {
                distance = (int)doubleDistance + "m";
            }

            vh.mView.Id = Integer.ParseInt(exhibit.Id);

            vh.mDistance.SetText(distance, TextView.BufferType.Normal);

            byte[] d = exhibit.Image.Data;
            BitmapDrawable bitmapDrawable = (BitmapDrawable)d;
            Bitmap bmp = bitmapDrawable.Bitmap;
            vh.mImage.SetImageBitmap(ImageManipulation.getCroppedImage(bmp, 100));
        }



       /* public ViewHolder OnCreateViewHolder (ViewGroup parent, int viewType)
      

        /**
     * Calculates the size of mExhibitSet (invoked by the layout manager)
     *
     * @return size of mExhibitSet
     */

        public override int ItemCount
        {
            get
            {
                return mExhibitSet.ActiveSet.Count();
            }
        }

        /*      Provide a reference to the views for each data item
      * Complex data items may need more than one view per item, and
      * you provide access to all the views for a data item in a view holder
      */
        public class ViewHolder : RecyclerView.ViewHolder
        {

            // each data item is just a string in this case
            public View mView { get; set; }
            public ImageView mImage { get; set; }
            public TextView mName { get; set; }
            public TextView mDistance { get; set; }


            public ViewHolder(View v) : base(v)
            {

                this.mView = v;
                this.mImage = (ImageView)v.FindViewById(Resource.Id.mainRowItemImage);
                this.mName = (TextView)v.FindViewById(Resource.Id.mainRowItemName);
                this.mDistance = (TextView)v.FindViewById(Resource.Id.mainRowItemDistance);
            }

        }

    }
}