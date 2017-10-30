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

using System.Net.Http;
using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer
{
    /// <summary>
    /// Get response for the given path path of the content api server
    /// </summary>
    public interface IContentApiClient
    {
        /// <summary>
        /// Get result for the given path on the content api server as string
        /// </summary>
        /// <param name="urlPath"></param>
        /// <returns>String result of the api call</returns>
        Task<string> GetResponseFromUrlAsString(string urlPath);

        /// <summary>
        /// Get result for the given path on the content api server as byte array
        /// </summary>
        /// <param name="urlPath"></param>
        /// <returns>String result of the api call</returns>
        Task<byte[]> GetResponseFromUrlAsBytes(string urlPath);

        Task<HttpResponseMessage> PostRequestFormBased(string url, FormUrlEncodedContent content);
    }
}