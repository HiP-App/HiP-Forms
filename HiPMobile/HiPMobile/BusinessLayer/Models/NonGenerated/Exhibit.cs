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

using System;


namespace de.upb.hip.mobile.pcl.BusinessLayer.Models {
    public partial class Exhibit {

        /// <summary>
        /// Calculate the distance between the exhibit and a given point.
        /// </summary>
        /// <param name="location">The location to calculate the distance to.</param>
        /// <returns>The distance.</returns>
        public double GetDistance (GeoLocation location)
        {
            // Harversine formula
            double R = 6373;
            double distance = 0.0;

            double dLat = toRadian (this.Location.Latitude - location.Latitude);
            double dLon = toRadian (this.Location.Longitude - location.Longitude);
            double a = Math.Sin (dLat / 2) * Math.Sin (dLat / 2) +
                       Math.Cos (this.toRadian (this.Location.Latitude)) * Math.Cos (this.toRadian (location.Latitude)) *
                       Math.Sin (dLon / 2) * Math.Sin (dLon / 2);

            double c = 2 * Math.Asin (Math.Min (1, Math.Sqrt (a)));
            double d = R * c;
            distance += d;

            return distance;
        }

        private double toRadian(double val)
        {
            return (Math.PI / 180) * val;
        }

    }
}