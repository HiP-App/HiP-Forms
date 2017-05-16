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
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.DtoToModelConverters;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiDtos;

namespace PaderbornUniversity.SILab.Hip.Mobile.HipMobileTests.BusinessLayer.DtoToModelConverters {

    [TestFixture]
    public class RouteConverterTest
    {

        [Test, Category ("UnitTest")]
        public void Convert_RouteTest()
        {
            var sut = CreateSystemUnderTest ();

            var route = Substitute.For<Route> ();
            var routeDto = CreateRouteDto ();

            sut.Convert (routeDto, route);
            Assert.AreEqual (1, route.IdForRestApi);
            Assert.AreEqual (1234, route.UnixTimestamp);
            Assert.AreEqual ("Test Description", route.Description);
            Assert.AreEqual ("Test Title", route.Title);
            Assert.AreEqual (42.2, route.Distance);
            Assert.AreEqual (100, route.Duration);
        }

        #region HelperMethods

        private RouteDto CreateRouteDto ()
        {
            return new RouteDto
            {
                Id = 1,
                Timestamp = 1234,
                Description = "Test Description",
                Distance = 42.2,
                Duration = 100,
                Title = "Test Title"
            };
        }

        private RouteConverter CreateSystemUnderTest()
        {
            return new RouteConverter ();
        }

        #endregion


    }
}