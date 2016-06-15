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

using Android.Content.Res;
using Android.Util;

namespace de.upb.hip.mobile.droid.Helpers
{
    /// <summary>
    /// Provides functionality to convert from pixels to dp and vice versa.
    /// (see http://stackoverflow.com/questions/4605527/converting-pixels-to-dp)
    /// </summary>
    public class PixelDpConversion
    {
        /// <summary>
        /// This method converts dp unit to equivalent pixels, depending on device density.
        /// </summary>
        /// <param name="dp">A value in dp (density independent pixels) unit.</param>
        /// <returns>A float value to represent px equivalent to dp depending on device density.</returns>
        public static float ConvertDpToPixel(float dp)
        {
            DisplayMetrics metrics = Resources.System.DisplayMetrics;
            return dp * ((float)metrics.DensityDpi / (float)DisplayMetricsDensity.Default);
        }

        /// <summary>
        /// This method converts device specific pixels to density independent pixels.
        /// </summary>
        /// <param name="px">A value in px (pixels) unit.</param>
        /// <returns>A float value to represent dp equivalent to px value.</returns>
        public static float ConvertPixelsToDp(float px)
        {
            DisplayMetrics metrics = Resources.System.DisplayMetrics;
            return px / ((float)metrics.DensityDpi / (float)DisplayMetricsDensity.Default);
        }
    }
}