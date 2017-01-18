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

using System.ComponentModel;
using NUnit.Framework;
using Xamarin.UITest;

namespace HipMobileUI.UITests
{
    /// <summary>
    /// Class containg UI tests for the application
    /// On windows only the android tests can be executed (exactly one android simulator needs to run)
    /// </summary>
    [TestFixture]
    public class UiTests
    {
        private IApp app;

        /// <summary>
        /// Sets up the application according to the platform, the test should run on
        /// </summary>
        /// <param name="platform"></param>
        private void BeforeEachTest(Platform platform)
        {
            switch (platform)
            {
                case Platform.Android:
                    app = ConfigureApp.Android
                                .ApkFile(@"..\..\..\HipMobileUI.Droid\bin\Release\de.upb.hip.mobile.droid.forms.apk")
                                .StartApp();
                    break;
                case Platform.iOS:
                    app = ConfigureApp.iOS
                                .AppBundle (@"../../../HipMobileUI.iOS/bin/iPhoneSimulator/iOS Release/HipMobileUI.iOS.app")
                                .StartApp ();
                    break;
            }
        }

        [TestCase(Platform.Android, Category= "UITestAndroid")]
        [TestCase(Platform.iOS, Category = "UITestiOS")]
        public void AppLaunches_MainScreenHasTextBlue(Platform platform)
        {
            BeforeEachTest(platform);

            var result = app.Query(x => x.Text("Blue"));
            Assert.Greater(result.Length, 0);
        }

        [TestCase(Platform.Android, Category = "UITestAndroid")]
        [TestCase(Platform.iOS, Category = "UITestiOS")]
        public void DummyViewsUiTest(Platform platform)
        {
            BeforeEachTest(platform);

            app.Tap(x => x.Marked(GetPlatformMenuName (platform)));
            app.Tap(x => x.Text("Blue").Index(1));

            app.DragCoordinates(0, 500, 500, 500);
            app.Tap(x => x.Text("Red"));

            app.Tap(x => x.Marked(GetPlatformMenuName(platform)));
            app.Tap(x => x.Text("Green"));
        }

        private string GetPlatformMenuName (Platform platform)
        {
            switch (platform)
            {
                case Platform.Android:
                    return "OK"; //text for the burger menu icon in android
                case Platform.iOS:
                    return "Menu";
                default:
                    throw new InvalidEnumArgumentException (nameof (platform), (int) platform, typeof (Platform));
            }
        }

    }
}

