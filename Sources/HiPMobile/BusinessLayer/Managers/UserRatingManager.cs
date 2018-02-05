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

using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.ModelClasses;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers
{

    public interface IUserRatingManager
    {

        /// <summary>
        /// Fetch the user rating for one exhibit.
        /// </summary>
        /// <param name="idForRestApi">The id of the exhibit for which the user rating should be returned.</param>
        /// <returns>An object which represents the user rating.</returns>
        Task<UserRating> GetUserRatingAsync(int idForRestApi);

        /// <summary>
        /// Sends a rating for an exhibit to the server.
        /// </summary>
        /// <param name="idForRestApi">The id of the exhibit for which the user rating should be returned.</param>
        /// <param name="rating">The rating for an exhibit. The rating should be between 1 and 5.</param>
        /// <returns>A boolean which tells if the rating was successfully sent to the server.</returns>
        Task<bool> SendUserRatingAsync(int idForRestApi, int rating);

        /// <summary>
        /// Fetch the rating the current user has given the exhibit.
        /// </summary>
        /// <param name="idForRestApi">The id of the exhibit for which the user rating should be returned.</param>
        /// <returns>The last rating given from the logged in user.</returns>
        Task<int> GetPreviousUserRatingAsync(int idForRestApi);

        /// <summary>
        /// Initializes a dictionary with the values 1-0, 2-0, 3-0, 4-0 and 5-0.
        /// </summary>
        /// <returns>A rating table where each value is 0.</returns>
        Dictionary<int, int> InitializeEmptyRatingTable();
    }

    public class UserRatingManager : IUserRatingManager
    {

        private readonly IUserRatingApiAccess client = IoCManager.Resolve<IUserRatingApiAccess>();

        public async Task<UserRating> GetUserRatingAsync(int idForRestApi)
        {
            var userRatingDto = await client.GetUserRatingAsync(idForRestApi);
            userRatingDto.RatingTable = userRatingDto.RatingTable ?? InitializeEmptyRatingTable();
            return new UserRating(userRatingDto);
        }

        public async Task<int> GetPreviousUserRatingAsync(int idForRestApi)
        {
            return await client.GetPreviousUserRatingAsync(idForRestApi);
        }

        public async Task<bool> SendUserRatingAsync(int idForRestApi, int rating)
        {
            return await client.SendUserRatingAsync(idForRestApi, rating);
        }

        public Dictionary<int, int> InitializeEmptyRatingTable()
        {
            var ratingTable = new Dictionary<int, int>();
            for (var i = 1; i <= 5; i++)
                ratingTable.Add(i, 0);
            return ratingTable;
        }
    }
}