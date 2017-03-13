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

using System.Collections.Generic;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using de.upb.hip.mobile.pcl.Common;
using de.upb.hip.mobile.pcl.Common.Contracts;
using de.upb.hip.mobile.pcl.DataAccessLayer;
using HipMobileUI.Location;
using HipMobileUI.Navigation;
using HipMobileUI.Viewmodels.Pages;
using HipMobileUI.ViewModels.Views;
using NSubstitute;
using NUnit.Framework;

namespace HipMobileUI.Tests.ViewModels.Pages {

    [TestFixture]
    public class RoutesDetailsPageViewModelTest
    {
        [Test, Category("UnitTest")]
        public void Creation_PropertiesFilled ()
        {
            var sut = CreateSystemUnderTest ();

            Assert.AreEqual ("Test Title", sut.Title);
            Assert.AreEqual ("Test Description", sut.Description);
            Assert.AreEqual ("10 km", sut.Distance);
            Assert.AreEqual ("60 min", sut.Duration);
            Assert.AreEqual (sut.Tabs.Count, 2);

            Assert.NotNull (sut.Image);
            Assert.NotNull (sut.StartRouteCommand);
            Assert.NotNull (sut.StartDescriptionPlaybackCommand);
        }
        
        #region HelperMethods

        private RouteDetailsPageViewModel CreateSystemUnderTest ()
        {
            var imageDimensions = Substitute.For<IImageDimension>();
            IoCManager.RegisterInstance(typeof(IImageDimension), imageDimensions);
            IoCManager.RegisterInstance (typeof(ILocationManager), Substitute.For<ILocationManager>());

            var route = Substitute.For<Route>();

            var image = Substitute.For<Image>();
            image.Data = new byte[] { 1, 2, 3, 4 };

            route.Description = "Test Description";
            route.Distance = 10;
            route.Duration = 3600;
            route.Title = "Test Title";
            route.Image = image;

            var dataAccess = Substitute.For<IDataAccess> ();
            dataAccess.GetItems<Route> ().Returns (new List<Route> {route});
            IoCManager.RegisterInstance (typeof (IDataAccess), dataAccess);
            IoCManager.RegisterInstance (typeof (INavigationService), Substitute.For<INavigationService> ());

            return new RouteDetailsPageViewModel (route);
        }

        #endregion
    }
}