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
                              .ApkFile(@"..\..\..\HipMobileUI.Droid\bin\Release\de.upb.hip.mobile.droid.forms.apk")
                              .StartApp();
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
            const int blue = -16776961; //android blue int
            const int red = -65536; //android red int
            const int green = -16744448; //android green int

            app.Tap(x => x.Marked("OK"));
            app.Tap(x => x.Text("Blue").Index(1));
            var blueColor = app.Query(x => x.Class("BoxRenderer").Invoke("getBackground").Invoke("getColor").Value<int>()).First();
            Assert.AreEqual(blue, blueColor); 

            app.DragCoordinates (0, 500, 500, 500);
            app.Tap(x => x.Text("Red"));
            var redColor = app.Query(x => x.Class("BoxRenderer").Invoke("getBackground").Invoke("getColor").Value<int>()).First();
            Assert.AreEqual(red, redColor); 

            app.Tap(x => x.Marked("OK"));
            app.Tap(x => x.Text("Green"));
            var greenColor = app.Query(x => x.Class("BoxRenderer").Invoke("getBackground").Invoke("getColor").Value<int>()).First();
            Assert.AreEqual(green, greenColor); 
        }

    }
}

