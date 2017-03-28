// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//       http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections.Generic;
using System.Linq;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using NSubstitute;
using NUnit.Framework;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;

namespace PaderbornUniversity.SILab.Hip.Mobile.HipMobileUITests.ViewModels.Views {

    [TestFixture]
    public class RoutesOverviewViewModelTest
    {
        [Test, Category("UnitTest")]
        public void Creation_PropertiesFilled ()
        {
            var sut = CreateSystemUnderTest ();

            var route = sut.Routes.First ();

            Assert.AreEqual ("Test Description", route.RouteDescription);
            Assert.AreEqual ("10 Kilometer", route.Distance);
            Assert.AreEqual ("80 Minuten", route.Duration);
            Assert.AreEqual ("Test Title", route.RouteTitle);
            Assert.NotNull (route.Image);
        }

        #region HelperMethods

        private RoutesOverviewViewModel CreateSystemUnderTest ()
        {
            IoCManager.Clear ();
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