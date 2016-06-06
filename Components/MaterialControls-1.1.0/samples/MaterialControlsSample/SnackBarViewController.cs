using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using MaterialControls;

namespace MaterialControlsSample
{
	partial class SnackBarViewController : UIViewController
	{
		int count = 1;

		public SnackBarViewController (IntPtr handle) : base (handle)
		{
		}

		partial void handleButtonShow (MaterialControls.MDButton sender)
		{
			string message = String.Format("Connection time out. Test for long message {0}", count++);
			MDSnackbar snackbar = new MDSnackbar(message, "retry");
			snackbar.ActionTitleColor = UIColorHelper.ColorWithRGBA("#4CAF50");
			snackbar.Multiline = (count % 2 == 0);
			snackbar.Show();
		}
	}
}
