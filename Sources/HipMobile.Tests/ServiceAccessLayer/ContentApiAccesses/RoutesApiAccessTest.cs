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

namespace PaderbornUniversity.SILab.Hip.Mobile.HipMobileTests.ServiceAccessLayer.ContentApiAccesses
{
    [TestFixture]
    public class RoutesApiAccessTest
    {

        private IContentApiAccess contentApiSubstitute;

        [Test, Category("UnitTest")]
        public async void GetRoutes_SingleRoute()
        {
            var sut = CreateSystemUnderTest();
            string jsonReturn = "{" +
                                "total: 1," +
                                "items: [" +
                                "{" +
                                "\"id\": 1," +
                                "\"title\": \"Test Title\"," +
                                "\"description\": \"Test Description\"," +
                                "\"duration\": 1800," +
                                "\"distance\": 4.2," +
                                "\"image\": 42," +
                                "\"audio\": 103," +
                                "\"exhibits\": [ 1, 2, 3, 5, 6 ]," +
                                "\"status\": \"Test Status\"," +
                                "\"tags\": [ 2, 4 ]," +
                                "\"timestamp\": 1492029999" +
                                "}"+
                                "]" +
                                "}";
            contentApiSubstitute.GetResponseFromUrl(null).ReturnsForAnyArgs(jsonReturn);

            var routes = await sut.GetRoutes();
            Assert.AreEqual(1, routes.Total);
            Assert.AreEqual(1, routes.Items.Count);

            var firstRoute = routes.Items[0];
            Assert.AreEqual(1, firstRoute.Id);
            Assert.AreEqual("Test Title", firstRoute.Title);
            Assert.AreEqual("Test Description", firstRoute.Description);
            Assert.AreEqual(42, firstRoute.Image);
            Assert.AreEqual(1800, firstRoute.Duration);
            Assert.AreEqual(4.2, firstRoute.Distance);
            Assert.AreEqual(103, firstRoute.Audio);

            Assert.AreEqual("Test Status", firstRoute.Status);
            Assert.AreEqual(1492029999, firstRoute.Timestamp);

            Assert.AreEqual(2, firstRoute.Tags.Count);
            Assert.AreEqual(2, firstRoute.Tags[0]);
            Assert.AreEqual(4, firstRoute.Tags[1]);

            Assert.AreEqual(5, firstRoute.Exhibits.Count);
            Assert.AreEqual(1, firstRoute.Exhibits[0]);
            Assert.AreEqual(2, firstRoute.Exhibits[1]);
            Assert.AreEqual(3, firstRoute.Exhibits[2]);
            Assert.AreEqual(5, firstRoute.Exhibits[3]);
            Assert.AreEqual(6, firstRoute.Exhibits[4]);
        }

        [Test, Category("UnitTest")]
        public async void GetRoutes_MultipleRoutes()
        {
            var sut = CreateSystemUnderTest();
            string jsonReturn = "{" +
                                "total: 2," +
                                "items: [" +
                                "{" +
                                "\"id\": 1," +
                                "\"title\": \"Test Title\"," +
                                "\"description\": \"Test Description\"," +
                                "\"duration\": 1800," +
                                "\"distance\": 4.2," +
                                "\"image\": 42," +
                                "\"audio\": 103," +
                                "\"exhibits\": [ 1, 2, 3, 5, 6 ]," +
                                "\"status\": \"Test Status\"," +
                                "\"tags\": [ 2, 4 ]," +
                                "\"timestamp\": 1492029999" +
                                "}," +
                                "{" +
                                "\"id\": 2," +
                                "\"title\": \"Test Title2\"," +
                                "\"description\": \"Test Description2\"," +
                                "\"duration\": 2800," +
                                "\"distance\": 4.3," +
                                "\"image\": 43," +
                                "\"audio\": 104," +
                                "\"exhibits\": [ 2, 3, 4, 6, 7 ]," +
                                "\"status\": \"Test Status2\"," +
                                "\"tags\": [ 3, 5 ]," +
                                "\"timestamp\": 2492029999" +
                                "}" +
                                "]" +
                                "}";
            contentApiSubstitute.GetResponseFromUrl(null).ReturnsForAnyArgs(jsonReturn);

            var routes = await sut.GetRoutes();
            Assert.AreEqual(2, routes.Total);
            Assert.AreEqual(2, routes.Items.Count);

            var firstRoute = routes.Items[0];
            Assert.AreEqual(1, firstRoute.Id);
            Assert.AreEqual("Test Title", firstRoute.Title);
            Assert.AreEqual("Test Description", firstRoute.Description);
            Assert.AreEqual(42, firstRoute.Image);
            Assert.AreEqual(1800, firstRoute.Duration);
            Assert.AreEqual(4.2, firstRoute.Distance);
            Assert.AreEqual(103, firstRoute.Audio);

            Assert.AreEqual("Test Status", firstRoute.Status);
            Assert.AreEqual(1492029999, firstRoute.Timestamp);

            Assert.AreEqual(2, firstRoute.Tags.Count);
            Assert.AreEqual(2, firstRoute.Tags[0]);
            Assert.AreEqual(4, firstRoute.Tags[1]);

            Assert.AreEqual(5, firstRoute.Exhibits.Count);
            Assert.AreEqual(1, firstRoute.Exhibits[0]);
            Assert.AreEqual(2, firstRoute.Exhibits[1]);
            Assert.AreEqual(3, firstRoute.Exhibits[2]);
            Assert.AreEqual(5, firstRoute.Exhibits[3]);
            Assert.AreEqual(6, firstRoute.Exhibits[4]);

            var secondRoute = routes.Items[1];
            Assert.AreEqual(2, secondRoute.Id);
            Assert.AreEqual("Test Title2", secondRoute.Title);
            Assert.AreEqual("Test Description2", secondRoute.Description);
            Assert.AreEqual(43, secondRoute.Image);
            Assert.AreEqual(2800, secondRoute.Duration);
            Assert.AreEqual(4.3, secondRoute.Distance);
            Assert.AreEqual(104, secondRoute.Audio);

            Assert.AreEqual("Test Status2", secondRoute.Status);
            Assert.AreEqual(2492029999, secondRoute.Timestamp);

            Assert.AreEqual(2, secondRoute.Tags.Count);
            Assert.AreEqual(3, secondRoute.Tags[0]);
            Assert.AreEqual(5, secondRoute.Tags[1]);

            Assert.AreEqual(5, secondRoute.Exhibits.Count);
            Assert.AreEqual(2, secondRoute.Exhibits[0]);
            Assert.AreEqual(3, secondRoute.Exhibits[1]);
            Assert.AreEqual(4, secondRoute.Exhibits[2]);
            Assert.AreEqual(6, secondRoute.Exhibits[3]);
            Assert.AreEqual(7, secondRoute.Exhibits[4]);
        }

        #region HelperMethods

        private RoutesApiAccess CreateSystemUnderTest()
        {
            contentApiSubstitute = Substitute.For<IContentApiAccess>();

            return new RoutesApiAccess(contentApiSubstitute);
        }

        #endregion

    }
}