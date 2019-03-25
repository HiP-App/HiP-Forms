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
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.FeatureToggleApiDto;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.FeatureToggleApiAccess
{
    public class FeatureToggleApiAccess : IFeatureToggleApiAccess
    {
        private readonly IContentApiClient contentApiClient;

        public FeatureToggleApiAccess(IContentApiClient contentApiClient)
        {
            this.contentApiClient = contentApiClient;
        }

        public async Task<IList<FeatureDto>> GetEnabledFeaturesAsync()
        {
            /*const string requestPath = "/Features/IsEnabled";
            var json = await contentApiClient.GetResponseFromUrlAsString(requestPath);
            return JsonConvert.DeserializeObject<IList<FeatureDto>>(json);*/
            return null;
        }
    }
}