﻿// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
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
    public class Waypoint : IIdentifiable
    {
        //Attributes
        public string Id { get; set; }

        public virtual GeoLocation Location { get; set; }

        public virtual bool Visited { get; set; }

        //Associations
        public virtual Exhibit Exhibit { get; set; }

        // Contructor
        public Waypoint()
        {
        }
    }
}

