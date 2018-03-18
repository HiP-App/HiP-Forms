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
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;

namespace PaderbornUniversity.SILab.Hip.Mobile.HipMobileUITests.ViewModels.Pages
{
    public class UserOnboardingPageViewModelTest
    {
        [TestFixtureSetUp]
        public void Init()
        {
            IoCManager.RegisterInstance(typeof(INavigationService), Substitute.For<INavigationService>());
        }

        [Test, Category("UnitTest")]
        public void Creation_PropertiesFilled()
        {
            var sut = CreateSystemUnderTest();

            Assert.NotNull(sut.ForwardCommand);
            Assert.NotNull(sut.ForwardCommand);
            Assert.IsTrue(sut.Pages.Count == 3);
            Assert.IsTrue(sut.SelectedPage == 0);
            Assert.IsFalse(sut.IsFinishVisible);
            Assert.IsTrue(sut.IsSkipVisible);
            Assert.IsTrue(sut.IsForwardVisible);
        }

        [Test, Category("UnitTest")]
        public void SelectedPage_Increased()
        {
            var sut = CreateSystemUnderTest();

            sut.ForwardCommand.Execute(null);
            Assert.IsTrue(sut.SelectedPage == 1);
            Assert.IsTrue(sut.IsForwardVisible);
            Assert.IsTrue(sut.IsSkipVisible);
            Assert.IsFalse(sut.IsFinishVisible);

            sut.ForwardCommand.Execute(null);
            Assert.IsTrue(sut.SelectedPage == 2);
            Assert.IsFalse(sut.IsForwardVisible);
            Assert.IsFalse(sut.IsSkipVisible);
            Assert.IsTrue(sut.IsFinishVisible);
        }

        [Test, Category("UnitTest")]
        public void SelectedPage_Decreased()
        {
            var sut = CreateSystemUnderTest();
            sut.SelectedPage = 2;

            sut.SelectedPage = 1;
            Assert.IsTrue(sut.SelectedPage == 1);
            Assert.IsTrue(sut.IsForwardVisible);
            Assert.IsTrue(sut.IsSkipVisible);
            Assert.IsFalse(sut.IsFinishVisible);

            sut.SelectedPage = 0;
            Assert.IsTrue(sut.SelectedPage == 0);
            Assert.IsTrue(sut.IsForwardVisible);
            Assert.IsTrue(sut.IsSkipVisible);
            Assert.IsFalse(sut.IsFinishVisible);
        }

        #region Helper Methods

        public UserOnboardingPageViewModel CreateSystemUnderTest()
        {
            return new UserOnboardingPageViewModel();
        }

        #endregion
    }
}