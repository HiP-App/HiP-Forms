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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Practices.ObjectBuilder2;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiDtos;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers
{
    public static class AchievementManager
    {
        private static readonly IDataAccess DataAccess = IoCManager.Resolve<IDataAccess>();

        public static IEnumerable<IAchievement> DequeuePendingAchievementNotifications()
        {
            using (DataAccess.StartTransaction())
            {
                var rAchievementsPending = DataAccess.GetItems<RouteFinishedAchievementPendingNotification>().ToList();
                var eAchievementsPending = DataAccess.GetItems<ExhibitsVisitedAchievementPendingNotification>().ToList();
                var rAchievements = rAchievementsPending.Select(it => it.Achievement).ToList();
                var eAchievements = eAchievementsPending.Select(it => it.Achievement).ToList();
                rAchievementsPending.ForEach(it => DataAccess.DeleteItem<RouteFinishedAchievementPendingNotification>(it.Id));
                eAchievementsPending.ForEach(it => DataAccess.DeleteItem<ExhibitsVisitedAchievementPendingNotification>(it.Id));
                return rAchievements.Union<IAchievement>(eAchievements);
            }
        }

        /// <summary>
        /// Retrieve achievements of any type from the local database
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IAchievement> GetAchievements()
        {
            return DataAccess.GetItems<RouteFinishedAchievement>()
                             .Union<IAchievement>(DataAccess.GetItems<ExhibitsVisitedAchievement>())
                             .ToList();
        }

        /// <summary>
        /// Post visited exhibits to API and enqueue resulting achievement
        /// notifications.
        /// </summary>
        /// <returns></returns>
        public static async Task UpdateServerAndLocalState()
        {
            await PostVisitedExhibitsToApi();
            var newlyUnlocked = await IoCManager.Resolve<IAchievementFetcher>().UpdateAchievements();
            var data = IoCManager.Resolve<IDataAccess>();
            using (data.StartTransaction())
            {
                foreach (var achievement in newlyUnlocked)
                {
                    switch (achievement)
                    {
                        case ExhibitsVisitedAchievement e:
                            if (data.GetItem<ExhibitsVisitedAchievementPendingNotification>(achievement.Id) != null)
                            {
                                continue;
                            }

                            var pendingE = data.CreateObject<ExhibitsVisitedAchievementPendingNotification>();
                            pendingE.Achievement = e;
                            pendingE.Id = e.Id;
                            break;
                        case RouteFinishedAchievement r:
                            if (data.GetItem<RouteFinishedAchievementPendingNotification>(achievement.Id) != null)
                            {
                                continue;
                            }

                            var pendingR = data.CreateObject<RouteFinishedAchievementPendingNotification>();
                            pendingR.Achievement = r;
                            pendingR.Id = r.Id;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("Unknown achievement type!");
                    }
                }
            }
        }

        /// <summary>
        /// Check which exhibits are unlocked and mark them as visited
        /// in the API.
        /// </summary>
        /// <returns></returns>
        private static async Task PostVisitedExhibitsToApi()
        {
            var exhibits = IoCManager.Resolve<IDataAccess>()
                                     .GetItems<ExhibitSet>()
                                     .SelectMany(set => set.ActiveSet);
            var visitedExhibitIds = exhibits.Where(e => e.Unlocked).Select(e => e.IdForRestApi).ToList();
            var action = new ExhibitsVisitedActionDto(visitedExhibitIds);
            await IoCManager.Resolve<IAchievementsApiAccess>().PostExhibitVisited(action);
        }
    }
}