using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Util;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using de.upb.hip.mobile.droid.Activities;
using Java.Lang;

namespace de.upb.hip.mobile.droid.Listeners
{
    class RecyclerItemClickListener : RecyclerView.IOnItemTouchListener {

        private MainActivity mMainActivity;
        private GestureDetector mGestureDetector;

        public RecyclerItemClickListener (MainActivity mMainActivity)
        {
            this.mMainActivity = mMainActivity;
            mGestureDetector = new GestureDetector (mMainActivity,new GestureDetector.SimpleOnGestureListener ());
        }



        public IntPtr Handle
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool OnInterceptTouchEvent(RecyclerView view, MotionEvent e)
        {
            View childView = view.FindChildViewUnder(e.RawX, e.RawY);
            if (childView != null && mGestureDetector.OnTouchEvent(e))
            {

                Intent intent = new Intent(mMainActivity, typeof(DetailsActivity));



                    ActivityOptionsCompat activityOptions =
                    ActivityOptionsCompat.MakeSceneTransitionAnimation(
                            this.mMainActivity,
                            // Now we provide a list of Pair items which contain the view we can
                            // transitioning from, and the name of the view it is transitioning to,
                            // in the launched activity
                            new Pair(childView.FindViewById(Resource.Id.mainRowItemImage),
                                    DetailsActivity.VIEW_NAME_IMAGE),
                            new Pair(childView.FindViewById(Resource.Id.mainRowItemName),
                                    DetailsActivity.VIEW_NAME_TITLE));

        intent.PutExtra(DetailsActivity.INTENT_EXHIBIT_ID, childView.Id);
            ActivityCompat.StartActivity(this.mMainActivity, intent, activityOptions.ToBundle());
        }
        return false;
        }

        public void OnRequestDisallowInterceptTouchEvent(bool disallowIntercept)
        {
            throw new NotImplementedException();
        }

        public void OnTouchEvent(RecyclerView rv, MotionEvent @event)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
