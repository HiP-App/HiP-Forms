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

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiDtos
{
    public class PageDto
    {
        public int ExhibitId { get; set; }

        public int Id { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public PageTypeDto Type { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }

        public int? Audio { get; set; }

        public int? Image { get; set; }

        public IList<PageImageDto> Images { get; set; }

        public bool? HideYearNumbers { get; set; }

        public IList<int> AdditionalInformationPages { get; set; }

        public string Status { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public string Description { get; set; }

        public string FontFamily { get; set; }
    }
}