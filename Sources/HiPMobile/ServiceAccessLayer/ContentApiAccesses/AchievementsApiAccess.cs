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
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiDtos;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses
{
    public class AchievementsApiAccess : IAchievementsApiAccess
    {
        private readonly IContentApiClient contentApiClient;

        public AchievementsApiAccess(IContentApiClient contentApiClient)
        {
            this.contentApiClient = contentApiClient;
        }

        public async Task<IEnumerable<AchievementDto>> GetUnlockedAchievements()
        {
            const string requestPath = "/Achievements/Unlocked";
            var json = await contentApiClient.GetResponseFromUrlAsString(requestPath);
            var achievementJsons = (JArray) JObject.Parse(json)["items"];
            var achievements = achievementJsons.Select<JToken, AchievementDto>(achievement =>
            {
                var type = achievement["type"].ToString();
                switch (type)
                {
                    case "ExhibitsVisited":
                        return JsonConvert.DeserializeObject<ExhibitsVisitedAchievementDto>(achievement.ToString());
                    case "RouteFinished":
                        return JsonConvert.DeserializeObject<RouteFinishedAchievementDto>(achievement.ToString());
                    default:
                        throw new SerializationException("Unexpected AchievementType!");
                }
            }).ToList();

            return achievements;
        }

        public async Task PostExhibitVisited(ExhibitsVisitedActionDto action)
        {
            const string requestPath = "/Actions/ExhibitVisited/Many";
            var json = JsonConvert.SerializeObject(action);
            var httpResponseMessage = await contentApiClient.PostRequestBody(requestPath, json);
            var responseString = await httpResponseMessage.Content.ReadAsStringAsync();
            Debug.WriteLine($"Posted {action.EntityIds.Count} visited exhibits to API with response {responseString}");
        }
    }
}