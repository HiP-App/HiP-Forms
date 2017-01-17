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

        [Test, Category("UITestAndroid")]
        public void Android_AppLaunches_MainScreenHasTextBlue()
        {
            BeforeEachTest(Platform.Android);
            AppLaunches_MainScreenHasTextBlue ();
        }

        [Test, Category("UITestiOS")]
        public void IOS_AppLaunches_MainScreenHasTextBlue()
        {
            BeforeEachTest(Platform.iOS);
            AppLaunches_MainScreenHasTextBlue ();
        }

        private void AppLaunches_MainScreenHasTextBlue()
        {
            var result = app.Query (x => x.Text ("Blue"));
            Assert.Greater (result.Length, 0);
        }

        #endregion

        #region DummyViewsUiTest

        [Test, Category("UITestAndroid")]
        public void Android_DummyViewsUiTest()
        {
            BeforeEachTest(Platform.Android);
            DummyViewsUiTest ();
        }

        [Test, Category("UITestiOS")]
        public void IOS_DummyViewsUiTest()
        {
            BeforeEachTest(Platform.iOS);
            DummyViewsUiTest ();
        }


        private void DummyViewsUiTest()
        {
            app.Tap(x => x.Marked("OK"));
            app.Tap(x => x.Text("Blue").Index(1));

            app.DragCoordinates(0, 500, 500, 500);
            app.Tap(x => x.Text("Red"));

            app.Tap(x => x.Marked("OK"));
            app.Tap(x => x.Text("Green"));
        }

        #endregion

    }
}

