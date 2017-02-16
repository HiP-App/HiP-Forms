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
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using de.upb.hip.mobile.pcl.Common;
using de.upb.hip.mobile.pcl.Common.Contracts;
using HipMobileUI.Navigation;
using HipMobileUI.ViewModels.Views.ExhibitDetails;
using NSubstitute;
using NUnit.Framework;

namespace HipMobileUI.Tests.ViewModels.Views.ExhibitDetails
{
    class ImageViewModelTest
    {
        [TestFixtureSetUp]
        public void Init()
        {
            IoCManager.RegisterInstance(typeof(INavigationService), Substitute.For<INavigationService>());
            IoCManager.RegisterInstance(typeof(IImageDimension), Substitute.For<IImageDimension>());
        }

        [Test, Category("UnitTest")]
        public void Creation_PropertiesFilled()
        {
            var sut = CreateSystemUnderTest();

            Assert.AreEqual(sut.Headline, "Foo");
            Assert.AreEqual(sut.Description, "Bar");
        }

        #region Helper Methods

        public ImageViewModel CreateSystemUnderTest()
        {
            var imagePage = Substitute.For<ImagePage>();
            imagePage.Image = CreateImage ();

            return new ImageViewModel(imagePage);
        }

        private Image CreateImage()
        {
            var image = Substitute.For<Image>();
            image.Data = new byte[] { 1, 2, 3, 4 };
            image.Title = "Foo";
            image.Description = "Bar";
            return image;
        }
        #endregion
    }
}
