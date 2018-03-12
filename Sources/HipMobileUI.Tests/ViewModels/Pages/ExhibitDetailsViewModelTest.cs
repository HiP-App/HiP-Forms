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
using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using NSubstitute;
using NUnit.Framework;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.UI.AudioPlayer;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views.ExhibitDetails;
using Xamarin.Forms;
using Image = PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.Image;
using Page = PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.Page;

namespace PaderbornUniversity.SILab.Hip.Mobile.HipMobileUITests.ViewModels.Pages
{
    class ExhibitDetailsViewModelTest
    {
        [OneTimeSetUp]
        public void Init()
        {
            IoCManager.Clear();
            IoCManager.RegisterInstance(typeof(INavigationService), Substitute.For<INavigationService>());
            IoCManager.RegisterInstance(typeof(IMediaFileManager), Substitute.For<IMediaFileManager>());
            IoCManager.RegisterInstance(typeof(IImageDimension), Substitute.For<IImageDimension>());
            IoCManager.RegisterInstance(typeof(IAudioPlayer), Substitute.For<IAudioPlayer>());
            IoCManager.RegisterInstance(typeof(IDbChangedHandler), Substitute.For<IDbChangedHandler>());
            IoCManager.RegisterInstance(typeof(ApplicationResourcesProvider), new ApplicationResourcesProvider(
                                            new Dictionary<string, object>
                                            {
                                                { "PrimaryDarkColor", Color.Pink },
                                                { "PrimaryColor", Color.Pink }
                                            }));
        }

        [Test, Category("UnitTest")]
        public void NextView_Single()
        {
            IoCManager.RegisterInstance(typeof(IBarsColorsChanger), Substitute.For<IBarsColorsChanger>());
            var sut = CreateSystemUnderTest();

            var selectedViewOne = sut.SelectedView;
            sut.NextViewCommand.Execute(null);
            var selectedViewTwo = sut.SelectedView;

            Assert.AreNotEqual(selectedViewOne, selectedViewTwo);
        }

        [Test, Category("UnitTest")]
        public void PreviousView_Single()
        {
            IoCManager.RegisterInstance(typeof(IBarsColorsChanger), Substitute.For<IBarsColorsChanger>());
            var sut = CreateSystemUnderTest();
            sut.NextViewCommand.Execute(null);

            var selectedViewOne = sut.SelectedView;
            sut.PreviousViewCommand.Execute(null);
            var selectedViewTwo = sut.SelectedView;

            Assert.AreNotEqual(selectedViewOne, selectedViewTwo);
        }

        [Test, Category("UnitTest")]
        public void Navigation_All()
        {
            IoCManager.RegisterInstance(typeof(IBarsColorsChanger), Substitute.For<IBarsColorsChanger>());
            var sut = CreateSystemUnderTest();

            // forwards navigation
            var navigations = 0;
            while (sut.NextViewAvailable)
            {
                sut.NextViewCommand.Execute(null);
                navigations++;
            }
            Assert.AreEqual(navigations, 1);

            // backward navigation
            while (sut.PreviousViewAvailable)
            {
                sut.PreviousViewCommand.Execute(null);
                navigations--;
            }
            Assert.AreEqual(navigations, 0);
        }

        [Test, Category("UnitTest")]
        public void Creation_PropertiesFilled()
        {
            IoCManager.RegisterInstance(typeof(IBarsColorsChanger), Substitute.For<IBarsColorsChanger>());
            var sut = CreateSystemUnderTest();

            Assert.AreEqual(sut.PreviousViewAvailable, false);
            Assert.AreEqual(sut.NextViewAvailable, true);
            Assert.NotNull(sut.SelectedView);
            Assert.NotNull(sut.NextViewCommand);
            Assert.NotNull(sut.PreviousViewCommand);
        }

        [Test, Category("UnitTest")]
        public void PageToViewModel_All()
        {
            IoCManager.RegisterInstance(typeof(IBarsColorsChanger), Substitute.For<IBarsColorsChanger>());
            var sut = CreateSystemUnderTest();

            Assert.IsInstanceOf<TimeSliderViewModel>(sut.SelectedView);
            sut.NextViewCommand.Execute(null);
            Assert.IsInstanceOf<ImageViewModel>(sut.SelectedView);
        }

        [Test, Category("UnitTest")]
        public void ToggleButtonVisibility_IsVisibleChangedAfter3Secs()
        {
            IoCManager.RegisterInstance(typeof(IBarsColorsChanger), Substitute.For<IBarsColorsChanger>());
            var sut = CreateSystemUnderTest();
            sut.NextViewCommand.Execute(null);

            Assert.AreEqual(false, sut.NextVisible);
            Assert.AreEqual(true, sut.PreviousVisible);

            Thread.Sleep(3000);

            Assert.AreEqual(false, sut.NextVisible);
            Assert.AreEqual(false, sut.PreviousVisible);
        }

        [Test, Category("UnitTest")]
        public void Creation_AdditionalInformationWithCorrectStatusBarColors()
        {
            var resources = Substitute.For<IBarsColorsChanger>();
            IoCManager.RegisterInstance(typeof(IBarsColorsChanger), resources);
            // ReSharper disable once UnusedVariable
            // It's necessary to create this object once
            var sut = new ExhibitDetailsPageViewModel(new Exhibit(), new List<Page>(), "Test", true);

            resources.Received().ChangeToolbarColor(Color.FromRgb(128, 128, 128), Color.FromRgb(169, 169, 169));
        }

        [Test, Category("UnitTest")]
        public void Creation_NormalStatusBarColors()
        {
            var resources = Substitute.For<IBarsColorsChanger>();
            IoCManager.RegisterInstance(typeof(IBarsColorsChanger), resources);
            // ReSharper disable once UnusedVariable
            // It's necessary to create this object once
            var sut = new ExhibitDetailsPageViewModel(new Exhibit(), new List<Page>(), "Test");

            resources.Received().ChangeToolbarColor(Color.Pink, Color.Pink);
        }

        #region Helper Methods

        public ExhibitDetailsPageViewModel CreateSystemUnderTest()
        {
            var exhibit = Substitute.For<Exhibit>();
            var pages = new List<Page> { CreateImagePage(), CreateTimeSliderPage(), CreateImagePage() };
            exhibit.Pages.Returns(pages);

            exhibit.Unlocked = true;
            return new ExhibitDetailsPageViewModel(exhibit);
        }

        private Page CreateAppetizerPage()
        {
            var page = Substitute.For<AppetizerPage>();
            page.Image = CreateImage();
            return page;
        }

        private Page CreateImagePage()
        {
            var page = Substitute.For<ImagePage>();
            page.Image = CreateImage();
            return page;
        }

        private Page CreateTimeSliderPage()
        {
            var page = Substitute.For<TimeSliderPage>();
            return page;
        }

        private Image CreateImage()
        {
            var image = Substitute.For<Image>();
            image.GetDataAsync().ReturnsForAnyArgs(Task.FromResult(new byte[] { 1, 2, 3, 4 }));
            image.GetDataBlocking().ReturnsForAnyArgs(new byte[] { 1, 2, 3, 4 });
            return image;
        }

        #endregion
    }
}