// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using MaterialControls;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace MaterialControlsSample
{
	[Register ("ProgressViewController")]
	partial class ProgressViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MaterialControls.MDProgress progressDeterminate { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MaterialControls.MDProgress progressIndeterminate { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MaterialControls.MDProgress progressLinearDeterminate { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MaterialControls.MDProgress progressLinearIndeterminate { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (progressDeterminate != null) {
				progressDeterminate.Dispose ();
				progressDeterminate = null;
			}
			if (progressIndeterminate != null) {
				progressIndeterminate.Dispose ();
				progressIndeterminate = null;
			}
			if (progressLinearDeterminate != null) {
				progressLinearDeterminate.Dispose ();
				progressLinearDeterminate = null;
			}
			if (progressLinearIndeterminate != null) {
				progressLinearIndeterminate.Dispose ();
				progressLinearIndeterminate = null;
			}
		}
	}
}
