// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace MaterialControlsSample
{
	[Register ("ToastViewController")]
	partial class ToastViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MaterialControls.MDButton buttonLongLineToast { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MaterialControls.MDButton buttonSingleLineToast { get; set; }

		[Action ("handleLongLineToast:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void handleLongLineToast (MaterialControls.MDButton sender);

		[Action ("handleSingleLineToast:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void handleSingleLineToast (MaterialControls.MDButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (buttonLongLineToast != null) {
				buttonLongLineToast.Dispose ();
				buttonLongLineToast = null;
			}
			if (buttonSingleLineToast != null) {
				buttonSingleLineToast.Dispose ();
				buttonSingleLineToast = null;
			}
		}
	}
}
