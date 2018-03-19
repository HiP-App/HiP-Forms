// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
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

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models
{
    public struct GeoLocation
    {
        public double Latitude { get; }
        public double Longitude { get; }

        public GeoLocation(double latitude, double longitude) : this()
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public static bool operator ==(GeoLocation a, GeoLocation b) => a.Equals(b);

        public static bool operator !=(GeoLocation a, GeoLocation b) => !a.Equals(b);

        // Structs have default implementations of Equals and GetHashCode, but overriding
        // == and != without overriding Equals and GetHashCode causes warnings.
        public override bool Equals(object obj) => base.Equals(obj);

        public override int GetHashCode() => base.GetHashCode();
    }
}

