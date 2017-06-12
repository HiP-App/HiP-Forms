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
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer
{
    public class ContentApiClient : IContentApiClient
    {
        private const int MaxRetryCount = 5;

        /// <summary>
        /// Returns json string if get was successful (Status 200)
        /// Returns null if the requested url returns not modified (Status 304)
        /// Throws a <see cref="NetworkAccessFailedException"/> if the server is not reachable
        /// Throws an <see cref="ArgumentException"/> if there is an unexpected response code
        /// Throws a <see cref="NotFoundException"/> if the requestes url was not found (Status 404)
        /// </summary>
        /// <param name="urlPath">Http request url path</param>
        /// <returns>Json result of the requested url</returns>
        public async Task<string> GetResponseFromUrlAsString(string urlPath)
        {
            var response = await GetHttpWebResponse(urlPath);
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                return reader.ReadToEnd();
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
        /// <returns>Byte array result of the requested url</returns>
        public async Task<byte[]> GetResponseFromUrlAsBytes(string urlPath)
        {
            var response = await GetHttpWebResponse (urlPath);
            using (Stream responseStream = response.GetResponseStream ())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    responseStream.CopyTo(ms);
                    return ms.ToArray();
                }
            }
        }

        private async Task<HttpWebResponse> GetHttpWebResponse(string urlPath)
        {
            string fullUrl = ServerEndpoints.DatastoreApiPath + urlPath;
            Exception innerException = null;

            for (int i = 0; i < MaxRetryCount; i++)
            {
                if (i != 0)
                {
                    await Task.Delay(300);
                }

                try
                {
                    var request = (HttpWebRequest)WebRequest.Create(fullUrl);
                    var response = (HttpWebResponse)await request.GetResponseAsync();

                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            return response;
                        case HttpStatusCode.NotModified:
                            return null;
                        default:
                            throw new WebException($"Unexpected response status: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    innerException = ex;
                }
            }

            WebException webException = innerException as WebException;
            if (webException != null)
            {
                WebResponse errorResponse = webException.Response;
                var httpResponse = errorResponse as HttpWebResponse;
                if (httpResponse != null && httpResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new NotFoundException(fullUrl);
                }

                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    string exceptionMessage = reader.ReadToEnd();
                    throw new NetworkAccessFailedException(exceptionMessage, webException);
                }
            }
            throw new ArgumentException("Unexpected error during fetching data");
        }
        public async Task<HttpResponseMessage> PostRequestFormBased (string url, FormUrlEncodedContent content)
        {
        try
            {
            using (HttpClient client = new HttpClient ())
                {
                var result = await TransientRetry.Do (() => client.PostAsync (url, content), new TimeSpan (0, 0, 0, 3));
                return result;
                }
            }
            catch (Exception ex)
            {
            throw new Exception(ex.Message);
            }


        }
    }
}