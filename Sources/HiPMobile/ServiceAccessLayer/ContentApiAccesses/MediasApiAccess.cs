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

using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiDtos;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses {
    public class MediasApiAccess : IMediasApiAccess {

        private readonly IContentApiAccess contentApiAccess;

        public MediasApiAccess (IContentApiAccess contentApiAccess)
        {
            this.contentApiAccess = contentApiAccess;
        }

        public async Task<MediasDto> GetMedias ()
        {
            return await GetMediasDto(null, null);
        }

        public async Task<MediasDto> GetMedias (long timestamp)
        {
            return await GetMediasDto(timestamp, null);
        }

        public async Task<MediasDto> GetMedias (IList<int> includeOnly)
        {
            return await GetMediasDto(null, includeOnly);
        }

        public async Task<MediasDto> GetMedias (long timestamp, IList<int> includeOnly)
        {
            return await GetMediasDto (timestamp, includeOnly);
        }

        private async Task<MediasDto> GetMediasDto(long? timestamp, IList<int> includeOnly)
        {
            string requestPath = @"/Media";
            requestPath += UriQueryBuilder.GetAdditionalParametersQuery(timestamp, includeOnly);

            string json = await contentApiAccess.GetResponseFromUrl(requestPath);
            if (json != null)
            {
                return JsonConvert.DeserializeObject<MediasDto>(json);
            }

            return new MediasDto { Items = new List<MediaDto>(), Total = 0 };
        }

        public async Task<IList<int>> GetIds()
        {
            string requestPath = @"/Media/ids";
            string json = await contentApiAccess.GetResponseFromUrl(requestPath);

            return JsonConvert.DeserializeObject<IList<int>>(json);
        }

    }
}