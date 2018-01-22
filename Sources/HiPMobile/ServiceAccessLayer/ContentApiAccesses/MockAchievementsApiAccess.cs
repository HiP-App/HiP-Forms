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
using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiDtos;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses
{
    public class MockAchievementsApiAccess : IAchievementsApiAccess
    {
        private readonly IEnumerable<AchievementDto> achievements = new List<AchievementDto>
        {
            new RouteFinishedAchievementDto(0, "Route finished achievement", "Description", "TODO thumburl", -1, 10, 0),
            new ExhibitsVisitedAchievementDto(1, "Exhibits visited achievement", "Description", "TODO thumburl", -1, 10, 5)
        };

        public Task<IEnumerable<AchievementDto>> GetUnlockedAchievements() => Task.FromResult(achievements);

        public Task<IEnumerable<AchievementDto>> GetAchievements() => Task.FromResult(achievements);

        public Task PostExhibitVisited(ExhibitsVisitedActionDto action) => Task.FromResult(0);
    }
}