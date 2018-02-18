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
    public partial class Exhibit : IIdentifiable, IDownloadable
    {
        //Attributes
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        [NotMapped]
        public GeoLocation Location
        {
            get => GeoLocationConverter.FromString(LocationAsString);
            set => LocationAsString = GeoLocationConverter.ToString(value);
        }

        public string LocationAsString { get; set; }

        [NotMapped] // purpose of this property is unclear
        public IList<string> Categories { get; }

        [NotMapped] // purpose of this property is unclear
        public IList<string> Tags { get; }

        public int Radius { get; set; }

        public int IdForRestApi { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public DateTimeOffset? LastNearbyTime { get; set; }

        public bool DetailsDataLoaded { get; set; }

        public bool Unlocked { get; set; }

        //Associations
        public ICollection<Page> Pages => new JoinCollectionFacade<Page, Exhibit, JoinExhibitPage>(this, PagesRefs);

        public IList<JoinExhibitPage> PagesRefs { get; } = new List<JoinExhibitPage>();

        public Image Image { get; set; }

        public DownloadableType Type => DownloadableType.Exhibit;

        // Constructor
        public Exhibit()
        {
        }
    }
}

