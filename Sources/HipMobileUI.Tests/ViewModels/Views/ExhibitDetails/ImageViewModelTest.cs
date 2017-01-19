using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var appetizerPage = Substitute.For<ImagePage>();
            appetizerPage.Image = CreateImage ();

            return new ImageViewModel(appetizerPage);
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
