using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using MaterialControls;

namespace MaterialControlsSample
{
	partial class ToastViewController : UIViewController
	{
		public ToastViewController (IntPtr handle) : base (handle)
		{
		}

		partial void handleLongLineToast (MaterialControls.MDButton sender)
		{
			var toast = new MDToast("Attention! This is a very very very long long text message", 1.0);
			toast.Show();
		}

		partial void handleSingleLineToast (MaterialControls.MDButton sender)
		{
			var toast = new MDToast("Toast test message", 1.0);
			toast.Show();
		}
	}
}
