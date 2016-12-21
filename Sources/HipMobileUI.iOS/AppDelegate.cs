﻿using de.upb.hip.mobile.pcl.Common;
using de.upb.hip.mobile.pcl.Common.Contracts;
using Foundation;
using HipMobileUI.iOS.Contracts;
using HipMobileUI.Navigation;
using UIKit;
using Microsoft.Practices.Unity;

namespace HipMobileUI.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
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
            IoCManager.UnityContainer.RegisterType<IImageDimension, IosImageDimensions>();

            // Init Navigation
            //NavigationService.Instance.RegisterViewModels (typeof(MainPage).Assembly);
            IoCManager.UnityContainer.RegisterInstance (typeof(INavigationService), NavigationService.Instance);

            global::Xamarin.Forms.Forms.Init();
            Xamarin.FormsMaps.Init();
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}