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

using System.IO;
using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models
{
    public static class AchievementExtensions
    {
        public static async Task<Stream> LoadImage(this IAchievement achievement)
        {
            var url = $"{ServerEndpoints.ThumbnailApiPath}?Url={achievement.ThumbnailUrl}";
            try
            {
                var response = await new ContentApiClient("").GetHttpWebResponse(url);
                return response.GetResponseStream();
            }
            catch (NotFoundException)
            {
                return Stream.Null;
            }
        }
    }
}