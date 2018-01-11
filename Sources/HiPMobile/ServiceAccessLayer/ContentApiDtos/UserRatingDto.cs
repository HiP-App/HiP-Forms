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

using Newtonsoft.Json;
using System.Collections.Generic;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiDtos
{
    public class UserRatingDto
    {
        [JsonProperty("id")]
        public int Id { get; private set; }

        [JsonProperty("average")]
        public double Average { get; private set; }

        [JsonProperty("count")]
        public int Count { get; private set; }

        [JsonProperty("ratingTable")]
        public Dictionary<int, int> RatingTable { get; private set; }

        public UserRatingDto()
        {
            Id = -1;
            Average = 0;
            Count = 0;
            RatingTable = new Dictionary<int, int>();
            for (int i = 1; i <= 5; i++)
                RatingTable.Add(i, 0);
        }

        public UserRatingDto(int id, double average, int count, Dictionary<int, int> ratingTable)
        {
            Id = id;
            Average = average;
            Count = count;
            RatingTable = ratingTable;
        }
    }
}