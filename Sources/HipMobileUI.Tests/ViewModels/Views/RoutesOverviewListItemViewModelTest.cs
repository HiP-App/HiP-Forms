// Copyright (C) 2016 History in Paderborn App - Universität Paderborn
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
// limitations under the License.using System

using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using NSubstitute;
using NUnit.Framework;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;

namespace PaderbornUniversity.SILab.Hip.Mobile.HipMobileUITests.ViewModels.Views
{
    [TestFixture]
    public class RoutesOverviewListItemViewModelTest
    {
        [TestFixtureSetUp]
        public void Init ()
        {
            IoCManager.Clear ();
            IoCManager.RegisterInstance (typeof (IMediaFileManager), Substitute.For<IMediaFileManager> ());
            IoCManager.RegisterInstance (typeof (INavigationService), Substitute.For<INavigationService> ());
        }

        [Test, Category("UnitTest")]
        public void GetRouteDistanceText_FormatedText()
        {
            var sut = CreateSystemUnderTest();

            string distanceText = sut.GetRouteDistanceText(10);
            Assert.AreEqual("10 Kilometer", distanceText);
        }

        [Test, Category("UnitTest")]
        public void GetRouteDurationText_FormatedText()
        {
            var sut = CreateSystemUnderTest();

            string distanceText = sut.GetRouteDurationText(480);
            Assert.AreEqual("8 Minuten", distanceText);
        }

        [Test, Category("UnitTest")]
        public void Creation_PropertiesFilled()
        {
            var sut = CreateSystemUnderTest();

            Assert.AreEqual("Test Description", sut.RouteDescription);
            Assert.AreEqual("10 Kilometer", sut.Distance);
            Assert.AreEqual("80 Minuten", sut.Duration);
            Assert.AreEqual("Test Title", sut.RouteTitle);
            Assert.NotNull(sut.Image);
        }

        #region HelperMethods

        private RoutesOverviewListItemViewModel CreateSystemUnderTest()
        {
            var imageDimensions = Substitute.For<IImageDimension>();
            IoCManager.RegisterInstance(typeof(IImageDimension), imageDimensions);

            var route = Substitute.For<Route>();

            var image = Substitute.For<Image>();
            image.GetDataAsync().ReturnsForAnyArgs(Task.FromResult(new byte[] { 1, 2, 3, 4 }));
            image.GetDataBlocking().ReturnsForAnyArgs(new byte[] { 1, 2, 3, 4 });

            route.Description = "Test Description";
            route.Distance = 10;
            route.Duration = 4800;
            route.Title = "Test Title";
            route.Image = image;

            return new RoutesOverviewListItemViewModel(route);
        }

        #endregion
    }
}