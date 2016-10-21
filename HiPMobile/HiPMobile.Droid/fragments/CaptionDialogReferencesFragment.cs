// Copyright (C) 2016 History in Paderborn App - Universität Paderborn
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//       http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections.Generic;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using de.upb.hip.mobile.droid.Adapters;
using de.upb.hip.mobile.droid.Helpers.InteractiveSources;
using Java.Lang;

namespace de.upb.hip.mobile.droid.fragments
{
    public class CaptionDialogReferencesFragment : Fragment
    {
        public List<Source> References { get; set; }

        public RecyclerView RecyclerView { get; private set; }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_exhibit_details_caption_dialog_references, container, false);

            var recyclerView = view.FindViewById<RecyclerView>(Resource.Id.captionDialogReferencesRecyclerView);
            recyclerView.SetAdapter(new CaptionDialogReferencesRecyclerAdapter(References));
            recyclerView.SetLayoutManager(new LinearLayoutManagerWithSmoothScroller(Context));

            RecyclerView = recyclerView;

            return view;
        }

        private class LinearLayoutManagerWithSmoothScroller : LinearLayoutManager
        {
            public LinearLayoutManagerWithSmoothScroller(Context context) : base(context, (int)Android.Widget.Orientation.Vertical, false)
            {

            }

            public override void SmoothScrollToPosition(RecyclerView recyclerView, RecyclerView.State state,
                                       int position)
            {
                var smoothScroller = new TopSnappedSmoothScroller(recyclerView.Context, this)
                {
                    TargetPosition = position
                };
                StartSmoothScroll(smoothScroller);
            }

            private class TopSnappedSmoothScroller : LinearSmoothScroller
            {
                private readonly LinearLayoutManagerWithSmoothScroller scroller;
                private const float Scrollspeed = 5f;

                public TopSnappedSmoothScroller(Context context, LinearLayoutManagerWithSmoothScroller scroller) : base(context)
                {
                    this.scroller = scroller;
                }

                public override PointF ComputeScrollVectorForPosition(int targetPosition)
                {
                    return scroller.ComputeScrollVectorForPosition(targetPosition);
                }

                protected override int VerticalSnapPreference => SnapToStart;

                protected override float CalculateSpeedPerPixel(DisplayMetrics displayMetrics)
                {
                    return base.CalculateSpeedPerPixel (displayMetrics) * Scrollspeed;
                }

            }
        }
    }
}