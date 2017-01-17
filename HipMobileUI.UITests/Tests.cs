using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace HipMobileUI.UITests
{
    [TestFixture(Platform.Android)]
    public class Tests
    {
        IApp app;
        Platform platform;

        public Tests(Platform platform)
        {
            this.platform = platform;
        }

        [SetUp]
        public void BeforeEachTest()
        {
            app = ConfigureApp.Android
                              .ApkFile (@"C:\Data\Git\HiP-Forms\Sources\HipMobileUI.Droid\bin\Release\de.upb.hip.mobile.droid.forms-Signed.apk")
                              .StartApp ();
        }

        [Test]
        public void AppLaunches()
        {
           
        }
    }
}

