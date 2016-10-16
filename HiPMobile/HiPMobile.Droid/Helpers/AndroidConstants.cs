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
using de.upb.hip.mobile.pcl.BusinessLayer.Models;

namespace de.upb.hip.mobile.droid.Helpers {
    public static class AndroidConstants {

        public static int DatabaseVersion { get; private set; } = 1;

        public static readonly GeoLocation PaderbornMainStation = new GeoLocation
        {
            Latitude = 51.71352,
            Longitude = 8.74021
        };

        public static readonly GeoLocation PaderbornCenter = new GeoLocation
        {
            Latitude = 51.7189205,
            Longitude = 8.7575093
        };

        public static readonly int MinTimeBwUpdates = 2000; //2000 milliseconds (2seconds)
        public static readonly int MinDistanceChangeForUpdates = 10; // 2 metres
        public static readonly double ExhibitRadius = 0.02; // 20m

    }
}