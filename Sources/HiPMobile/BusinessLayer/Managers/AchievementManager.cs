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
using System.Linq;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers
{
    public static class AchievementManager
    {
        private static readonly IDataAccess DataAccess = IoCManager.Resolve<IDataAccess>();

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
    }
}