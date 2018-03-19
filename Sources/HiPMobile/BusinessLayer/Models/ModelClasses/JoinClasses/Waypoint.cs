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

using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.JoinClasses
{
    /// <summary>
    /// Represents the many-to-many relationship between
    /// <see cref="Models.Route"/> and <see cref="Models.Exhibit"/>.
    /// </summary>
    public class Waypoint : IJoinEntity<Route>, IJoinEntity<Exhibit>
    {
        public Exhibit Exhibit { get; set; }
        public string ExhibitId { get; set; }
        Exhibit IJoinEntity<Exhibit>.Navigation { get => Exhibit; set => Exhibit = value; }

        public Route Route { get; set; }
        public string RouteId { get; set; }
        Route IJoinEntity<Route>.Navigation { get => Route; set => Route = value; }

        [NotMapped]
        public GeoLocation Location
        {
            get => GeoLocationConverter.FromString(LocationAsString);
            set => LocationAsString = GeoLocationConverter.ToString(value);
        }

        public string LocationAsString { get; set; }

        public bool Visited { get; set; }

        //Contructor
        public Waypoint()
        {
        }
    }
}

