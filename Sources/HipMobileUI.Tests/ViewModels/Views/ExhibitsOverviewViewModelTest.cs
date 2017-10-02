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
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using NSubstitute;
using NUnit.Framework;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.UI.AudioPlayer;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Location;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;

namespace PaderbornUniversity.SILab.Hip.Mobile.HipMobileUITests.ViewModels.Views
{
    [TestFixture]
    class ExhibitsOverviewViewModelTest
    {
        private INavigationService navservice;

        [TestFixtureSetUp]
        public void Init()
        {
            IoCManager.Clear();
            navservice = Substitute.For<INavigationService>();
            IoCManager.RegisterInstance(typeof(INavigationService), navservice);
            IoCManager.RegisterInstance(typeof(IImageDimension), Substitute.For<IImageDimension>());
            IoCManager.RegisterInstance(typeof(IDataAccess), Substitute.For<IDataAccess>());
            IoCManager.RegisterInstance(typeof(IAudioPlayer), Substitute.For<IAudioPlayer>());
            IoCManager.RegisterInstance(typeof(ILocationManager), Substitute.For<ILocationManager>());
            IoCManager.RegisterInstance(typeof(INearbyExhibitManager), Substitute.For<INearbyExhibitManager>());
            IoCManager.RegisterInstance(typeof(INearbyRouteManager), Substitute.For<INearbyRouteManager>());
            IoCManager.RegisterInstance(typeof(IDbChangedHandler), Substitute.For<IDbChangedHandler>());
        }

        [Test, Category("UnitTest")]
        public void Creation_PropertiesFilled()
        {
            var sut = CreateSystemUnderTest();

            Assert.AreEqual(sut.ExhibitsList.Count, 3);
            Assert.AreNotSame(sut.ExhibitsList[0], sut.ExhibitsList[1]);
            Assert.AreNotSame(sut.ExhibitsList[0], sut.ExhibitsList[2]);
            Assert.AreNotSame(sut.ExhibitsList[1], sut.ExhibitsList[2]);
            Assert.NotNull(sut.ItemTappedCommand);
            Assert.IsFalse(sut.DisplayDistances);
        }

        [TestCase(0), Category("UnitTest")]
        [TestCase(1), Category("UnitTest")]
        [TestCase(2), Category("UnitTest")]
        public void ItemTapped_Once(int item)
        {
            var sut = CreateSystemUnderTest();

            sut.ItemTappedCommand.Execute(sut.ExhibitsList[item]);
            navservice.ReceivedWithAnyArgs().PushAsync(null);
        }

        #region Helper Methods

        private ExhibitsOverviewViewModel CreateSystemUnderTest()
        {
            var set = Substitute.For<ExhibitSet>();
            var exhibitList = new List<Exhibit> { CreateExhibit("Exhibit 1", 51, 7), CreateExhibit("Exhibit 2", 52, 8), CreateExhibit("Exhibit 3", 52.5, 7.5) };
            set.ActiveSet.Returns(exhibitList);

            return new ExhibitsOverviewViewModel(set);
        }

        private Exhibit CreateExhibit(string name, double latitude = 0, double longitude = 0)
        {
            var exhibit = Substitute.For<Exhibit>();
            exhibit.Name = name;
            exhibit.Image = CreateImage();

            exhibit.Location = CreateGeoLocation(latitude, longitude);

            var pages = new List<Page> { CreateAppetizerPage() };
            exhibit.Pages.Returns(pages);

            return exhibit;
        }

        private GeoLocation CreateGeoLocation(double latitude, double longitude)
        {
            var geolocation = Substitute.For<GeoLocation>();
            geolocation.Latitude.Returns(latitude);
            geolocation.Longitude.Returns(longitude);
            return geolocation;
        }

        private Page CreateAppetizerPage()
        {
            var page = Substitute.For<Page>();
            page.AppetizerPage = Substitute.For<AppetizerPage>();
            page.AppetizerPage.Image = CreateImage();
            return page;
        }

        private Image CreateImage()
        {
            var image = Substitute.For<Image>();
            image.Data = new byte[] { 1, 2, 3, 4 };
            return image;
        }

        #endregion
    }
}