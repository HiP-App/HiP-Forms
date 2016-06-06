using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using MaterialControls;

namespace MaterialControlsSample
{
	partial class TabBarViewController : UIViewController
	{
		public TabBarViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			NSObject[] items = { new NSString("ITEM ONE"), new NSString("ITEM TWO"), new NSString("NEXT ITEM") };
			tabBar.SetItems (items);
		}
	}
}
