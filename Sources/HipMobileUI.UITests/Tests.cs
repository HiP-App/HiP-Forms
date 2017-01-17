using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace HipMobileUI.UITests
{
    [TestFixture(Platform.Android)]
    [TestFixture(Platform.iOS)]
    public class Tests
    {
        private IApp app;
        private readonly Platform platform;

        public Tests(Platform platform)
        {
            this.platform = platform;
        }

        [SetUp]
        public void BeforeEachTest()
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
                                .AppBundle (@"..\..\..\HipMobileUI.iOS\bin\iPhoneSimulator\iOS Release\HipMobileUI.iOS.app")
                                .StartApp ();
                    break;
            }
            
        }

        [Test, Category("UITest")]
        public void AppLaunches_MainScreenHasTextBlue()
        {
            var result = app.Query (x => x.Text ("Blue"));
            Assert.Greater (result.Length, 0);
        }

        [Test, Category("UITest")]
        public void DummyViewsUiTest()
        {
            app.Tap(x => x.Marked("OK"));
            app.Tap(x => x.Text("Blue").Index(1));

            app.DragCoordinates (0, 500, 500, 500);
            app.Tap(x => x.Text("Red"));

            app.Tap(x => x.Marked("OK"));
            app.Tap(x => x.Text("Green"));
        }

    }
}

