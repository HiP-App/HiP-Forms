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

using Android.Content;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;

namespace de.upb.hip.mobile.droid.Helpers
{
    public class CustomViewPager : ViewPager
    {
        public CustomViewPager(Context context) : base(context)
        {

        }

        public CustomViewPager(Context context, IAttributeSet attrs) : base(context, attrs)
        {

        }


        public override bool OnTouchEvent(MotionEvent e)
        {
            return Enabled && base.OnTouchEvent(e);
        }

        public override bool OnInterceptTouchEvent(MotionEvent e)
        {
            return Enabled && base.OnInterceptTouchEvent(e);
        }
    }
}