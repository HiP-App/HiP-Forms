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
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer {
    public class ContentApiAccess : IContentApiAccess {

        private const string ServerUrl = "https://docker-hip.cs.upb.de/";

        /// <summary>
        /// Returns json string if webcall was successful (Status 200)
        /// Returns null if the requested url returns not modified (Status 304)
        /// Throws a <see cref="NetworkAccessFailedException"/> if the server is not reachable
        /// Throws an <see cref="ArgumentException"/> if there is an unexpected response code
        /// </summary>
        /// <param name="urlPath">Http request url path</param>
        /// <returns>Json result of the requested url</returns>
        public async Task<string> GetResponseFromUrl(string urlPath)
        {
            string fullUrl = ServerUrl + urlPath;
            var request = (HttpWebRequest)WebRequest.Create(fullUrl);
            try
            {
                var response = (HttpWebResponse)await request.GetResponseAsync ();

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        using (Stream responseStream = response.GetResponseStream())
                        {
                            StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                            return reader.ReadToEnd();
                        }
                    case HttpStatusCode.NotModified:
                        return null;
                    default:
                        throw new ArgumentException($"Unexpected response status: {response.StatusCode}");
                }
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    String errorText = reader.ReadToEnd();

                    throw new NetworkAccessFailedException(errorText);
                }
            }
        }
    }
}