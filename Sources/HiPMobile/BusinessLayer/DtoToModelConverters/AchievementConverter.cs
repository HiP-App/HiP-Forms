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
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiDtos;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.DtoToModelConverters
{
    public static class AchievementConverter
    {
        private static readonly RouteFinishedAchievementConverter RouteFinishedAchievementConverter =
            new RouteFinishedAchievementConverter();

        private static readonly ExhibitsVisitedAchievementConverter ExhibitsVisitedAchievementConverter =
            new ExhibitsVisitedAchievementConverter();

        /// <summary>
        /// Converts AchievementDto instances to local models using the
        /// <see cref="RouteFinishedAchievementConverter"/> and <see cref="ExhibitsVisitedAchievementConverter"/>.
        /// </summary>
        /// <returns>Converted items</returns>
        /// <exception cref="ArgumentException">For unsupported achievement types.</exception>
        public static IList<IAchievement> Convert(IEnumerable<AchievementDto> dtos)
        {
            return dtos.Select<AchievementDto, IAchievement>(dto =>
            {
                switch (dto)
                {
                    case RouteFinishedAchievementDto r:
                        return RouteFinishedAchievementConverter.Convert(r, r.Id.ToString(), updateCurrent: true);
                    case ExhibitsVisitedAchievementDto e:
                        return ExhibitsVisitedAchievementConverter.Convert(e, e.Id.ToString(), updateCurrent: true);
                    default:
                        throw new ArgumentException("Unsupported achievement type!");
                }
            }).ToList();
        }
    }
}