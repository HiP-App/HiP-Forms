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

using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiDtos;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses {
    public class RoutesApiAccess : IRoutesApiAccess
    {

        private readonly IContentApiAccess contentApiAccess;

        public RoutesApiAccess(IContentApiAccess contentApiAccess)
        {
            this.contentApiAccess = contentApiAccess;
        }
       
        private async Task<RoutesDto> GetRoutesDto (long? timestamp, IList<int> includeOnly)
        {
            string requestPath = @"/Routes";
            requestPath += UriQueryBuilder.GetAdditionalParametersQuery (timestamp, includeOnly);

            string json = await contentApiAccess.GetResponseFromUrl(requestPath);
            return JsonConvert.DeserializeObject<RoutesDto>(json);
        }

        public async Task<RoutesDto> GetRoutes ()
        {
            return await GetRoutesDto (null, null);
        }

        public async Task<RoutesDto> GetRoutes (long timestamp)
        {
            return await GetRoutesDto(timestamp, null);
        }

        public async Task<RoutesDto> GetRoutes (IList<int> includeOnly)
        {
            return await GetRoutesDto(null, includeOnly);
        }

        public async Task<RoutesDto> GetRoutes (long timestamp, IList<int> includeOnly)
        {
            return await GetRoutesDto(timestamp, includeOnly);
        }

    }
}