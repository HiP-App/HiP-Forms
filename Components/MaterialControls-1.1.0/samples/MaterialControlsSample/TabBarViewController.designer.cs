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
	[Register ("TabBarViewController")]
	partial class TabBarViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MaterialControls.MDTabBar tabBar { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (tabBar != null) {
				tabBar.Dispose ();
				tabBar = null;
			}
		}
	}
}
