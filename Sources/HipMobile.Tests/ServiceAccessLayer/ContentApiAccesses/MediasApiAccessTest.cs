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

using NSubstitute;
using NUnit.Framework;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses;
using System;
using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.Mobile.HipMobileTests.ServiceAccessLayer.ContentApiAccesses
{
    [TestFixture]
    public class MediasApiAccessTest
    {
        private IContentApiClient contentApiSubstitute;

        [Test, Category("UnitTest")]
        public async Task GetMedias_SingleMedia()
        {
            var sut = CreateSystemUnderTest();
            string jsonReturn = "{" +
                                "total: 1," +
                                "items: [" +
                                "{" +
                                "\"id\": 1, " +
                                "\"title\": \"Test Title\"," +
                                "\"description\": \"Test Description\"," +
                                "\"type\": \"image\"," +
                                "\"timestamp\": \"2017-05-29T10:10:10.10+00:00\"," +
                                "\"status\": \"Test Status\"," +
                                "\"used\": true" +
                                "}" +
                                "]" +
                                "}";
            contentApiSubstitute.GetResponseFromUrlAsString(null).ReturnsForAnyArgs(jsonReturn);

            var medias = await sut.GetMedias();
            Assert.AreEqual(1, medias.Total);
            Assert.AreEqual(1, medias.Items.Count);

            var firstMedia = medias.Items[0];
            Assert.AreEqual(1, firstMedia.Id);
            Assert.AreEqual("Test Title", firstMedia.Title);
            Assert.AreEqual("Test Description", firstMedia.Description);
            Assert.AreEqual("Test Status", firstMedia.Status);
            Shared.Helpers.AssertionHelper.AreEqual(new DateTimeOffset(2017, 5, 29, 10, 10, 10, 10, TimeSpan.Zero), firstMedia.Timestamp);
            Assert.AreEqual(true, firstMedia.Used);
        }

        [Test, Category("UnitTest")]
        public async Task GetMedias_MultipleMedia()
        {
            var sut = CreateSystemUnderTest();
            string jsonReturn = "{" +
                                "total: 2," +
                                "items: [" +
                                "{" +
                                "\"id\": 1, " +
                                "\"title\": \"Test Title\"," +
                                "\"description\": \"Test Description\"," +
                                "\"type\": \"image\"," +
                                "\"timestamp\": \"2017-05-29T10:10:10.10+00:00\"," +
                                "\"status\": \"Test Status\"," +
                                "\"used\": true" +
                                "}," +
                                "{" +
                                "\"id\": 2, " +
                                "\"title\": \"Test Title2\"," +
                                "\"description\": \"Test Description2\"," +
                                "\"type\": \"audio\"," +
                                "\"timestamp\": \"2017-05-29T10:10:10.20+00:00\"," +
                                "\"status\": \"Test Status2\"," +
                                "\"used\": false" +
                                "}" +
                                "]" +
                                "}";
            contentApiSubstitute.GetResponseFromUrlAsString(null).ReturnsForAnyArgs(jsonReturn);

            var medias = await sut.GetMedias();
            Assert.AreEqual(2, medias.Total);
            Assert.AreEqual(2, medias.Items.Count);

            var firstMedia = medias.Items[0];
            Assert.AreEqual(1, firstMedia.Id);
            Assert.AreEqual("Test Title", firstMedia.Title);
            Assert.AreEqual("Test Description", firstMedia.Description);
            Assert.AreEqual("Test Status", firstMedia.Status);
            Shared.Helpers.AssertionHelper.AreEqual(new DateTimeOffset(2017, 5, 29, 10, 10, 10, 10, TimeSpan.Zero), firstMedia.Timestamp);
            Assert.AreEqual(true, firstMedia.Used);

            var secondMedia = medias.Items[1];
            Assert.AreEqual(2, secondMedia.Id);
            Assert.AreEqual("Test Title2", secondMedia.Title);
            Assert.AreEqual("Test Description2", secondMedia.Description);
            Assert.AreEqual("Test Status2", secondMedia.Status);
            Shared.Helpers.AssertionHelper.AreEqual(new DateTimeOffset(2017, 5, 29, 10, 10, 10, 20, TimeSpan.Zero), secondMedia.Timestamp);
            Assert.AreEqual(false, secondMedia.Used);
        }

        #region HelperMethods

        private MediasApiAccess CreateSystemUnderTest()
        {
            contentApiSubstitute = Substitute.For<IContentApiClient>();

            return new MediasApiAccess(contentApiSubstitute);
        }

        #endregion
    }
}