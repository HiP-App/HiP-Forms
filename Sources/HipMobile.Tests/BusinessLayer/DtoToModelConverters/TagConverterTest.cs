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
    public class TagConverterTest
    {

        [Test, Category ("UnitTest")]
        public void Convert_TagTest()
        {
            var sut = CreateSystemUnderTest ();

            var route = Substitute.For<RouteTag> ();
            var routeDto = CreateRouteDto ();

            sut.Convert (routeDto, route);
            Assert.AreEqual (1, route.IdForRestApi);
            Assert.AreEqual (1234, route.UnixTimestamp);
            Assert.AreEqual ("Test Description", route.Name);
            Assert.AreEqual ("Test Title", route.Tag);
        }

        #region HelperMethods

        private TagDto CreateRouteDto ()
        {
            return new TagDto
            {
                Id = 1,
                Timestamp = 1234,
                Description = "Test Description",
                Title = "Test Title"
            };
        }

        private TagConverter CreateSystemUnderTest()
        {
            return new TagConverter();
        }

        #endregion


    }
}