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
using Android.Util;
using Android.Widget;

namespace de.upb.hip.mobile.droid.Helpers
{
	public class CustomSeekBar : SeekBar
	{
		private Paint selectedColor, unselectedColor;
		private const int MaxDotHeight = 15;

		public CustomSeekBar(Context context) : base(context)
		{
			Init();
		}

		public CustomSeekBar(Context context, IAttributeSet attrs) : base(context, attrs)
		{
			Init();
		}

		public List<int> DotList { get; set; } = new List<int>();

		private void Init()
		{
			selectedColor = new Paint(PaintFlags.AntiAlias)
			{
				Color = Resources.GetColor(Resource.Color.colorPrimary)
			};
			selectedColor.SetStyle(Paint.Style.Fill);

			unselectedColor = new Paint(PaintFlags.AntiAlias)
			{
				Color = Resources.GetColor(Resource.Color.textColorSecondaryInverse)
			};

			selectedColor.SetStyle(Paint.Style.Fill);
		}

		protected override void OnDraw(Canvas canvas)
		{
			var paddingLeft = PaddingLeft;
			var paddingRight = PaddingRight;
			var paddingTop = PaddingTop;

			float seekBarWidth = canvas.Width - (paddingLeft + paddingRight);
			var halfOfSeekbarHeight = (canvas.Height + paddingTop) * .5f;
			int dotRadius = halfOfSeekbarHeight >= MaxDotHeight ? MaxDotHeight
																	: Convert.ToInt32(halfOfSeekbarHeight);
			ThumbOffset = dotRadius;

			for (var i = 0; i < DotList.Count; i++)
			{
				var middleOfDot = seekBarWidth * DotList.ElementAt(i) / 100;
				var positionOfDot = new RectF();
				positionOfDot.Set(paddingLeft + middleOfDot - dotRadius,
									halfOfSeekbarHeight - dotRadius,
									paddingLeft + middleOfDot + dotRadius,
									halfOfSeekbarHeight + dotRadius);

				var progress = DotList.ElementAt(i);
				canvas.DrawOval(
					positionOfDot,
					progress <= Progress ? selectedColor : unselectedColor
				);
			}

			base.OnDraw(canvas);
		}

	}
}