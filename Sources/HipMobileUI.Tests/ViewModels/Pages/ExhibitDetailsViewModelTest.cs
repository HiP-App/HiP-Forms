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
using System.Threading;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using de.upb.hip.mobile.pcl.Common;
using de.upb.hip.mobile.pcl.Common.Contracts;
using HipMobileUI.AudioPlayer;
using HipMobileUI.Contracts;
using HipMobileUI.Helpers;
using HipMobileUI.Navigation;
using HipMobileUI.ViewModels.Pages;
using HipMobileUI.ViewModels.Views.ExhibitDetails;
using NSubstitute;
using NUnit.Framework;
using Xamarin.Forms;
using Image = de.upb.hip.mobile.pcl.BusinessLayer.Models.Image;
using Page = de.upb.hip.mobile.pcl.BusinessLayer.Models.Page;

namespace HipMobileUI.Tests.ViewModels.Pages
{
    class ExhibitDetailsViewModelTest
    {
        [TestFixtureSetUp]
        public void Init()
        {
            IoCManager.RegisterInstance(typeof(INavigationService), Substitute.For<INavigationService>());
            IoCManager.RegisterInstance(typeof(IImageDimension), Substitute.For<IImageDimension>());
            IoCManager.RegisterInstance(typeof(IAudioPlayer), Substitute.For<IAudioPlayer>());
            IoCManager.RegisterInstance (typeof(IBarsColorsChanger), Substitute.For<IBarsColorsChanger> ());
            IoCManager.RegisterInstance (typeof (ApplicationResourcesProvider), new ApplicationResourcesProvider (
                                             new Dictionary<string, object>
                                             {
                                                 {"PrimaryDarkColor", Color.Pink},
                                                 {"PrimaryColor", Color.Pink}
                                             }));
        }

        [Test, Category("UnitTest")]
        public void NextView_Single()
        {
            var sut = CreateSystemUnderTest();

            var selectedViewOne = sut.SelectedView;
            sut.NextViewCommand.Execute(null);
            var selectedViewTwo = sut.SelectedView;

            Assert.AreNotEqual(selectedViewOne, selectedViewTwo);
        }

        [Test, Category("UnitTest")]
        public void PreviousView_Single ()
        {
            var sut = CreateSystemUnderTest ();
            sut.NextViewCommand.Execute (null);

            var selectedViewOne = sut.SelectedView;
            sut.PreviousViewCommand.Execute (null);
            var selectedViewTwo = sut.SelectedView;

            Assert.AreNotEqual (selectedViewOne, selectedViewTwo);
        }

        [Test, Category("UnitTest")]
        public void Navigation_All()
        {
            var sut = CreateSystemUnderTest();

            // forwards navigation
            int navigations = 0;
            while (sut.NextViewAvailable)
            {
                sut.NextViewCommand.Execute (null);
                navigations++;
            }
            Assert.AreEqual (navigations, 2);

            // backward navigation
            while (sut.PreviousViewAvailable)
            {
                sut.PreviousViewCommand.Execute(null);
                navigations--;
            }
            Assert.AreEqual(navigations, 0);
        }

        [Test, Category ("UnitTest")]
        public void Creation_PropertiesFilled ()
        {
            var sut = CreateSystemUnderTest ();

            Assert.AreEqual (sut.PreviousViewAvailable, false);
            Assert.AreEqual(sut.NextViewAvailable, true);
            Assert.NotNull (sut.SelectedView);
            Assert.NotNull (sut.NextViewCommand);
            Assert.NotNull(sut.PreviousViewCommand);
        }

        [Test, Category("UnitTest")]
        public void PageToViewModel_All ()
        {
            var sut = CreateSystemUnderTest ();

            Assert.IsInstanceOf<AppetizerViewModel> (sut.SelectedView);
            sut.NextViewCommand.Execute (null);
            Assert.IsInstanceOf<ImageViewModel>(sut.SelectedView);
            sut.NextViewCommand.Execute(null);
            Assert.IsInstanceOf<TimeSliderViewModel>(sut.SelectedView);
        }

        [Test, Category("UnitTest")]
        public void ToggleButtonVisibility_IsVisibleChangedAfter3Secs()
        {
            var sut = CreateSystemUnderTest ();
            sut.NextViewCommand.Execute (null);

            Assert.AreEqual(true, sut.NextVisible);
            Assert.AreEqual(true, sut.PreviousVisible);

            Thread.Sleep(3000);

            Assert.AreEqual(false, sut.NextVisible);
            Assert.AreEqual(false, sut.PreviousVisible);
        }

        #region Helper Methods

        public ExhibitDetailsViewModel CreateSystemUnderTest()
        {
            var exhibit = Substitute.For<Exhibit> ();
            var pages = new List<Page> {CreateAppetizerPage (), CreateImagePage (), CreateTimeSliderPage ()};
            exhibit.Pages.Returns (pages);
            
            return new ExhibitDetailsViewModel (exhibit);
        }

        private Page CreateAppetizerPage ()
        {
            var page = Substitute.For<Page> ();
            page.AppetizerPage = Substitute.For<AppetizerPage> ();
            page.AppetizerPage.Image = CreateImage ();
            return page;
        }
        private Page CreateImagePage()
        {
            var page = Substitute.For<Page>();
            page.ImagePage = Substitute.For<ImagePage>();
            page.ImagePage.Image = CreateImage ();
            return page;
        }

        private Page CreateTimeSliderPage()
        {
            var page = Substitute.For<Page>();
            page.TimeSliderPage = Substitute.For<TimeSliderPage>();
            return page;
        }

        private Image CreateImage ()
        {
            var image = Substitute.For<Image>();
            image.Data = new byte[] { 1, 2, 3, 4 };
            return image;
        }
        #endregion
    }
}
