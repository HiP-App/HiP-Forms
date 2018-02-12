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

using System;
using System.Collections.Generic;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models
{
    public partial class Page : IIdentifiable
    {
        //Attributes
        public string Id { get; set; }

        public Audio Audio { get; set; }

        public int IdForRestApi { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        //Associations
        public AppetizerPage AppetizerPage { get; set; }

        public ImagePage ImagePage { get; set; }

        public TextPage TextPage { get; set; }

        public TimeSliderPage TimeSliderPage { get; set; }

        public IList<Page> AdditionalInformationPages { get; }

        // Contructor
        public Page()
        {
        }
    }
}

