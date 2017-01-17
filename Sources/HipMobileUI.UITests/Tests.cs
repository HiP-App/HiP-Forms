using NUnit.Framework;
using Xamarin.UITest;

namespace HipMobileUI.UITests
{
    [TestFixture]
    public class Tests
    {
        private IApp app;

        private void BeforeEachTest(Platform platform)
        {
            switch (platform)
            {
                case Platform.Android:
                    app = ConfigureApp.Android
                            .DeviceSerial ("169.254.190.187:5555")
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

        #region AppLaunches_MainScreenHasTextBlue
        [TestCase(Platform.Android, Category= "UITestAndroid")]
        [TestCase(Platform.iOS, Category = "UITestiOS")]
        public void AppLaunches_MainScreenHasTextBlue(Platform platform)
        {
            BeforeEachTest(platform);

            var result = app.Query(x => x.Text("Blue"));
            Assert.Greater(result.Length, 0);
        }

        #endregion

        #region DummyViewsUiTest

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
            if (platform == Platform.Android)
            {
                return "OK";
            }
            else
            {
                return "Menu";
            }
        }

        #endregion

    }
}

