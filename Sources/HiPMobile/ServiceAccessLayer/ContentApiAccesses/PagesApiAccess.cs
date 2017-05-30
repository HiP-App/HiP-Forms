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

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses {
    public class PagesApiAccess : IPagesApiAccess
    {
        private readonly IContentApiClient contentApiClient;

        public PagesApiAccess(IContentApiClient contentApiClient)
        {
            this.contentApiClient = contentApiClient;
        }

        private async Task<PagesDto> GetPagesDtoWithExhibitConstraint(int exhibitId, DateTimeOffset? timestamp, IList<int> includeOnly)
        {
            string requestPath = $@"/Exhibits/{exhibitId}/Pages";
            requestPath += UriQueryBuilder.GetAdditionalParametersQuery(timestamp, includeOnly);

            string json = await contentApiClient.GetResponseFromUrl(requestPath);
            if (json != null)
            {
                return JsonConvert.DeserializeObject<PagesDto>(json);
            }

            return new PagesDto { Items = new List<PageDto>(), Total = 0 };
        }

        private async Task<PagesDto> GetPagesDto (DateTimeOffset? timestamp, IList<int> includeOnly)
        {
            string requestPath = @"/Exhibits/Pages";
            requestPath += UriQueryBuilder.GetAdditionalParametersQuery (timestamp, includeOnly);

            string json = await contentApiClient.GetResponseFromUrl(requestPath);
            if (json != null)
            {
                return JsonConvert.DeserializeObject<PagesDto>(json);
            }

            return new PagesDto { Items = new List<PageDto>(), Total = 0 };
        }

        public async Task<PagesDto> GetPages ()
        {
            return await GetPagesDto (null, null);
        }

        public async Task<PagesDto> GetPages (DateTimeOffset timestamp)
        {
            return await GetPagesDto(timestamp, null);
        }

        public async Task<PagesDto> GetPages (IList<int> includeOnly)
        {
            return await GetPagesDto(null, includeOnly);
        }

        public async Task<PagesDto> GetPages (DateTimeOffset timestamp, IList<int> includeOnly)
        {
            return await GetPagesDto(timestamp, includeOnly);
        }

        public async Task<PagesDto> GetPages (int exhibitId)
        {
            return await GetPagesDtoWithExhibitConstraint(exhibitId, null, null);
        }

        public async Task<PagesDto> GetPages (int exhibitId, DateTimeOffset timestamp)
        {
            return await GetPagesDtoWithExhibitConstraint(exhibitId, timestamp, null);
        }

        public async Task<PagesDto> GetPages (int exhibitId, IList<int> includeOnly)
        {
            return await GetPagesDtoWithExhibitConstraint(exhibitId, null, includeOnly);
        }

        public async Task<PagesDto> GetPages (int exhibitId, DateTimeOffset timestamp, IList<int> includeOnly)
        {
            return await GetPagesDtoWithExhibitConstraint(exhibitId, timestamp, includeOnly);
        }

        public async Task<IList<int>> GetIds()
        {
            string requestPath = @"/Exhibits/Pages/ids";
            string json = await contentApiClient.GetResponseFromUrl(requestPath);

            return JsonConvert.DeserializeObject<IList<int>>(json);
        }
    }
}