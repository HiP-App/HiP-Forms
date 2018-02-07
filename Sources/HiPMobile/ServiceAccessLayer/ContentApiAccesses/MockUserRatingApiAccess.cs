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
    public class MockUserRatingApiAccess : IUserRatingApiAccess
    {
        public Task<UserRatingDto> GetUserRatingAsync(int idForRestApi) => Task.FromResult(
            new UserRatingDto(idForRestApi, 5, 1, new Dictionary<int, int>
                {
                    { 5, 1 },
                    { 4, 0 },
                    { 3, 0 },
                    { 2, 0 },
                    { 1, 0 }
                }
            )
        );

        public Task<bool> SendUserRatingAsync(int idForRestApi, int rating) => Task.FromResult(true);

        public Task<int> GetPreviousUserRatingAsync(int idForRestApi) => Task.FromResult(3);
    }
}