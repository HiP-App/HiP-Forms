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
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using de.upb.hip.mobile.droid.Activities;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;

namespace de.upb.hip.mobile.droid.Listeners {
    public class RecyclerItemClickListener : Java.Lang.Object, RecyclerView.IOnItemTouchListener  {

        private MainActivity mainActivity;
        private ExhibitSet exhibitSet;
        private GestureDetector gestureDetector;

        public RecyclerItemClickListener (MainActivity mainActivity, ExhibitSet exhibitSet)
        {
            this.mainActivity = mainActivity;
            this.exhibitSet = exhibitSet;
            gestureDetector = new GestureDetector (mainActivity, new MyCustomSimpleOnGestureListener());
        }


        public bool OnInterceptTouchEvent(RecyclerView view, MotionEvent e)
        {
            View childView = view.FindChildViewUnder(e.GetX(), e.GetY());
            if (childView != null && gestureDetector.OnTouchEvent(e))
            {
                Intent intent = new Intent(this.mainActivity, typeof (ExhibitDetailsActivity));

                Exhibit exhibit = null;
                for (int i = 0; i < exhibitSet.ActiveSet.Count; ++i)
                {
                //TODO does this work
                    exhibit = exhibitSet.ActiveSet[i];
                    if (exhibit.Id == childView.ContentDescription)
                        break;
                }

                if (exhibit != null)
                {
                    var pageList = exhibit.Pages;
                    if (pageList == null || !pageList.Any())
                    {
                        Toast.MakeText(mainActivity,
                            mainActivity.GetString(Resource.String.currently_no_further_info),
                            ToastLength.Short)
                            .Show();
                        return false;
                    }
                    intent.PutExtra(ExhibitDetailsActivity.INTENT_EXTRA_EXHIBIT_ID, exhibit.Id);
                    ActivityCompat.StartActivity(this.mainActivity, intent, null);
                }
            }
            return false;
        }

        public void OnRequestDisallowInterceptTouchEvent(bool disallowIntercept)
        {
        }

        public void OnTouchEvent(RecyclerView rv, MotionEvent @event)
        {
        }

        public void Dispose()
        {
        }

        /*public class MyCustomSimpleOnGestureListener : GestureDetector.SimpleOnGestureListener
        {
            public override bool OnSingleTapUp(MotionEvent e)
            {
                return true;
            }
        }*/

    }
}