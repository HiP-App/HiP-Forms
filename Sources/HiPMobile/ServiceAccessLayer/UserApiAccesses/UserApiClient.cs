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

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.UserApiAccesses
{
    public class UserApiClient
    {
        private const int MaxRetryCount = 5;


        private readonly string basePath;

        /// <param name="basePath">e.g. ServerEndpoints.DatastoreApiPath</param>
        public UserApiClient(string basePath = ServerEndpoints.RegisterUrl)
        {
            this.basePath = basePath;
        }

        public async Task<string> GetResponseFromUrlAsString(string urlPath, string accessToken)
        {
            try
            {
                var response = await GetHttpWebResponse(urlPath, accessToken);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        if (responseStream != null)
                        {
                            var reader = new StreamReader(responseStream, Encoding.UTF8);
                            return reader.ReadToEnd();
                        }
                    }
                }

                return null;
            }
            catch (NetworkAccessFailedException e)
            {
                if ((((WebException)e.InnerException).Response as HttpWebResponse)?.StatusCode == HttpStatusCode.Unauthorized)
                {

                    //Error Message
                    return null;

                }
                else
                {
                    throw e;
                }
            }
        }

        /// <summary>
        /// Returns response as byte array if get was successful (Status 200)
        /// Returns null if the requested url returns not modified (Status 304)
        /// Throws a <see cref="NetworkAccessFailedException"/> if the server is not reachable
        /// Throws an <see cref="ArgumentException"/> if there is an unexpected response code
        /// Throws a <see cref="NotFoundException"/> if the requestes url was not found (Status 404)
        /// </summary>
        /// <param name="urlPath">Http request url path</param>
        /// /// <param name="accessToken">Token for the user</param>
        /// <returns>Byte array result of the requested url</returns>
        public async Task<byte[]> GetResponseFromUrlAsBytes(string urlPath, string accessToken)
        {
            try
            {
                var response = await GetHttpWebResponse(urlPath, accessToken);
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        using (var ms = new MemoryStream())
                        {
                            if (responseStream != null)
                            {
                                responseStream.CopyTo(ms);
                                return ms.ToArray();
                            }
                        }
                    }
                }

                return null;
            }
            catch (NetworkAccessFailedException e)
            {
                if ((((WebException)e.InnerException).Response as HttpWebResponse)?.StatusCode == HttpStatusCode.Unauthorized)
                {

                    return null;

                }
                else
                {
                    throw e;
                }
            }
        }

        public async Task<HttpWebResponse> GetHttpWebResponse(string urlPath, string accessToken)
        {
            var fullUrl = basePath + urlPath;
            Exception innerException = null;

            for (var i = 0; i < MaxRetryCount; i++)
            {
                if (i != 0)
                {
                    await Task.Delay(300);
                }

                try
                {
                    var request = WebRequest.Create(fullUrl) as HttpWebRequest;
                    request.Headers["Authorization"] = "Bearer " + accessToken;
                    request.Accept = "application/json";
                    var response = (HttpWebResponse)await request.GetResponseAsync();

                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            return response;
                        case HttpStatusCode.NotModified:
                            return response;
                        default:
                            throw new WebException($"Unexpected response status: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    innerException = ex;
                }
            }

            var webException = innerException as WebException;
            if (webException != null)
            {
                var errorResponse = webException.Response;
                var httpResponse = errorResponse as HttpWebResponse;
                if (httpResponse != null && httpResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    return httpResponse;
                }

                using (var responseStream = errorResponse.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        var reader = new StreamReader(responseStream, Encoding.UTF8);
                        var exceptionMessage = reader.ReadToEnd();
                        throw new NetworkAccessFailedException(exceptionMessage, webException);
                    }
                }
            }
            throw new ArgumentException("Unexpected error during fetching data");
        }

    }
}