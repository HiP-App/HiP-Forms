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
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses;
using System;
using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.Mobile.HipMobileTests.ServiceAccessLayer.ContentApiAccesses
{
    [TestFixture]
    public class TagsApiAccessTest
    {
        private IContentApiClient contentApiSubstitute;

        [Test, Category("UnitTest")]
        public async Task GetTags_SingleTag()
        {
            var sut = CreateSystemUnderTest();
            string jsonReturn = "{" +
                                "total: 1," +
                                "items: [" +
                                "{" +
                                "\"id\": 1," +
                                "\"title\": \"Test Title\"," +
                                "\"description\": \"Test Description\"," +
                                "\"image\": 42," +
                                "\"status\": \"Test Status\"," +
                                "\"timestamp\": \"2017-05-29T10:10:10.10+00:00\"," +
                                "\"used\": true" +
                                "}" +
                                "]" +
                                "}";
            contentApiSubstitute.GetResponseFromUrlAsString(null).ReturnsForAnyArgs(jsonReturn);

            var tags = await sut.GetTags();
            Assert.AreEqual(1, tags.Total);
            Assert.AreEqual(1, tags.Items.Count);

            var firstTag = tags.Items[0];
            Assert.AreEqual(1, firstTag.Id);
            Assert.AreEqual("Test Title", firstTag.Title);
            Assert.AreEqual("Test Description", firstTag.Description);
            Assert.AreEqual(42, firstTag.Image);

            Assert.AreEqual("Test Status", firstTag.Status);
            Shared.Helpers.AssertionHelper.AreEqual(new DateTimeOffset(2017, 5, 29, 10, 10, 10, 10, TimeSpan.Zero), firstTag.Timestamp);
        }

        [Test, Category("UnitTest")]
        public async Task GetTags_MultipleTags()
        {
            var sut = CreateSystemUnderTest();
            string jsonReturn = "{" +
                                "total: 2," +
                                "items: [" +
                                "{" +
                                "\"id\": 1," +
                                "\"title\": \"Test Title\"," +
                                "\"description\": \"Test Description\"," +
                                "\"image\": 42," +
                                "\"status\": \"Test Status\"," +
                                "\"timestamp\": \"2017-05-29T10:10:10.10+00:00\"," +
                                "\"used\": true" +
                                "}," +
                                "{" +
                                "\"id\": 2," +
                                "\"title\": \"Test Title2\"," +
                                "\"description\": \"Test Description2\"," +
                                "\"image\": 43," +
                                "\"status\": \"Test Status2\"," +
                                "\"timestamp\": \"2017-05-29T10:10:10.20+00:00\"," +
                                "\"used\": false" +
                                "}" +
                                "]" +
                                "}";
            contentApiSubstitute.GetResponseFromUrlAsString(null).ReturnsForAnyArgs(jsonReturn);

            var tags = await sut.GetTags();
            Assert.AreEqual(2, tags.Total);
            Assert.AreEqual(2, tags.Items.Count);

            var firstTag = tags.Items[0];
            Assert.AreEqual(1, firstTag.Id);
            Assert.AreEqual("Test Title", firstTag.Title);
            Assert.AreEqual("Test Description", firstTag.Description);
            Assert.AreEqual(42, firstTag.Image);

            Assert.AreEqual("Test Status", firstTag.Status);
            Shared.Helpers.AssertionHelper.AreEqual(new DateTimeOffset(2017, 5, 29, 10, 10, 10, 10, TimeSpan.Zero), firstTag.Timestamp);

            var secondTag = tags.Items[1];
            Assert.AreEqual(2, secondTag.Id);
            Assert.AreEqual("Test Title2", secondTag.Title);
            Assert.AreEqual("Test Description2", secondTag.Description);
            Assert.AreEqual(43, secondTag.Image);

            Assert.AreEqual("Test Status2", secondTag.Status);
            Shared.Helpers.AssertionHelper.AreEqual(new DateTimeOffset(2017, 5, 29, 10, 10, 10, 20, TimeSpan.Zero), secondTag.Timestamp);
        }

        #region HelperMethods

        private TagsApiAccess CreateSystemUnderTest()
        {
            contentApiSubstitute = Substitute.For<IContentApiClient>();

            return new TagsApiAccess(contentApiSubstitute);
        }

        #endregion
    }
}