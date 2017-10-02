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

using System.ComponentModel;
using Xamarin.UITest;

namespace HipMobileUI.UITests
{
    public class UiTestBase
    {
        protected IApp App { get; set; }

        /// <summary>
        /// Sets up the application according to the platform, the test should run on
        /// </summary>
        /// <param name="platform"></param>
        protected void BeforeEachTest(Platform platform)
        {
            switch (platform)
            {
                case Platform.Android:
                    App = ConfigureApp.Android
                                      .ApkFile(@"..\..\..\HipMobileUI.Droid\bin\Release\de.upb.hip.mobile.droid.forms.apk")
                                      .StartApp();
                    break;
                case Platform.iOS:
                    App = ConfigureApp.iOS
                                      .AppBundle(@"../../../HipMobileUI.iOS/bin/iPhoneSimulator/iOS Release/HipMobileUI.iOS.app")
                                      .StartApp();
                    break;
            }
        }

        protected string GetPlatformMenuName(Platform platform)
        {
            switch (platform)
            {
                case Platform.Android:
                    return "OK"; //text for the burger menu icon in android
                case Platform.iOS:
                    return "Menu";
                default:
                    throw new InvalidEnumArgumentException(nameof(platform), (int) platform, typeof(Platform));
            }
        }
    }
}