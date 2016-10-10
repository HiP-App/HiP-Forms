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
    [Register ("HomeScreenViewController")]
    partial class HomeScreenViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView exhibitsTableView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        HiPMobile.iOS.OsmMapView mapView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (exhibitsTableView != null) {
                exhibitsTableView.Dispose ();
                exhibitsTableView = null;
            }

            if (mapView != null) {
                mapView.Dispose ();
                mapView = null;
            }
        }
    }
}