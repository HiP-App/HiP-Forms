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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.AuthenticationApiAccess;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiDtos;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.UserApiAccesses
{
    public class ProfilePictureApiAccess
    {
        private readonly UserApiClient userApiClient;

        public ProfilePictureApiAccess(UserApiClient userApiClient)
        {
            this.userApiClient = userApiClient;
        }

        public async Task<ProfilePicture> GetProfilePicture(string userId, string accessToken)
        {
            string requestPath = $@"/{userId}/Photo";
            var response = await userApiClient.GetResponseFromUrlAsBytes(requestPath, accessToken);
            if (response == null)
            {
                return null;
            }
            else
            {
                return new ProfilePicture
                {
                    Data = response,
                    UserId = userId
                };
            }

        }

        public async Task<string> PostProfilePicture(Stream picture, string userId, string accessToken)
        {
            var path = "https://docker-hip.cs.uni-paderborn.de/public/userstore/api";
            var requestPath = $@"/Users/{userId}/Photo";
            var completePath = path + requestPath;

            HttpContent fileStreamContent = new StreamContent(picture);
            //HttpContent fileStreamContent = new StreamContent(stream);

            fileStreamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
            fileStreamContent.Headers.ContentDisposition.Name = "file";
            fileStreamContent.Headers.ContentDisposition.FileName = "IMG_20180702_092034.jpg";
            fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var formData = new MultipartFormDataContent();
            formData.Add(fileStreamContent);

            var response = await client.PutAsync(completePath, formData);
            if (response != null)
            {
                var responseString = response.Content.ReadAsStringAsync().ToString();
                return responseString;
            }
            return null;
        }
    }
}