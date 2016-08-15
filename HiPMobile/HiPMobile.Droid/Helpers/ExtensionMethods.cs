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

using System.Threading.Tasks;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;

namespace de.upb.hip.mobile.droid.Helpers {
    public static class ExtensionMethods {

        /// <summary>
        /// Loads the drawable from the given image. The whole image is loaded into memory. For saving memory please use the other method overload.
        /// </summary>
        /// <param name="img"></param>
        /// <param name="context">The context of the drawable.</param>
        /// <returns></returns>
        public static Drawable GetDrawable (this Image img, Context context)
        {
            return new BitmapDrawable (context.Resources, BitmapFactory.DecodeByteArray (img.Data, 0, img.Data.Length));
        }

        /// <summary>
        /// Loads the drawable from the given image in the given height.
        /// </summary>
        /// <param name="img"></param>
        /// <param name="context">The context of the drawable.</param>
        /// <param name="width">The desired width.</param>
        /// <param name="height">The desired height.</param>
        /// <returns></returns>
        public static BitmapDrawable GetDrawable(this Image img, Context context, int width, int height)
        {
            return new BitmapDrawable(context.Resources, LoadScaledDownBitmapForDisplayAsync (img, new BitmapFactory.Options (), width, height));
        }

        /// <summary>
        /// Gets the dimensions for the given image.
        /// </summary>
        /// <param name="img">The image to get the dimensions from.</param>
        /// <returns>The dimension of the image.</returns>
        public static int[] GetDimensions (this Image img)
        {
            var bmp = BitmapFactory.DecodeByteArray (img.Data, 0, img.Data.Length);
            return new[] {bmp.Width, bmp.Height};
        }

        /// <summary>
        /// https://developer.xamarin.com/recipes/android/resources/general/load_large_bitmaps_efficiently/
        /// </summary>
        /// <param name="options"></param>
        /// <param name="reqWidth"></param>
        /// <param name="reqHeight"></param>
        /// <returns></returns>
        public static int CalculateInSampleSize(BitmapFactory.Options options, int reqWidth, int reqHeight)
        {
            // Raw height and width of image
            float height = options.OutHeight;
            float width = options.OutWidth;
            double inSampleSize = 1D;

            if (height > reqHeight || width > reqWidth)
            {
                int halfHeight = (int)(height / 2);
                int halfWidth = (int)(width / 2);

                // Calculate a inSampleSize that is a power of 2 - the decoder will use a value that is a power of two anyway.
                while ((halfHeight / inSampleSize) > reqHeight && (halfWidth / inSampleSize) > reqWidth)
                {
                    inSampleSize *= 2;
                }
            }

            return (int)inSampleSize;
        }

        /// <summary>
        /// Adapted from https://developer.xamarin.com/recipes/android/resources/general/load_large_bitmaps_efficiently/
        /// </summary>
        /// <param name="img"></param>
        /// <param name="options"></param>
        /// <param name="reqWidth"></param>
        /// <param name="reqHeight"></param>
        /// <returns></returns>
        public static Bitmap LoadScaledDownBitmapForDisplayAsync(this Image img, BitmapFactory.Options options, int reqWidth, int reqHeight)
        {
            // Calculate inSampleSize
            options.InSampleSize = CalculateInSampleSize(options, reqWidth, reqHeight);

            // Decode bitmap with inSampleSize set
            options.InJustDecodeBounds = false;

            return BitmapFactory.DecodeByteArray(img.Data, 0, img.Data.Length);
        }

    }
}