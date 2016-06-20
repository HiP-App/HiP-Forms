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

using System;
using System.Collections.Generic;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;

namespace de.upb.hip.mobile.droid.Helpers {
    /// <summary>
    ///     An encapsulation class for ImageView that allows drawing on it.
    /// </summary>
    public class DrawView : ImageView {

        public DrawView (IntPtr javaReference, JniHandleOwnership transfer) : base (javaReference, transfer)
        {
        }

        public DrawView (Context context) : base (context)
        {
        }

        public DrawView (Context context, IAttributeSet attrs) : base (context, attrs)
        {
        }

        public DrawView (Context context, IAttributeSet attrs, int defStyleAttr) : base (context, attrs, defStyleAttr)
        {
        }

        public DrawView (Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base (context, attrs, defStyleAttr, defStyleRes)
        {
        }

        public bool DrawOnImage { get; set; } = true;
        public List<Rectangle> Rectangles { get; } = new List<Rectangle> ();

        //Needed for scaling the drawed rectangles to the correct size
        public int[] OriginalImageDimensions { get; set; } = {1, 1};

        protected override void OnDraw (Canvas canvas)
        {
            base.OnDraw (canvas);

            if (!DrawOnImage)
            {
                return;
            }

            var paint = new Paint ();
            paint.Color = Color.Red;
            paint.SetStyle (Paint.Style.Stroke);
            paint.StrokeWidth = 10;
            foreach (var rect in Rectangles)
            {
                var widthScalingFactor = Width / (double) OriginalImageDimensions [0];
                var heightScalingFactor = Height / (double) OriginalImageDimensions [1];

                canvas.DrawRect ((int) (rect.Left * widthScalingFactor),
                                 (int) (rect.Top * heightScalingFactor),
                                 (int) (rect.Right * widthScalingFactor),
                                 (int) (rect.Bottom * heightScalingFactor),
                                 paint);
            }
        }

    }
}