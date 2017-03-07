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
using de.upb.hip.mobile.pcl.Common;
using de.upb.hip.mobile.pcl.Common.Contracts;
using FFImageLoading.Forms.Touch;
using Foundation;
using HipMobileUI.AudioPlayer;
using HipMobileUI.Contracts;
using HipMobileUI.iOS.Contracts;
using HipMobileUI.Location;
using HipMobileUI.Navigation;
using HipMobileUI.Pages;
using UIKit;

namespace HipMobileUI.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            IoCManager.RegisterType<IImageDimension, IosImageDimensions>();

            // Init Navigation
            NavigationService.Instance.RegisterViewModels (typeof(MainPage).Assembly);
            IoCManager.RegisterInstance (typeof(INavigationService), NavigationService.Instance);
            IoCManager.RegisterInstance(typeof(IViewCreator), NavigationService.Instance);

            // init other inversion of control classes
            IoCManager.RegisterInstance (typeof(IAudioPlayer), new IosAudioPlayer ());
            IoCManager.RegisterType<IStatusBarController, IosStatusBarController> ();
            IoCManager.RegisterInstance(typeof(ILocationManager), new LocationManager());

            // init forms and third party libraries
            CachedImageRenderer.Init ();
            Xamarin.Forms.Forms.Init();
            Xamarin.FormsMaps.Init();

            LoadApplication(new App());

#if ENABLE_TEST_CLOUD
            Xamarin.Calabash.Start();
#endif

            return base.FinishedLaunching(app, options);
        }
    }
}
