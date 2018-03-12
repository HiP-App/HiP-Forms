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
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiDtos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers
{
    public static class AchievementManager
    {
        public static Instance Achievements(this ITransactionDataAccess dataAccess) => new Instance(dataAccess);

        public struct Instance
        {
            private readonly ITransactionDataAccess _dataAccess;

            public Instance(ITransactionDataAccess dataAccess)
            {
                _dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
            }

            /// <summary>
            /// Retrieve achievements of any type from the local database
            /// </summary>
            /// <returns></returns>
            public IEnumerable<AchievementBase> GetAchievements()
            {
                return _dataAccess.GetItems<AchievementBase>().ToList();
            }

            /// <summary>
            /// Check which exhibits are unlocked and mark them as visited
            /// in the API.
            /// </summary>
            /// <returns></returns>
            internal async Task PostVisitedExhibitsToApi()
            {
                var exhibits = _dataAccess.Exhibits().GetExhibits();
                var visitedExhibitIds = exhibits.Where(e => e.Unlocked).Select(e => e.IdForRestApi).ToList();
                var action = new ExhibitsVisitedActionDto(visitedExhibitIds);
                await IoCManager.Resolve<IAchievementsApiAccess>().PostExhibitVisited(action);
            }
        }

        public static IEnumerable<IAchievement> DequeuePendingAchievementNotifications()
        {
            using (var transaction = DbManager.StartTransaction())
            {
                var dataAccess = transaction.DataAccess;

                var notifications = dataAccess.GetItems<AchievementPendingNotification>().ToList();
                notifications.ForEach(dataAccess.DeleteItem);
                return notifications.Select(it => it.Achievement);
            }
        }

        /// <summary>
        /// Post visited exhibits to API and enqueue resulting achievement
        /// notifications.
        /// </summary>
        /// <returns></returns>
        public static async Task UpdateServerAndLocalState()
        {
            using (var transaction = DbManager.StartTransaction())
            {
                var dataAccess = transaction.DataAccess;
                IEnumerable<AchievementBase> newlyUnlocked;
                try
                {
                    await dataAccess.Achievements().PostVisitedExhibitsToApi();
                    newlyUnlocked = await IoCManager.Resolve<IAchievementFetcher>().UpdateAchievements(dataAccess);
                }
                catch (Exception e)
                {
                    // If this fails, we can just log, ignore and try again later
                    Debug.WriteLine(e);
                    return;
                }

                foreach (var achievement in newlyUnlocked)
                {
                    if (dataAccess.GetItem<AchievementPendingNotification>(achievement.Id) == null)
                    {
                        dataAccess.AddItem(new AchievementPendingNotification
                        {
                            Achievement = achievement,
                            Id = achievement.Id
                        });
                    }
                }
            }
        }
    }
}