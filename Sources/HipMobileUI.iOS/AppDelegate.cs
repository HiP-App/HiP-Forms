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

using FFImageLoading.Forms.Touch;
using Foundation;
using HockeyApp.iOS;
using PaderbornUniversity.SILab.Hip.Mobile.Ios.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.DtoToModelConverters;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.UI.AudioPlayer;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Location;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;
using UIKit;
using App = PaderbornUniversity.SILab.Hip.Mobile.UI.App;
using MainPage = PaderbornUniversity.SILab.Hip.Mobile.UI.Pages.MainPage;

namespace PaderbornUniversity.SILab.Hip.Mobile.Ios
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
            NavigationService.Instance.RegisterViewModels(typeof(MainPage).Assembly);
            IoCManager.RegisterInstance(typeof(INavigationService), NavigationService.Instance);
            IoCManager.RegisterInstance(typeof(IViewCreator), NavigationService.Instance);

            // init other inversion of control classes
            IoCManager.RegisterInstance(typeof(IAudioPlayer), new IosAudioPlayer());
            IoCManager.RegisterType<IStatusBarController, IosStatusBarController>();
            IoCManager.RegisterInstance(typeof(ILocationManager), new LocationManager());
            IoCManager.RegisterInstance(typeof(IKeyProvider), new IosKeyProvider());
            IoCManager.RegisterInstance(typeof(IBarsColorsChanger), new IosBarsColorsChanger());
            IoCManager.RegisterInstance(typeof(IDbChangedHandler), new DbChangedHandler());
            IoCManager.RegisterInstance(typeof(INetworkAccessChecker), new IosNetworkAccessChecker());
            IoCManager.RegisterInstance(typeof(IStorageSizeProvider), new IosStorageSizeProvider ());

            // init crash manager
            var manager = BITHockeyManager.SharedHockeyManager;
            manager.Configure(IoCManager.Resolve<IKeyProvider>().GetKeyByName("hockeyapp.ios"));
            manager.DisableUpdateManager = true;
            manager.StartManager();
            manager.Authenticator.AuthenticateInstallation();

            // init forms and third party libraries
            CachedImageRenderer.Init();
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
