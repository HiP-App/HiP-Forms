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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using NSubstitute;
using NUnit.Framework;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views.ExhibitDetails;

namespace PaderbornUniversity.SILab.Hip.Mobile.HipMobileUITests.ViewModels.Views.ExhibitDetails
{
    class TimeSliderViewModelTest
    {
        [OneTimeSetUp]
        public void Init()
        {
            IoCManager.RegisterInstance(typeof(INavigationService), Substitute.For<INavigationService>());
            IoCManager.RegisterInstance(typeof(IImageDimension), Substitute.For<IImageDimension>());
            IoCManager.RegisterInstance(typeof(IMediaFileManager), Substitute.For<IMediaFileManager>());
        }

        [Test, Category("UnitTest")]
        public void Creation_PropertiesFilled()
        {
            var sut = CreateSystemUnderTest(() => { });

            Assert.AreEqual(sut.Headline, "A title");
            Assert.AreEqual(sut.Description, "A text");
            Assert.NotNull(sut.Images);
            Assert.IsTrue(Math.Abs(sut.SelectedValue) < 0.01);
            Assert.IsTrue(sut.DisplayedText.Equals("Foo"));
        }

        [Test, Category("UnitTest")]
        public void SelectedValue_DisplayedText()
        {
            var sut = CreateSystemUnderTest(() => { });

            sut.SelectedValue = 1;
            Assert.IsTrue(sut.DisplayedText.Equals("Bar"));
            sut.SelectedValue = 1.3;
            Assert.IsTrue(sut.DisplayedText.Equals("Bar"));
            sut.SelectedValue = 1.7;
            Assert.IsTrue(sut.DisplayedText.Equals("69"));
            sut.SelectedValue = 2;
            Assert.IsTrue(sut.DisplayedText.Equals("69"));
            sut.SelectedValue = 2.4;
            Assert.IsTrue(sut.DisplayedText.Equals("69"));
        }

        [Test, Category("UnitTest")]
        public void ToggleButtonVisibility_ActionCalled()
        {
            var actionSub = Substitute.For<Action>();
            var sut = CreateSystemUnderTest(actionSub);

            sut.ToggleButtonVisibility.Execute(null);

            actionSub.ReceivedWithAnyArgs().Invoke();
        }

        #region Helper Methods

        public TimeSliderViewModel CreateSystemUnderTest(Action action)
        {
            var timesliderPage = Substitute.For<TimeSliderPage>();
            List<Image> imageList = new List<Image> { CreateImage("Foo"), CreateImage("Bar"), CreateImage("69") };
            List<LongElement> dates = new List<LongElement> { CreateLongElement(1991), CreateLongElement(7867), CreateLongElement(454) };
            timesliderPage.Images.Returns(imageList);
            timesliderPage.Dates.Returns(dates);
            timesliderPage.Title = "A title";
            timesliderPage.Text = "A text";

            return new TimeSliderViewModel(timesliderPage, action);
        }

        private Image CreateImage(string description)
        {
            var image = Substitute.For<Image>();
            image.GetDataAsync().ReturnsForAnyArgs(Task.FromResult(new byte[] { 1, 2, 3, 4 }));
            image.GetDataBlocking().ReturnsForAnyArgs(new byte[] { 1, 2, 3, 4 });
            image.Title = "Foo";
            image.Description = description;
            return image;
        }

        private LongElement CreateLongElement(long year)
        {
            var longElement = Substitute.For<LongElement>();
            longElement.Value = year;
            return longElement;
        }

        #endregion
    }
}