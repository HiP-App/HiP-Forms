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


using NSubstitute;
using NUnit.Framework;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses;

namespace PaderbornUniversity.SILab.Hip.Mobile.HipMobileTests.ServiceAccessLayer.ContentApiAccesses
{
    [TestFixture]
    class UserRatingApiAccessTest
    {
        private readonly IContentApiClient contentApiSubstitute;
        private readonly UserRatingApiAccess client;

        public UserRatingApiAccessTest()
        {
            contentApiSubstitute = Substitute.For<IContentApiClient>();
            client = new UserRatingApiAccess(contentApiSubstitute);
        }

        [Test, Category("UnitTest")]
        public async void GetUserRating()
        {
            var json = "{" +
                       "\"id\": 1," +
                       "\"average\": 3.5," +
                       "\"count\": 24," +
                       "\"ratingTable\": {" +
                       "\"1\": 2," +
                       "\"2\": 4," +
                       "\"3\": 6," +
                       "\"4\": 4," +
                       "\"5\": 8" +
                       "}" +
                       "}";
            contentApiSubstitute.GetResponseFromUrlAsString(null).ReturnsForAnyArgs(json);

            var userRatingDto = await client.GetUserRatingAsync(0);
            Assert.NotNull(userRatingDto);
            Assert.AreEqual(userRatingDto.Id, 1);
            Assert.AreEqual(userRatingDto.Average, 3.5);
            Assert.AreEqual(userRatingDto.Count, 24);
            Assert.NotNull(userRatingDto.RatingTable);
            Assert.AreEqual(userRatingDto.RatingTable[1], 2);
            Assert.AreEqual(userRatingDto.RatingTable[2], 4);
            Assert.AreEqual(userRatingDto.RatingTable[3], 6);
            Assert.AreEqual(userRatingDto.RatingTable[4], 4);
            Assert.AreEqual(userRatingDto.RatingTable[5], 8);
        }
    }
}
