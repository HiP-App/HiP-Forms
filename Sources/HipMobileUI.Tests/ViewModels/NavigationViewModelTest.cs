// Copyright (C) 2016 History in Paderborn App - Universität Paderborn
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

using System.ComponentModel;
using de.upb.hip.mobile.pcl.Common;
using HipMobileUI.Navigation;
using HipMobileUI.ViewModels;
using NSubstitute;
using NUnit.Framework;

namespace HipMobileUI.Tests.ViewModels {
    [TestFixture]
    public class NavigationViewModelTest {

        [TestFixtureSetUp]
        public void Init ()
        {
            IoCManager.RegisterInstance (typeof(INavigationService), Substitute.For<INavigationService> ());
        }

        [Test, NUnit.Framework.Category("UnitTest")]
        public void Title_SetGet ()
        {
            var sut = CreateSystemUnderTest ();

            var title = "Foo";
            sut.Title = title;

            Assert.AreEqual (sut.Title, title);
        }

        [Test, NUnit.Framework.Category("UnitTest")]
        public void IsBusy_SetGet ()
        {
            var sut = CreateSystemUnderTest();

            sut.IsBusy = true;

            Assert.AreEqual (sut.IsBusy, true);
            Assert.AreEqual (sut.IsNotBusy, false);

            sut.IsBusy = false;

            Assert.AreEqual (sut.IsBusy, false);
            Assert.AreEqual (sut.IsNotBusy, true);
        }

        [Test, NUnit.Framework.Category("UnitTest")]
        public void PropertyChanged_Invoked ()
        {
            var sut = CreateSystemUnderTest();
            var handler = Substitute.For<PropertyChangedEventHandler> ();
            sut.PropertyChanged += handler;

            sut.Title = "Foo";
            handler.ReceivedWithAnyArgs(1).Invoke (null, null);
        }

        [Test, NUnit.Framework.Category("UnitTest")]
        public void PropertyChanged_InvokedTwice()
        {
            var sut = CreateSystemUnderTest();
            var handler = Substitute.For<PropertyChangedEventHandler>();
            sut.PropertyChanged += handler;
            var s = "Foo";

            sut.Title = s;
            sut.Title = s;
            handler.ReceivedWithAnyArgs(1).Invoke(null, null);
        }

        #region Helper Methods

        public NavigationViewModel CreateSystemUnderTest ()
        {
            return Substitute.For<NavigationViewModel> ();
        }
        #endregion

    }
}