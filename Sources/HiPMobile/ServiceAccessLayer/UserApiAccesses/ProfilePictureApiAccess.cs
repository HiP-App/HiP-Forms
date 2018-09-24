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
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;


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

        public async Task<HttpResponseMessage> PostProfilePicture(byte[] picture, string userId, string accessToken)
        {
            var path = "https://docker-hip.cs.uni-paderborn.de/public/userstore/api";
            var requestPath = $@"/Users/{userId}/Photo";
            var completePath = path + requestPath;

            var imageStream = new MemoryStream(picture, 0, picture.Length);

            HttpContent fileStreamContent = new StreamContent(imageStream);

            fileStreamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
            fileStreamContent.Headers.ContentDisposition.Name = "file";

            fileStreamContent.Headers.ContentDisposition.FileName = $"{userId}_profilepicture";
            fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var formData = new MultipartFormDataContent();
            formData.Add(fileStreamContent);

            var response = await client.PutAsync(completePath, formData);
            return response;
        }

        public async Task<HttpResponseMessage> PostPredProfilePicture(string id, string userId, string accessToken)
        {
            var path = "https://docker-hip.cs.uni-paderborn.de/public/userstore/api";
            var requestPath = $@"/Users/{userId}/Avatar?avatarId={id}";
            var completePath = path + requestPath;

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            HttpContent con = new StringContent(id, Encoding.UTF8, "application/json");
            con.Headers.ContentDisposition = new ContentDispositionHeaderValue("query");
            con.Headers.ContentDisposition.Name = "string";
            con.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            //var content = new StringContent(id.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync(completePath, con);


            return response;
        }

        public async Task<PredProfilePicture[]> GetPredefinedProfilePictures(string accessToken)
        {
            var path = "https://docker-hip.cs.uni-paderborn.de/public/userstore/api";
            var requestPath = "/PredefinedAvatars";
            var completePath = path + requestPath;
            var response = await userApiClient.GetResponseFromUrlAsString(requestPath, accessToken);
            var idArray = ParsePredefinedProfilePictures(response);
            var predProfilePictures = new PredProfilePicture[idArray.Length];
            for (var i = 0; i < idArray.Length; i++)
            {
                requestPath = $@"/PredefinedAvatars/{idArray[i]}";
                completePath = path + requestPath;
                var pictureBytes = await userApiClient.GetResponseFromUrlAsBytes(requestPath, accessToken);
                if (pictureBytes != null)
                {
                    predProfilePictures[i] = new PredProfilePicture(idArray[i], pictureBytes);
                }
            }

            return predProfilePictures;
        }

        public static string[] ParsePredefinedProfilePictures(string json)
        {

            var predProfilePictureJsons = JArray.Parse(json);
            var length = predProfilePictureJsons.Count;
            var idArray = new string[length];

            for (var i = 0; i < length; i++)
            {
                idArray[i] = predProfilePictureJsons[i].ToString();
            }

            return idArray;
        }
    }
}