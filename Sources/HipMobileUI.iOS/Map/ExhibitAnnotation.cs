// // Copyright (C) 2016 History in Paderborn App - Universität Paderborn
// //  
// // Licensed under the Apache License, Version 2.0 (the "License");
// // you may not use this file except in compliance with the License.
// // You may obtain a copy of the License at
// //  
// //       http://www.apache.org/licenses/LICENSE-2.0
// //  
// // Unless required by applicable law or agreed to in writing, software
// // distributed under the License is distributed on an "AS IS" BASIS,
// // WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// // See the License for the specific language governing permissions and
// // limitations under the License.

using CoreLocation;
using MapKit;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;

namespace PaderbornUniversity.SILab.Hip.Mobile.Ios.Map
{
    public class ExhibitAnnotation : MKAnnotation
    {
        public override CLLocationCoordinate2D Coordinate { get; }

        public override string Title { get; }

        public override string Subtitle { get; }

        public string ExhibitId { get; }

        public ExhibitAnnotation(CLLocationCoordinate2D coord, Exhibit exhibit)
        {
            Coordinate = coord;
            Title = exhibit.Name;
            // Prevent exception due to null-description
            Subtitle = exhibit.Description ?? "";
            ExhibitId = exhibit.Id;
        }

        public ExhibitAnnotation(double latitude, double longitude, Exhibit exhibit) :
            this(new CLLocationCoordinate2D(latitude, longitude), exhibit)
        {
        }
    }
}