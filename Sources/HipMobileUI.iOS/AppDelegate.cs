using de.upb.hip.mobile.pcl.Common;
using de.upb.hip.mobile.pcl.Common.Contracts;
using FFImageLoading.Forms.Touch;
using Foundation;
using HipMobileUI.AudioPlayer;
using HipMobileUI.iOS.Contracts;
using HipMobileUI.Navigation;
using HipMobileUI.Pages;
using UIKit;
using Xamarin.Forms;

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
            IoCManager.RegisterInstance (typeof(IAudioPlayer), new IosAudioPlayer ());

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
