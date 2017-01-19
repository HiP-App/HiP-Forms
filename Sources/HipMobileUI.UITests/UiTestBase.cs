using System.ComponentModel;
using Xamarin.UITest;

namespace HipMobileUI.UITests {
    public class UiTestBase {

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
                    throw new InvalidEnumArgumentException(nameof(platform), (int)platform, typeof(Platform));
            }
        }

    }
}