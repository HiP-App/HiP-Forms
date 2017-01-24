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
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using de.upb.hip.mobile.pcl.Common;
using de.upb.hip.mobile.pcl.Common.Contracts;
using de.upb.hip.mobile.pcl.DataAccessLayer;
using HipMobileUI.Navigation;
using HipMobileUI.ViewModels.Views;
using NSubstitute;
using NUnit.Framework;
using Plugin.Geolocator.Abstractions;

namespace HipMobileUI.Tests.ViewModels.Views
{

    [TestFixture]
    class ExhibitsOverviewViewModelTest
    {

        [TestFixtureSetUp]
        public void Init()
        {
            IoCManager.RegisterInstance(typeof(INavigationService), Substitute.For<INavigationService>());
            IoCManager.RegisterInstance(typeof(IImageDimension), Substitute.For<IImageDimension>());
            IoCManager.RegisterInstance(typeof(IDataAccess), Substitute.For<IDataAccess>());
        }

        [Test, Category("UnitTest")]
        public void Creation_PropertiesFilled()
        {
            var sut = CreateSystemUnderTest();

            Assert.AreEqual (sut.ExhibitsList.Count, 3);
            Assert.AreNotSame (sut.ExhibitsList[0], sut.ExhibitsList[1]);
            Assert.AreNotSame(sut.ExhibitsList[0], sut.ExhibitsList[2]);
            Assert.AreNotSame(sut.ExhibitsList[1], sut.ExhibitsList[2]);
            Assert.NotNull (sut.ItemTappedCommand);
        }

        [Test, Category("UnitTest")]
        public void ItemTapped_Once()
        {
            var sut = CreateSystemUnderTest();
            var navigationService = IoCManager.Resolve<INavigationService> ();

            sut.ItemTappedCommand.Execute (sut.ExhibitsList[0]);
            navigationService.ReceivedWithAnyArgs ().PushAsync (null);
        }

        #region Helper Methods
        private ExhibitsOverviewViewModel CreateSystemUnderTest ()
        {
            var set = Substitute.For<ExhibitSet> ();
            var exhibitList = new List<Exhibit> {CreateExhibit ("Exhibit 1"), CreateExhibit ("Exhibit 2"), CreateExhibit ("Exhibit 3")};
            set.ActiveSet.Returns (exhibitList);

            var locator = Substitute.For<IGeolocator> ();

            return new ExhibitsOverviewViewModel (set, locator);
        }

        private Exhibit CreateExhibit (string name)
        {
            var exhibit = Substitute.For<Exhibit> ();
            exhibit.Name = name;
            exhibit.Image = CreateImage ();

            var pages = new List<Page> { CreateAppetizerPage() };
            exhibit.Pages.Returns(pages);

            return exhibit;
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
