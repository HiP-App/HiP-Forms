using System.Collections.Generic;
using System.Linq;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using de.upb.hip.mobile.pcl.Common;
using de.upb.hip.mobile.pcl.Common.Contracts;
using de.upb.hip.mobile.pcl.DataAccessLayer;
using HipMobileUI.Navigation;
using HipMobileUI.ViewModels.Views;
using NSubstitute;
using NUnit.Framework;

namespace HipMobileUI.Tests.ViewModels.Views {

    [TestFixture]
    public class RoutesOverviewViewModelTest
    {
        [Test, Category("UnitTest")]
        public void Creation_PropertiesFilled ()
        {
            var sut = CreateSystemUnderTest ();

            var route = sut.Routes.First ();

            Assert.AreEqual ("Test Description", route.RouteDescription);
            Assert.AreEqual ("10 km", route.Distance);
            Assert.AreEqual ("80 Minuten", route.Duration);
            Assert.AreEqual ("Test Title", route.RouteTitle);
            Assert.NotNull (route.Image);
        }

        #region HelperMethods

        private RoutesOverviewViewModel CreateSystemUnderTest ()
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

            var dataAccess = Substitute.For<IDataAccess> ();
            dataAccess.GetItems<Route> ().Returns (new List<Route> {route});
            IoCManager.RegisterInstance (typeof (IDataAccess), dataAccess);
            IoCManager.RegisterInstance (typeof (INavigationService), Substitute.For<INavigationService> ());

            return new RoutesOverviewViewModel ();
        }

        #endregion
    }
}