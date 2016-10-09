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

namespace HiPMobile.iOS
{
    [Register ("MainViewController")]
    partial class MainViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView containerView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem menuBarButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView menuTableView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UINavigationBar navigationBar { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UINavigationItem navigationItem { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView shadowView { get; set; }

        [Action ("TapMenuBarButton:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void TapMenuBarButton (UIKit.UIBarButtonItem sender);

        void ReleaseDesignerOutlets ()
        {
            if (containerView != null) {
                containerView.Dispose ();
                containerView = null;
            }

            if (menuBarButton != null) {
                menuBarButton.Dispose ();
                menuBarButton = null;
            }

            if (menuTableView != null) {
                menuTableView.Dispose ();
                menuTableView = null;
            }

            if (navigationBar != null) {
                navigationBar.Dispose ();
                navigationBar = null;
            }

            if (navigationItem != null) {
                navigationItem.Dispose ();
                navigationItem = null;
            }

            if (shadowView != null) {
                shadowView.Dispose ();
                shadowView = null;
            }
        }
    }
}