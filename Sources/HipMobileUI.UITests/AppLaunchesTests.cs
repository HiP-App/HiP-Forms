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

using NUnit.Framework;
using Xamarin.UITest;

namespace HipMobileUI.UITests
{
    /// <summary>
    /// Class containg UI tests for the application
    /// On windows only the android tests can be executed (exactly one android simulator needs to run)
    /// </summary>
    [TestFixture]
    public class AppLaunchesTests : UiTestBase
    {
        [TestCase(Platform.Android, Category= "UITestAndroid")]
        [TestCase(Platform.iOS, Category = "UITestiOS")]
        public void AppLaunches_MainScreenHasTextBlue(Platform platform)
        {
            BeforeEachTest(platform);

            var result = App.Query(x => x.Marked(GetPlatformMenuName (platform)));
            Assert.Greater(result.Length, 0);
        }
    }
}

