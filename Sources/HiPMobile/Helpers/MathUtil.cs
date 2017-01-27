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

        public static double DistanceLatLon(GeoLocation user, GeoLocation exhibit) //(double lat1, double lat2, double lon1, double lon2, double el1, double el2)
        {
            const int r = 6371; // Radius of the earth

            double latDistance = DegreeToRadian(exhibit.Latitude - user.Latitude);
            double lonDistance = DegreeToRadian(exhibit.Longitude - user.Longitude);
            double a = Math.Sin(latDistance / 2) * Math.Sin(latDistance / 2)
                       + Math.Cos(DegreeToRadian(user.Latitude)) * Math.Cos(DegreeToRadian(exhibit.Latitude)) * Math.Sin(lonDistance / 2) * Math.Sin(lonDistance / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distance = r * c * 1000; // convert to meters


            return distance;
        }

        private static double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        private static double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

    }
}