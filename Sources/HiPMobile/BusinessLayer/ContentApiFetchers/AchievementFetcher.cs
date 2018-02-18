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

using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.DtoToModelConverters;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers
{
    public class AchievementFetcher : IAchievementFetcher
    {
        private readonly IAchievementsApiAccess client;

        public AchievementFetcher(IAchievementsApiAccess client)
        {
            this.client = client;
        }

        public async Task<IEnumerable<IAchievement>> UpdateAchievements(ITransactionDataAccess dataAccess)
        {
            var existingUnlocked = dataAccess.Achievements().GetAchievements()
                .Where(it => it.IsUnlocked)
                .Select(it => it.Id);

            var achievementDtos = await client.GetAchievements();
            var unlockedAchievementIds = (await client.GetUnlockedAchievements())
                .Select(it => it.Id.ToString()).ToList();

            var achievements = AchievementConverter.Convert(achievementDtos, dataAccess).ToList();
            foreach (var unlocked in achievements.Where(it => unlockedAchievementIds.Contains(it.Id)))
            {
                unlocked.IsUnlocked = true;
            }
            return achievements.Where(it => it.IsUnlocked && !existingUnlocked.Contains(it.Id));
        }
    }
}