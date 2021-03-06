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

using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers.Contracts
{
    public interface IAchievementFetcher
    {
        /// <summary>
        /// Fetches all achievements from the API and stores
        /// them in the database, including locked ones.
        /// WARNING: If you call this method, use the return value to show
        /// notifications about new unlocked achievements!
        /// </summary>
        /// <returns>Achievements that have been newly added to the database and are unlocked.</returns>
        Task<IEnumerable<AchievementBase>> UpdateAchievements();
    }
}