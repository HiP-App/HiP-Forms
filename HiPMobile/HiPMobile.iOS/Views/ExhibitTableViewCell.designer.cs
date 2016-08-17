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
    [Register ("ExhibitTableViewCell")]
    partial class ExhibitTableViewCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView exhibitImageView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel exhibitName { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (exhibitImageView != null) {
                exhibitImageView.Dispose ();
                exhibitImageView = null;
            }

            if (exhibitName != null) {
                exhibitName.Dispose ();
                exhibitName = null;
            }
        }
    }
}