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
using Newtonsoft.Json;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.FeatureToggleApiDto
{
    public class FeatureDto
    {
        [JsonProperty("id")]
        public int Id { get; private set; }

        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("parent")]
        public int ParentFeatureId { get; private set; }

        [JsonProperty("children")]
        public IList<int> ChildrenFeatureIds { get; private set; }

        [JsonProperty("groupsWhereEnabled")]
        public IList<int> GroupIdsWhereEnabled { get; private set; }

        public override string ToString() => 
            $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}, {nameof(ParentFeatureId)}: {ParentFeatureId}, " +
            $"{nameof(ChildrenFeatureIds)}: {ChildrenFeatureIds}, {nameof(GroupIdsWhereEnabled)}: {GroupIdsWhereEnabled}";
    }
}