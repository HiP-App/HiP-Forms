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
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;

namespace PaderbornUniversity.SILab.Hip.Mobile.HipMobileUITests.ViewModels.Pages
{
    class LoadingPageViewModelTest
    {
        [TestFixtureSetUp]
        public void Init()
        {
            IoCManager.RegisterInstance(typeof(INavigationService), Substitute.For<INavigationService>());
            IoCManager.RegisterInstance(typeof(IImageDimension), Substitute.For<IImageDimension>());
        }

        [Test, Category("UnitTest")]
        public void Creation_IsExtendedViewsVisible()
        {
            var sut = CreateSystemUnderTest();

            Assert.IsFalse(sut.IsExtendedViewsVisible);
        }

        [Test, Category("UnitTest")]
        public void Creation_StartLoading()
        {
            var sut = CreateSystemUnderTest();

            Assert.NotNull(sut.StartLoading);
        }

        [Test, Category("UnitTest")]
        public void Creation_Text()
        {
            var sut = CreateSystemUnderTest();

            Assert.IsTrue(sut.Text.Equals(Strings.LoadingPage_Text));
        }

        [Test, Category("UnitTest")]
        public void Creation_Subtext()
        {
            var sut = CreateSystemUnderTest();

            Assert.IsTrue(sut.Subtext.Equals(Strings.LoadingPage_Subtext));
        }

        #region Helper Methods

        public LoadingPageViewModel CreateSystemUnderTest()
        {
            var sut = new LoadingPageViewModel();
            return sut;
        }

        #endregion
    }
}