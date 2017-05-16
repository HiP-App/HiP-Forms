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
    public class ExhibitConverterTest {

        //TODO Realm under Windows first [Test, Category ("UnitTest")]
        public void Convert_ExhibitTest()
        {
            var sut = CreateSystemUnderTest ();

            var exhibit = Substitute.For<Exhibit> ();
            var exhibitDto = CreateExhibitDto ();

            sut.Convert (exhibitDto, exhibit);
            Assert.AreEqual (1, exhibit.IdForRestApi);
            Assert.AreEqual (1234, exhibit.UnixTimestamp);
            Assert.AreEqual ("Test Description", exhibit.Description);
            Assert.AreEqual ("Test Name", exhibit.Name);
            Assert.AreEqual (42.1, exhibit.Location.Latitude);
            Assert.AreEqual (42.2, exhibit.Location.Longitude);
        }

        #region HelperMethods

        private ExhibitDto CreateExhibitDto ()
        {
            return new ExhibitDto
            {
                Id = 1,
                Timestamp = 1234,
                Description = "Test Description",
                Name = "Test Name",
                Latitude = 42.1,
                Longitude = 42.2
            };
        }

        private ExhibitConverter CreateSystemUnderTest()
        {
            return new ExhibitConverter ();
        }

        #endregion


    }
}