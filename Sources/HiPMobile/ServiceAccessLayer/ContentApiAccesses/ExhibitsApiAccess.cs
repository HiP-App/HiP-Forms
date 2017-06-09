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
using System.Threading.Tasks;
using Newtonsoft.Json;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiDtos;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses
{
    public class ExhibitsApiAccess : IExhibitsApiAccess
    {

        private readonly IContentApiClient contentApiClient;

        public ExhibitsApiAccess(IContentApiClient contentApiClient)
        {
            this.contentApiClient = contentApiClient;
        }

        public async Task<ExhibitsDto> GetExhibits()
        {
            return await GetExhibitsDto(null, null);
        }

        public async Task<ExhibitsDto> GetExhibits(DateTimeOffset timestamp)
        {
            return await GetExhibits(timestamp, null);
        }

        public async Task<ExhibitsDto> GetExhibits(IList<int> includeOnly)
        {
            return await GetExhibitsDto(null, includeOnly);
        }

        public async Task<ExhibitsDto> GetExhibits(DateTimeOffset timestamp, IList<int> includeOnly)
        {
            return await GetExhibitsDto(timestamp, includeOnly);
        }

        private async Task<ExhibitsDto> GetExhibitsDto(DateTimeOffset? timestamp, IList<int> includeOnly)
        {
            string requestPath = @"/Exhibits";
            requestPath += UriQueryBuilder.GetAdditionalParametersQuery(timestamp, includeOnly);

            string json = await contentApiClient.GetResponseFromUrlAsString(requestPath);
            if (json != null)
            {
                return JsonConvert.DeserializeObject<ExhibitsDto>(json);
            }

            return new ExhibitsDto { Items = new List<ExhibitDto>(), Total = 0 };
        }

        public async Task<IList<int>> GetIds ()
        {
            string requestPath = @"/Exhibits/ids";
            string json = await contentApiClient.GetResponseFromUrlAsString(requestPath);

            return JsonConvert.DeserializeObject<IList<int>> (json);
        }

    }
}