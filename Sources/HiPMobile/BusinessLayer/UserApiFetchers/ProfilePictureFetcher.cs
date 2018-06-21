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

using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.UserApiAccesses;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.UserApiFetchers
{
    public class ProfilePictureFetcher : IProfilePictureFetcher
    {
        private readonly ProfilePictureApiAccess client;

        public ProfilePictureFetcher(ProfilePictureApiAccess client)
        {
            this.client = client;
        }
        public async Task<ProfilePicture> GetProfilePicture(string userId, string accessToken)
        {
            ProfilePicture profilePicture = await client.GetProfilePicture(userId, accessToken);
            if (profilePicture == null)
            {
                return null;
            }
            else
            {
                return new ProfilePicture
                {
                    UserId = profilePicture.UserId,
                    Data = profilePicture.Data
                };
            }
            
        }
    }
}