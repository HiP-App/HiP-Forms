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

using System.Threading.Tasks;
using Newtonsoft.Json;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiDtos;
using System.Net;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses
{
    public class UserRatingApiAccess : IUserRatingApiAccess
    {
        private readonly IContentApiClient contentApiClient;

        private const string RequestPath = "/Exhibits/Rating/";

        public UserRatingApiAccess(IContentApiClient contentApiClient)
        {
            this.contentApiClient = contentApiClient;
        }

        public async Task<UserRatingDto> GetUserRatingAsync(int idForRestApi)
        {
            var url = RequestPath + idForRestApi;
            var json = await contentApiClient.GetResponseFromUrlAsString(url);
            return JsonConvert.DeserializeObject<UserRatingDto>(json);
        }

        public async Task<int> GetPreviousUserRatingAsync(int idForRestApi)
        {
            //var url = RequestPath + "My/" + idForRestApi;
            //var json = await contentApiClient.GetResponseFromUrlAsString(url);
            return 0;
        }

        public async Task<bool> SendUserRatingAsync(int idForRestApi, int rating)
        {
            var url = RequestPath + idForRestApi + "?Rating=" + rating;
            var result = await contentApiClient.PostRequestBody(url, string.Empty, Settings.AccessToken);
            return result.StatusCode == HttpStatusCode.Created;
        }
    }
}