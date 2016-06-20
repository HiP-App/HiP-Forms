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

using Android.Content;
using Android.Graphics;

namespace de.upb.hip.mobile.droid.Helpers {
    internal class ImageManipulation {

        /// <summary>
        ///     Get a round cropped version of the image.
        /// </summary>
        /// <param name="bitmap">The image bitmap.</param>
        /// <param name="radius">The radius of the cropped part.</param>
        /// <returns></returns>
        public static Bitmap GetCroppedImage (Bitmap bitmap, int radius)
        {
            Bitmap scaledBitmap;
            if (bitmap.Width != radius || bitmap.Height != radius)
            {
                scaledBitmap = Bitmap.CreateScaledBitmap (bitmap, radius, radius, false);
            }
            else
            {
                scaledBitmap = bitmap;
            }
            var output = Bitmap.CreateBitmap (scaledBitmap.Width,
                                              scaledBitmap.Height, Bitmap.Config.Argb8888);
            var canvas = new Canvas (output);

            var paint = new Paint ();
            var rect = new Rect (0, 0, scaledBitmap.Width, scaledBitmap.Height);

            paint.AntiAlias = true;
            paint.FilterBitmap = true;
            paint.Dither = true;
            canvas.DrawARGB (0, 0, 0, 0);
            paint.Color = Color.ParseColor ("#BAB399");
            canvas.DrawCircle (scaledBitmap.Width / 2 + 0.7f, scaledBitmap.Height / 2 + 0.7f,
                               scaledBitmap.Width / 2 + 0.1f, paint);
            paint.SetXfermode (new PorterDuffXfermode (PorterDuff.Mode.SrcIn));
            canvas.DrawBitmap (scaledBitmap, rect, rect, paint);

            return output;
        }

        /// <summary>
        ///     Converts a square image into a round image with marker for maps.
        /// </summary>
        /// <param name="bitmap">The rectangular image</param>
        /// <param name="context">Android Context</param>
        /// <returns>Image of marker with input image inside</returns>
        public static Bitmap GetMarker (Bitmap bitmap, Context context)
        {
            bitmap = GetCroppedImage (bitmap, 55);

            var markerBitmap = BitmapFactory.DecodeResource (
                context.Resources, Resource.Drawable.marker_blue);
            var markerBitmapWidth = markerBitmap.Width;
            var markerBitmapHeight = markerBitmap.Height;
            var bitmapWidth = bitmap.Width;

            var marginLeft = (float) (markerBitmapWidth * 0.5 - bitmapWidth * 0.5);
            float marginTop = 13;

            var overlayBitmap = Bitmap.CreateBitmap (
                markerBitmapWidth, markerBitmapHeight, markerBitmap.GetConfig ());
            var canvas = new Canvas (overlayBitmap);
            canvas.DrawBitmap (markerBitmap, new Matrix (), null);
            canvas.DrawBitmap (bitmap, marginLeft, marginTop, null);

            return overlayBitmap;
        }

    }
}