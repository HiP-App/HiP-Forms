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

using System.Linq;
using NSubstitute;
using NUnit.Framework;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiDtos;

namespace PaderbornUniversity.SILab.Hip.Mobile.HipMobileTests.ServiceAccessLayer.ContentApiAccesses
{
    [TestFixture]
    public class AchievementsApiAccessTest
    {
        private readonly IContentApiClient contentApiSubstitute;
        private readonly AchievementsApiAccess client;

        public AchievementsApiAccessTest()
        {
            contentApiSubstitute = Substitute.For<IContentApiClient>();
            client = new AchievementsApiAccess(contentApiSubstitute);
        }

        [Test, Category("UnitTest")]
        public async void GetAchievements()
        {
            var json = "{\"total\":2," +
                       "\"items\":[" +
                       "{\"routeId\":0,\"id\":0,\"type\":\"RouteFinished\",\"status\":\"PUBLISHED\"," +
                       "\"title\":\"Finish Karls Route\",\"description\":" +
                       "\"Visit all exhibits on this route to unlock this achievement\"," +
                       "\"thumbnailUrl\": \"achievements/api/image/0/\"," +
                       "\"nextId\":0,\"points\":1," +
                       "\"userId\":\"auth0|5968ed8cdd1b3733ca94865d\",\"timestamp\":\"2017-10-23T13:28:43.6293372+00:00\"}," +
                       "{\"count\":10,\"id\":1,\"type\":\"ExhibitsVisited\",\"status\":\"PUBLISHED\"," +
                       "\"title\":\"Visit 10 exhibits\",\"description\":" +
                       "\"Visit 10 exhibits to unlock this achievement\"," +
                       "\"thumbnailUrl\": \"achievements/api/image/0/\"," +
                       "\"nextId\":0,\"points\":1," +
                       "\"userId\":\"auth0|5968ed8cdd1b3733ca94865d\",\"timestamp\":\"2017-10-23T13:27:55.427995+00:00\"}]}";
            contentApiSubstitute.GetResponseFromUrlAsString(null).ReturnsForAnyArgs(json);
            
            var achievementDtos = (await client.GetUnlockedAchievements()).ToList();
            var routeFinished = achievementDtos[0] as RouteFinishedAchievementDto;
            var exhibitsVisited = achievementDtos[1] as ExhibitsVisitedAchievementDto;

            Assert.NotNull(routeFinished);
            Assert.AreEqual(routeFinished.Id, 0);
            Assert.AreEqual(routeFinished.Title, "Finish Karls Route");
            Assert.AreEqual(routeFinished.Description, "Visit all exhibits on this route to unlock this achievement");
            Assert.AreEqual(routeFinished.ThumbnailUrl, "achievements/api/image/0/");
            Assert.AreEqual(routeFinished.NextId, 0);        
            
            Assert.NotNull(exhibitsVisited);
            Assert.AreEqual(exhibitsVisited.Id, 1);
            Assert.AreEqual(exhibitsVisited.Title, "Visit 10 exhibits");
            Assert.AreEqual(exhibitsVisited.Description, "Visit 10 exhibits to unlock this achievement");
            Assert.AreEqual(exhibitsVisited.ThumbnailUrl, "achievements/api/image/0/");
            Assert.AreEqual(exhibitsVisited.NextId, 0);
            Assert.AreEqual(exhibitsVisited.Count, 10);
        }
    }
}