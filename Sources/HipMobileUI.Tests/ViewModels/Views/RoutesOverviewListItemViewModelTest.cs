using System.Collections.Generic;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using de.upb.hip.mobile.pcl.Common;
using de.upb.hip.mobile.pcl.Common.Contracts;
using HipMobileUI.ViewModels.Views;
using NSubstitute;
using NUnit.Framework;

namespace HipMobileUI.Tests.ViewModels.Views {

    [TestFixture]
    public class RoutesOverviewListItemViewModelTest {

        [Test, Category("UnitTest")]
        public void GetRouteDistanceText_FormatedText()
        {
            var sut = CreateSystemUnderTest();

            string distanceText = sut.GetRouteDistanceText (10);
            Assert.AreEqual ("10 Kilometer", distanceText);
        }

        [Test, Category("UnitTest")]
        public void GetRouteDurationText_FormatedText()
        {
            var sut = CreateSystemUnderTest();

            string distanceText = sut.GetRouteDurationText(480);
            Assert.AreEqual("8 Minuten", distanceText);
        }

        [Test, Category("UnitTest")]
        public void Creation_PropertiesFilled ()
        {
            var sut = CreateSystemUnderTest ();

            Assert.AreEqual ("Test Description", sut.RouteDescription);
            Assert.AreEqual ("10 Kilometer", sut.Distance);
            Assert.AreEqual ("80 Minuten", sut.Duration);
            Assert.AreEqual ("Test Title", sut.RouteTitle);
            Assert.NotNull (sut.Image);
        }

        #region HelperMethods

        private RoutesOverviewListItemViewModel CreateSystemUnderTest ()
        {
            var imageDimensions = Substitute.For<IImageDimension>();
            IoCManager.RegisterInstance(typeof(IImageDimension), imageDimensions);

            var route = Substitute.For<Route> ();

            var image = Substitute.For<Image> ();
            image.Data = new byte[] {1, 2, 3, 4};


            route.Description = "Test Description";
            route.Distance = 10;
            route.Duration = 4800;
            route.Title = "Test Title";
            route.Image = image;

            return new RoutesOverviewListItemViewModel (route);
        }

        #endregion
    }
}