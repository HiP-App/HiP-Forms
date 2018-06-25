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
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using NSubstitute;
using NUnit.Framework;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views.ExhibitDetails;

namespace PaderbornUniversity.SILab.Hip.Mobile.HipMobileUITests.ViewModels.Views.ExhibitDetails
{
    [TestFixture]
    public class TextViewModelTest
    {
        [OneTimeSetUp]
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

            return new TextViewModel(textPage);
        }

        #endregion
    }
}