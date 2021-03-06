﻿// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
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
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.AudioPlayer;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.UI.DesignTime;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Location;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.NotificationPlayer;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels;
using System.IO;
using CarouselView.FormsPlugin.iOS;
using UIKit;
using UserNotifications;
using Xamarin.Forms;
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
            EarlyIoC.Register();

            var dataAccess = IoCManager.Resolve<IDataAccess>();

            if (Settings.ShouldDeleteDbOnLaunch)
            {
                File.Delete(dataAccess.DatabasePath);
                Settings.ShouldDeleteDbOnLaunch = false;
            }

            dataAccess.CreateDatabase(0); // ensures the database exists and is up to date

            IoCManager.RegisterType<IImageDimension, IosImageDimensions>();
            IoCManager.RegisterType<IAppCloser, IosAppCloser>();

            // Init Navigation
            NavigationService.Instance.RegisterViewModels(typeof(MainPage).Assembly);
            IoCManager.RegisterInstance(typeof(INavigationService), NavigationService.Instance);
            IoCManager.RegisterInstance(typeof(IViewCreator), NavigationService.Instance);

            // init other inversion of control classes
            IoCManager.RegisterType<IAudioPlayer, IosAudioPlayer>();
            IoCManager.RegisterInstance(typeof(INotificationPlayer), new IosNotificationPlayer());
            IoCManager.RegisterInstance(typeof(ILocationManager), new LocationManager());
            IoCManager.RegisterInstance(typeof(IKeyProvider), new IosKeyProvider());
            IoCManager.RegisterInstance(typeof(IBarsColorsChanger), new IosBarsColorsChanger());
            IoCManager.RegisterInstance(typeof(IDbChangedHandler), new DbChangedHandler());
            IoCManager.RegisterInstance(typeof(INetworkAccessChecker), new IosNetworkAccessChecker());
            IoCManager.RegisterInstance(typeof(IStorageSizeProvider), new IosStorageSizeProvider());

            // init crash manager
            var manager = BITHockeyManager.SharedHockeyManager;
            manager.Configure(IoCManager.Resolve<IKeyProvider>().GetKeyByName("hockeyapp.ios"));
            manager.DisableUpdateManager = true;
            manager.StartManager();
            manager.Authenticator.AuthenticateInstallation();

            // init forms and third party libraries
            CachedImageRenderer.Init();
            Xamarin.Forms.Forms.Init();
            CarouselViewRenderer.Init();

            DesignMode.IsEnabled = false;
            LoadApplication(new App());

#if ENABLE_TEST_CLOUD
            Xamarin.Calabash.Start();
#endif

            // display dialogue to ask for users permission to display notifications
            UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert, (approved, err) =>
            {
                // You could evaluate approval and error here
            });

            return base.FinishedLaunching(app, options);
        }

        public override void OnResignActivation(UIApplication uiApplication)
        {
            CallOnDisappearingForAllOpenPages();

            base.OnResignActivation(uiApplication);
        }

        private void CallOnDisappearingForAllOpenPages()
        {
            var tabController = Xamarin.Forms.Application.Current.MainPage as TabbedPage;
            var masterController = Xamarin.Forms.Application.Current.MainPage as MasterDetailPage;

            // First check to see if we're on a tabbed page, then master detail, finally go to overall fallback
            var nav = tabController?.CurrentPage?.Navigation ??
                      (masterController?.Detail as TabbedPage)?.CurrentPage?.Navigation ?? // special consideration for a tabbed page inside master/detail
                      masterController?.Detail?.Navigation ??
                      Xamarin.Forms.Application.Current.MainPage.Navigation;

            foreach (var page in nav.NavigationStack)
            {
                var navigationViewModel = page.BindingContext as NavigationViewModel;
                navigationViewModel?.OnDisappearing();
            }
        }
    }
}