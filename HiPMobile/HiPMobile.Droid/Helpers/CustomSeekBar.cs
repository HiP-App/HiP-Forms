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
using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Widget;

namespace de.upb.hip.mobile.droid.Helpers
{
    public class CustomSeekBar : SeekBar
    {

        private static readonly int HALF_SIZE = 15;

        private Paint selectedColor, unselectedColor;
        private Paint textPaint;
        private RectF position;
        public List<int> DotList { get; set; } = new List<int>();

        public CustomSeekBar(Context context) : base(context)
        {
            Init();
        }

        private void Init()
        {
            selectedColor = new Paint(PaintFlags.AntiAlias);
            selectedColor.Color = Resources.GetColor(Resource.Color.colorPrimary);
            selectedColor.SetStyle(Paint.Style.Fill);

            unselectedColor = new Paint(PaintFlags.AntiAlias);
            unselectedColor.Color = Resources.GetColor(Resource.Color.textColorSecondaryInverse);

            selectedColor.SetStyle(Paint.Style.Fill);
            position = new RectF();

            textPaint = new Paint(PaintFlags.AntiAlias);
            textPaint.Color = Resources.GetColor(Resource.Color.textColorSecondary);
            textPaint.TextSize = 48;
        }

        protected override void OnDraw(Canvas canvas)
        {
            int paddingLeft = PaddingLeft;
            int paddingTop = PaddingTop;
            float margin = (canvas.Width - (paddingLeft + PaddingRight));
            float halfHeight = (canvas.Height + paddingTop) * .5f;

            for (int i = 0; i < DotList.Count; i++)
            {
                int pos = (int)(margin / 100 * DotList.ElementAt(i));

                position.Set(paddingLeft + pos - HALF_SIZE, halfHeight - HALF_SIZE,
                        paddingLeft + pos + HALF_SIZE, halfHeight + HALF_SIZE);

                int progress = DotList.ElementAt(i);
                canvas.DrawOval(
                        position,
                        (progress < Progress) ? selectedColor : unselectedColor
                );
                //            canvas.drawText("653", paddingLeft + pos - HALF_SIZE * 2, halfHeight + HALF_SIZE * 4, mTextPaint);
            }

            base.OnDraw(canvas);
        }
    }
}