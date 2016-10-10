using Foundation;
using UIKit;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.Common;
using de.upb.hip.mobile.pcl.Common.Contracts;
using de.upb.hip.mobile.pcl.DataAccessLayer;
using de.upb.hip.mobile.pcl.DataLayer;
using MediaToolbox;
using Microsoft.Practices.Unity;

namespace HiPMobile.iOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations

		public override UIWindow Window {
			get;
			set;
		}

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
            // Override point for customization after application launch.
            // If not required for your application you can safely delete this method

            // Move this block to the Launch screen
            IoCManager.UnityContainer.RegisterType<IDataAccess, RealmDataAccess>();
            IoCManager.UnityContainer.RegisterType<IDataLoader, EmbeddedResourceDataLoader>();
            IoCManager.UnityContainer.RegisterType<IImageDimension, IosImageDimensions>();
            DbManager.UpdateDatabase();
            // End of block

            //setup default colors for all navigation bars in the app
            UINavigationBar.Appearance.BarTintColor = UIColor.FromRGB(25, 32, 71);
            UINavigationBar.Appearance.TintColor = UIColor.White;
            //UINavigationBar.Appearance.TitleTextAttributes = new UIStringAttributes();
            //UINavigationBar.Appearance.TitleTextAttributes.ForegroundColor = UIColor.Purple;

            return true;
		}

		public override void OnResignActivation (UIApplication application)
		{
			// Invoked when the application is about to move from active to inactive state.
			// This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
			// or when the user quits the application and it begins the transition to the background state.
			// Games should use this method to pause the game.
		}

		public override void DidEnterBackground (UIApplication application)
		{
			// Use this method to release shared resources, save user data, invalidate timers and store the application state.
			// If your application supports background exection this method is called instead of WillTerminate when the user quits.
		}

		public override void WillEnterForeground (UIApplication application)
		{
			// Called as part of the transiton from background to active state.
			// Here you can undo many of the changes made on entering the background.
		}

		public override void OnActivated (UIApplication application)
		{
			// Restart any tasks that were paused (or not yet started) while the application was inactive. 
			// If the application was previously in the background, optionally refresh the user interface.
		}

		public override void WillTerminate (UIApplication application)
		{
			// Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
		}
	}
}


