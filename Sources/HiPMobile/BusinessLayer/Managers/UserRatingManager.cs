// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//       http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.ModelClasses;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses.Contracts;
using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers
{

    public interface IUserRatingManager
    {
        Task<UserRating> GetUserRatingAsync(Exhibit exhibit);
        Task<bool> SendUserRatingAsync(Exhibit exhibit, int rating);
    }

    public class UserRatingManager : IUserRatingManager
    {

        private readonly IUserRatingApiAccess client = new UserRatingApiAccess(new ContentApiClient());

        public async Task<UserRating> GetUserRatingAsync(Exhibit exhibit)
        {
            var userRatingDto = await client.GetUserRatingAsync(exhibit);
            return new UserRating(userRatingDto);
        }

        public async Task<bool> SendUserRatingAsync(Exhibit exhibit, int rating)
        {
            return await client.SendUserRatingAsync(exhibit, rating);
        }
    }
}