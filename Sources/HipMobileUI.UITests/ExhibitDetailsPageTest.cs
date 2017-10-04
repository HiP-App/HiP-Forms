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

using System.Threading;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace HipMobileUI.UITests
{
    class ExhibitDetailsPageTest : UiTestBase
    {
        [TestCase(Platform.Android, Category = "UITestAndroid")]
        [TestCase(Platform.iOS, Category = "UITestiOS")]
        [Ignore]
        public void ExhibitDetailsTest(Platform platform)
        {
            BeforeEachTest(platform);

            App.ScrollDownTo("Die Pfalz Karls des Großen");
            App.Tap(x => x.Text("Die Pfalz Karls des Großen"));
            App.Repl();
            var result = App.Query(x => x.Marked("toolbar").Child(0).Text("Die Pfalz Karls des Großen"));
            Assert.Greater(result.Length, 0, "ExhibitDetailsPage could not be opened");

            while (HasNext())
            {
                GotoNext();
                if (HasFab())
                {
                    PressFab();
                    Thread.Sleep(1000);
                }
            }

            while (HasPrevious())
            {
                GotoPrevious();
            }
        }

        private bool HasNext()
        {
            return App.Query(x => x.Marked("NextButton")).Length > 0;
        }

        private bool HasPrevious()
        {
            return App.Query(x => x.Marked("PreviousButton")).Length > 0;
        }

        private bool HasFab()
        {
            return App.Query(x => x.Marked("Fab")).Length > 0;
        }

        private void GotoNext()
        {
            App.Tap(x => x.Marked("NextButton"));
        }

        private void GotoPrevious()
        {
            App.Tap(x => x.Marked("PreviousButton"));
        }

        private void PressFab()
        {
            App.Tap(x => x.Marked("Fab"));
        }
    }
}