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
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.FeatureToggleApiDto;

namespace PaderbornUniversity.SILab.Hip.Mobile.HipMobileTests.ServiceAccessLayer.ContentApiAccesses
{
    [TestFixture]
    public class FeatureToggleApiAccessTest
    {
        private readonly IContentApiClient contentApiSubstitute;
        private readonly FeatureToggleApiAccess client;

        public FeatureToggleApiAccessTest()
        {
            contentApiSubstitute = Substitute.For<IContentApiClient>();
            client = new FeatureToggleApiAccess(contentApiSubstitute);
        }

        [Test, Category("UnitTest")]
        public async Task GetEnabledFeaturesTest()
        {
            var json = @"
[
    {
        ""id"": 4,
        ""name"": ""Mein neues Feature"",
        ""parent"": null,
        ""children"": [],
        ""groupsWhereEnabled"": [
            1,
            2
        ]
    },
    {
        ""id"": 6,
        ""name"": ""Feature Beta"",
        ""parent"": null,
        ""children"": [],
        ""groupsWhereEnabled"": [
            1,
            2
        ]
    }
]";
            contentApiSubstitute.GetResponseFromUrlAsString(null).ReturnsForAnyArgs(json);
            var fetchedDtos = (await client.GetEnabledFeaturesAsync()).ToList();
            var checkDtos = new List<FeatureDto>
            {
                new FeatureDto(4, "Mein neues Feature", null, new List<int>(), new List<int> { 1, 2 }),
                new FeatureDto(6, "Feature Beta", null, new List<int>(), new List<int> { 1, 2 })
            };
            Assert.AreEqual(fetchedDtos.Count, checkDtos.Count);
            for (var i = 0; i < checkDtos.Count; i++)
            {
                var checkDto = checkDtos[i];
                var fetchedDto = fetchedDtos[i];
                
                Assert.AreEqual(checkDto.Id, fetchedDto.Id);
                Assert.AreEqual(checkDto.Name, fetchedDto.Name);
                Assert.AreEqual(checkDto.ChildrenFeatureIds, fetchedDto.ChildrenFeatureIds);
                Assert.AreEqual(checkDto.ParentFeatureId, fetchedDto.ParentFeatureId);
                Assert.AreEqual(checkDto.GroupIdsWhereEnabled, fetchedDto.GroupIdsWhereEnabled);
            }
        }
    }
}