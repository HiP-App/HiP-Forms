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
using NUnit.Core;
using NUnit.Framework;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.DtoToModelConverters;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiDtos;

namespace PaderbornUniversity.SILab.Hip.Mobile.HipMobileTests.BusinessLayer.DtoToModelConverters {

    [TestFixture]
    public class MediaToImageConverterTest
    {
        [TestFixtureSetUp]
        public void Init()
        {
            IoCManager.RegisterInstance(typeof(IImageDimension), Substitute.For<IImageDimension>());
        }

        [Test, Category ("UnitTest")]
        public void Convert_MediaToImageTest()
        {
            var sut = CreateSystemUnderTest();

            var image = Substitute.For<Image>();
            var mediaDto = CreateMediaDto();

            sut.Convert(mediaDto, image);
            Assert.AreEqual(1, image.IdForRestApi);
            Assert.AreEqual(new DateTimeOffset(2017, 5, 29, 10, 10, 10, 10, TimeSpan.Zero), image.Timestamp);
            Assert.AreEqual("Test Description", image.Description);
            Assert.AreEqual("Test Title", image.Title);
        }

        #region HelperMethods

        private MediaDto CreateMediaDto()
        {
            return new MediaDto
            {
                Id = 1,
                Timestamp = new DateTimeOffset(2017, 5, 29, 10, 10, 10, 10, TimeSpan.Zero),
                Description = "Test Description",
                Title = "Test Title"
            };
        }

        private MediaToImageConverter CreateSystemUnderTest()
        {
            return new MediaToImageConverter();
        }

        #endregion

    }
}