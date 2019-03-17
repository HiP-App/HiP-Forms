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

using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.JoinClasses;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models
{
    public partial class Route : IIdentifiable, IDownloadable
    {
        //Attributes
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Duration { get; set; }

        public double Distance { get; set; }

        public DateTimeOffset? LastTimeDismissed { get; set; }

        public bool DetailsDataLoaded { get; set; }

        public int IdForRestApi { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        //Associations
        [NotMapped]
        public ICollection<RouteTag> Tags => new JoinCollectionFacade<RouteTag, Route, JoinRouteTag>(this, TagsRefs);

        public IList<JoinRouteTag> TagsRefs { get; } = new List<JoinRouteTag>();

        public Image Image { get; set; }

        public IList<Waypoint> Waypoints { get; } = new List<Waypoint>();

        public Audio Audio { get; set; }

        public DownloadableType Type => DownloadableType.Route;

        //Constructor
        public Route()
        {
            DetailsDataLoaded = true;
        }
    }
}

