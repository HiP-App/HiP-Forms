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
namespace de.upb.hip.mobile.droid.Listeners {
    internal class RecyclerItemClickListener {

/* : RecyclerView.IOnItemTouchListener {

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
        }*/

    }
}