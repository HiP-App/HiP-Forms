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

using System.Collections.Generic;
using System.Linq;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Routing
{
    /// <summary>
    /// Class representing an ordered route. This means multiple sections of a route represented by all the geolocations on these sections.
    /// </summary>
    public class OrderedRoute
    {

        public OrderedRoute ()
        {
            RouteLocations = new List<List<GeoLocation>> ();
        }

        /// <summary>
        /// A list of sections which contain of a set of geolocations.
        /// </summary>
        public List<List<GeoLocation>> RouteLocations { get; private set; }

        /// <summary>
        /// The currenlty first section.
        /// </summary>
        public List<GeoLocation> FirstSection => RouteLocations.First ();

        /// <summary>
        /// The sections which are not the first section.
        /// </summary>
        public List<GeoLocation> NonFirstSections {
            get { return RouteLocations.GetRange (1, RouteLocations.Count-1).SelectMany (list => list).ToList(); }
        }

        /// <summary>
        /// All the geolocations of this route.
        /// </summary>
        public List<GeoLocation> Locations => RouteLocations.SelectMany (list => list).ToList ();

        /// <summary>
        /// Adds a new section to this route. A section is a list of geolocations representing a subpart of a route.
        /// </summary>
        /// <param name="section">Thew list of geolocations representing a section.</param>
        public void AddSection (List<GeoLocation> section)
        {
            if (section != null)
            {
                RouteLocations.Add (section);
            }
        }

        /// <summary>
        /// Updates an existing section of the route.
        /// </summary>
        /// <param name="section">The geolocations representing the new section.</param>
        /// <param name="position">The index of the sction to be updated.</param>
        public void UpdateSection (List<GeoLocation> section, int position)
        {
            if (section != null && position > 0 && position < RouteLocations.Count)
            {
                RouteLocations.RemoveAt (position);
                RouteLocations.Insert(position, section);
            }
        }

    }
}
