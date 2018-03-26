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

using NSubstitute;
using NUnit.Framework;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.HipMobileUITests.ViewModels.Pages
{
    class UserOnboardingItemViewModelTest
    {
        [OneTimeSetUp]
        public void Init()
        {
            IoCManager.RegisterInstance(typeof(INavigationService), Substitute.For<INavigationService>());
        }

        [Test, Category("UnitTest")]
        public void Creation_PropertiesFilled()
        {
            var sut = CreateSystemUnderTest();

            Assert.IsTrue(sut.Headline.Equals("Foo"));
            Assert.IsTrue(sut.Text.Equals("bar"));
            Assert.NotNull(sut.Image);
        }

        [Test, Category("UnitTest")]
        public void Rotation_Image()
        {
            var sut = CreateSystemUnderTest();

            sut.OrientationChanged(DeviceOrientation.Landscape);
            Assert.NotNull(sut.Image);
        }

        #region Helper methods

        public UserOnboardingItemViewModel CreateSystemUnderTest()
        {
            var sut = new UserOnboardingItemViewModel("Foo", "bar", "image", Color.Blue, "landscape");

            return sut;
        }

        #endregion
    }
}