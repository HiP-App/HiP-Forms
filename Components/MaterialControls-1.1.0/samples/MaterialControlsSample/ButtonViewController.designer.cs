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
	[Register ("ButtonViewController")]
	partial class ButtonViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MaterialControls.MDButton buttonFlat { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MaterialControls.MDButton buttonFloat { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MaterialControls.MDButton buttonRaised { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (buttonFlat != null) {
				buttonFlat.Dispose ();
				buttonFlat = null;
			}
			if (buttonFloat != null) {
				buttonFloat.Dispose ();
				buttonFloat = null;
			}
			if (buttonRaised != null) {
				buttonRaised.Dispose ();
				buttonRaised = null;
			}
		}
	}
}
