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
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers
{
    public class MathUtil
    {
        /// <summary>
        ///     distance calculations between two lat/lon coordinates in meter
        /// </summary>
        public static double CalculateDistance(GeoLocation one, GeoLocation two)
        {
            var dlon = ToRadians(two.Longitude - one.Longitude);
            var dlat = ToRadians(two.Latitude - one.Latitude);
            var a = Math.Pow(Math.Sin(dlat / 2), 2) + Math.Cos(ToRadians(one.Latitude)) * Math.Cos(ToRadians(two.Latitude)) * Math.Pow(Math.Sin(dlon / 2), 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return 6373 * c * 1000;
        }

        /// <summary>
        /// Convert degree to radians.
        /// </summary>
        /// <param name="deg">The degree value.</param>
        /// <returns>The radians value.</returns>
        public static double ToRadians(double deg)
        {
            return deg * (Math.PI / 180);
        }
    }
}