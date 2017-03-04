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
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using de.upb.hip.mobile.pcl.Common;
using de.upb.hip.mobile.pcl.Common.Contracts;
using HipMobileUI.Navigation;
using HipMobileUI.ViewModels.Views.ExhibitDetails;
using NSubstitute;
using NUnit.Framework;

namespace HipMobileUI.Tests.ViewModels.Views.ExhibitDetails
{
    [TestFixture]
    public class TextViewModelTest
    {
        [TestFixtureSetUp]
        public void Init()
        {
            IoCManager.RegisterInstance(typeof(INavigationService), Substitute.For<INavigationService>());
        }

        [Test, Category("UnitTest")]
        public void Creation_PropertiesFilled()
        {
            var sut = CreateSystemUnderTest(() => { });

            Assert.AreEqual(Text, sut.Text);
            Assert.AreEqual(Font, sut.FontFamily);
            Assert.AreEqual(Title, sut.Headline);
            Assert.AreEqual(Description, sut.Description);
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

        private const string Title = "Title Test";
        private const string Description = "Description Test";
        private const string Text = "This is a test text.";
        private const string Font = "Test Font";
        public TextViewModel CreateSystemUnderTest(Action action)
        {
            var textPage = Substitute.For<TextPage>();
            textPage.Text = Text;
            textPage.FontFamily = Font;
            textPage.Title = Title;
            textPage.Description = Description;

            return new TextViewModel(textPage, action);
        }

        #endregion
    }
}
