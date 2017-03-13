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
using NUnit.Framework;
using Xamarin.UITest;

namespace HipMobileUI.UITests
{
    /// <summary>
    /// Class containg UI tests for the application
    /// On windows only the android tests can be executed (exactly one android simulator needs to run)
    /// </summary>
    [TestFixture]
    public class MainPageUiTests : UiTestBase
    {
        [TestCase(Platform.Android, Category = "UITestAndroid")]
        [TestCase(Platform.iOS, Category = "UITestiOS")]
        [Ignore]
        public void MenuEntrySelectionTest(Platform platform)
        {
            BeforeEachTest(platform);

            var titles = new List<string> { "Routen", "Einstellungen", "Feedback", "Rechtliche Hinweise", "Übersicht" };
            
            // intended duplicated for loop (because selecting an already selected element has no effect)
            foreach (var title in titles)
            {
                TestEntry(platform, title, false);
            }
            foreach (var title in titles)
            {
                TestEntry(platform, title, true);
            }

        }

        /// <summary>
        /// Checks whether a menu entry with the specified title exists and can be selected.
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="title">The title of the entry to check for.</param>
        /// <param name="drag">Indicates whether the menu should be opened via dragging (true) or via tapping (false).</param>
        public void TestEntry (Platform platform, string title, bool drag = false)
        {
            if (drag)
                App.DragCoordinates(5, 500, 500, 500);
            else
                App.Tap(x => x.Marked(GetPlatformMenuName(platform)));

            App.WaitForElement(x => x.Text(title), timeout: TimeSpan.FromMinutes(1));
            App.Tap(x => x.Text(title));
            App.WaitForElement (x => x.Marked (GetPlatformMenuName (platform)), timeout: TimeSpan.FromMinutes(1));
        }
    }
}

