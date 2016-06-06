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
	[Register ("SnackBarViewController")]
	partial class SnackBarViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MaterialControls.MDButton buttonShow { get; set; }

		[Action ("handleButtonShow:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void handleButtonShow (MaterialControls.MDButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (buttonShow != null) {
				buttonShow.Dispose ();
				buttonShow = null;
			}
		}
	}
}
