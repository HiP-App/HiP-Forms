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

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiDtos
{
    public class AchievementDto
    {
        [JsonProperty("id")]
        public int Id { get; private set; }

        [JsonProperty("title")]
        public string Title { get; private set; }

        [JsonProperty("description")]
        public string Description { get; private set; }

        [JsonProperty("thumbnailUrl")]
        public string ThumbnailUrl { get; private set; }

        [JsonProperty("nextId")]
        public int? NextId { get; private set; }

        [JsonProperty("points")]
        public int Points { get; private set; }

        public AchievementDto()
        {
        }

        public AchievementDto(int id, string title, string description, string thumbnailUrl, int? nextId, int points)
        {
            Id = id;
            Title = title;
            Description = description;
            ThumbnailUrl = thumbnailUrl;
            NextId = nextId;
            Points = points;
        }
    }

    public class RouteFinishedAchievementDto : AchievementDto
    {
        
        [JsonProperty("routeId")]
        public int RouteId { get; private set; }
        
        public RouteFinishedAchievementDto()
        {
        }

        public RouteFinishedAchievementDto(int id, string title, string description, string thumbnailUrl, int? nextId, int points, int routeId) :
            base(id, title, description, thumbnailUrl, nextId, points)
        {
            RouteId = routeId;
        }
    }

    public class ExhibitsVisitedAchievementDto : AchievementDto
    {
        [JsonProperty("count")]
        public int Count { get; private set; }

        public ExhibitsVisitedAchievementDto()
        {
        }

        public ExhibitsVisitedAchievementDto(int id, string title, string description, string thumbnailUrl, int? nextId, int count, int points) :
            base(id, title, description, thumbnailUrl, nextId, points)
        {
            Count = count;
        }
    }
}