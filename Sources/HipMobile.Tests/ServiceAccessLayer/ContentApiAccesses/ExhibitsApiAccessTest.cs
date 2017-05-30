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
using NSubstitute;
using NUnit.Framework;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses;

namespace PaderbornUniversity.SILab.Hip.Mobile.HipMobileTests.ServiceAccessLayer.ContentApiAccesses
{
    [TestFixture]
    public class ExhibitsApiAccessTest {

        private IContentApiClient contentApiSubstitute;

        [Test, Category("UnitTest")]
        public async void GetExhibits_SingleExhibit()
        {
            var sut = CreateSystemUnderTest();
            string jsonReturn = "{" +
                                "total: 1," +
                                "items: [" +
                                "{" +
                                "\"id\": 1," +
                                "\"name\": \"Test Name\"," +
                                "\"description\": \"Test Description\"," +
                                "\"image\": 42," +
                                "\"latitude\": 42.7," +
                                "\"longitude\": 42.8," +
                                "\"status\": \"Test Status\"," +
                                "\"tags\": [1, 3]," +
                                "\"pages\": [4, 5, 7]," +
                                "\"timestamp\": \"2017-05-29T10:10:10.10+00:00\"," +
                                "\"used\": true" +
                                "}" +
                                "]" +
                                "}";
            contentApiSubstitute.GetResponseFromUrl(null).ReturnsForAnyArgs(jsonReturn);

            var exhibits = await sut.GetExhibits();
            Assert.AreEqual(1, exhibits.Total);
            Assert.AreEqual(1, exhibits.Items.Count);

            var firstExhibit = exhibits.Items[0];
            Assert.AreEqual(1, firstExhibit.Id);
            Assert.AreEqual("Test Name", firstExhibit.Name);
            Assert.AreEqual("Test Description", firstExhibit.Description);
            Assert.AreEqual(42, firstExhibit.Image);
            Assert.AreEqual(42.7, firstExhibit.Latitude);
            Assert.AreEqual(42.8, firstExhibit.Longitude);
            Assert.AreEqual("Test Status", firstExhibit.Status);
            Shared.Helpers.AssertionHelper.AreEqual(new DateTimeOffset(2017, 5, 29, 10, 10, 10, 10, TimeSpan.Zero), firstExhibit.Timestamp);
            Assert.AreEqual(true, firstExhibit.Used);

            Assert.AreEqual(2, firstExhibit.Tags.Count);
            Assert.AreEqual(1, firstExhibit.Tags[0]);
            Assert.AreEqual(3, firstExhibit.Tags[1]);

            Assert.AreEqual(3, firstExhibit.Pages.Count);
            Assert.AreEqual(4, firstExhibit.Pages[0]);
            Assert.AreEqual(5, firstExhibit.Pages[1]);
            Assert.AreEqual(7, firstExhibit.Pages[2]);
        }

        [Test, Category("UnitTest")]
        public async void GetExhibits_MultipleExhibits()
        {
            var sut = CreateSystemUnderTest();
            string jsonReturn = "{" +
                                "total: 2," +
                                "items: [" +
                                "{" +
                                "\"id\": 1," +
                                "\"name\": \"Test Name\"," +
                                "\"description\": \"Test Description\"," +
                                "\"image\": 42," +
                                "\"latitude\": 42.7," +
                                "\"longitude\": 42.8," +
                                "\"status\": \"Test Status\"," +
                                "\"tags\": [1, 3]," +
                                "\"pages\": [4, 5, 7]," +
                                "\"timestamp\": \"2017-05-29T10:10:10.10+00:00\"," +
                                "\"used\": true" +
                                "}," +
                                "{" +
                                "\"id\": 2," +
                                "\"name\": \"Test Name2\"," +
                                "\"description\": \"Test Description2\"," +
                                "\"image\": 43," +
                                "\"latitude\": 42.8," +
                                "\"longitude\": 42.9," +
                                "\"status\": \"Test Status2\"," +
                                "\"tags\": [2, 4]," +
                                "\"pages\": [5, 6, 8]," +
                                "\"timestamp\": \"2017-05-29T10:10:10.20+00:00\"," +
                                "\"used\": false" +
                                "}" +
                                "]" +
                                "}";
            contentApiSubstitute.GetResponseFromUrl(null).ReturnsForAnyArgs(jsonReturn);

            var exhibits = await sut.GetExhibits();
            Assert.AreEqual(2, exhibits.Total);
            Assert.AreEqual(2, exhibits.Items.Count);

            var firstExhibit = exhibits.Items[0];
            Assert.AreEqual(1, firstExhibit.Id);
            Assert.AreEqual("Test Name", firstExhibit.Name);
            Assert.AreEqual("Test Description", firstExhibit.Description);
            Assert.AreEqual(42, firstExhibit.Image);
            Assert.AreEqual(42.7, firstExhibit.Latitude);
            Assert.AreEqual(42.8, firstExhibit.Longitude);
            Assert.AreEqual("Test Status", firstExhibit.Status);
            Shared.Helpers.AssertionHelper.AreEqual(new DateTimeOffset(2017, 5, 29, 10, 10, 10, 10, TimeSpan.Zero), firstExhibit.Timestamp);
            Assert.AreEqual(true, firstExhibit.Used);

            Assert.AreEqual(2, firstExhibit.Tags.Count);
            Assert.AreEqual(1, firstExhibit.Tags[0]);
            Assert.AreEqual(3, firstExhibit.Tags[1]);

            Assert.AreEqual(3, firstExhibit.Pages.Count);
            Assert.AreEqual(4, firstExhibit.Pages[0]);
            Assert.AreEqual(5, firstExhibit.Pages[1]);
            Assert.AreEqual(7, firstExhibit.Pages[2]);

            var secondExhibit = exhibits.Items[1];
            Assert.AreEqual(2, secondExhibit.Id);
            Assert.AreEqual("Test Name2", secondExhibit.Name);
            Assert.AreEqual("Test Description2", secondExhibit.Description);
            Assert.AreEqual(43, secondExhibit.Image);
            Assert.AreEqual(42.8, secondExhibit.Latitude);
            Assert.AreEqual(42.9, secondExhibit.Longitude);
            Assert.AreEqual("Test Status2", secondExhibit.Status);
            Shared.Helpers.AssertionHelper.AreEqual(new DateTimeOffset(2017, 5, 29, 10, 10, 10, 20, TimeSpan.Zero), secondExhibit.Timestamp);
            Assert.AreEqual(false, secondExhibit.Used);

            Assert.AreEqual(2, secondExhibit.Tags.Count);
            Assert.AreEqual(2, secondExhibit.Tags[0]);
            Assert.AreEqual(4, secondExhibit.Tags[1]);

            Assert.AreEqual(3, secondExhibit.Pages.Count);
            Assert.AreEqual(5, secondExhibit.Pages[0]);
            Assert.AreEqual(6, secondExhibit.Pages[1]);
            Assert.AreEqual(8, secondExhibit.Pages[2]);
        }

        #region HelperMethods

        private ExhibitsApiAccess CreateSystemUnderTest()
        {
            contentApiSubstitute = Substitute.For<IContentApiClient>();

            return new ExhibitsApiAccess(contentApiSubstitute);
        }

        #endregion

    }
}