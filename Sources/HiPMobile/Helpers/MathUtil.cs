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
using de.upb.hip.mobile.pcl.BusinessLayer.Models;

namespace de.upb.hip.mobile.pcl.Helpers {
    public class MathUtil {

        /// <summary>
        ///     distance calculations as presented on http://andrew.hedges.name/experiments/haversine/
        /// </summary>
        public static double CalculateDistance(double lat1, double long1, double lat2, double long2)
        {
            var dlon = ToRadians(long2 - long1);
            var dlat = ToRadians(lat2 - lat1);
            var a = Math.Pow(Math.Sin(dlat / 2), 2) + Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) * Math.Pow(Math.Sin(dlon / 2), 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return 6373 * c;
        }


        public static double DistanceLatLonInMeter(GeoLocation user, GeoLocation exhibit) //(double lat1, double lat2, double lon1, double lon2, double el1, double el2)
        {
            const int r = 6371; // Radius of the earth

            double latDistance = ToRadians(exhibit.Latitude - user.Latitude);
            double lonDistance = ToRadians(exhibit.Longitude - user.Longitude);
            double a = Math.Sin(latDistance / 2) * Math.Sin(latDistance / 2)
                       + Math.Cos(ToRadians(user.Latitude)) * Math.Cos(ToRadians(exhibit.Latitude)) * Math.Sin(lonDistance / 2) * Math.Sin(lonDistance / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distance = r * c * 1000; // convert to meters


            return distance;
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