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

namespace HiPMobile.iOS
{
    [Register ("ExhibitDetailsAppetizerViewController")]
    partial class ExhibitDetailsAppetizerViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView appetizerImageView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView appetizerTextView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MaterialControls.MDButton forwardButton { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (appetizerImageView != null) {
                appetizerImageView.Dispose ();
                appetizerImageView = null;
            }

            if (appetizerTextView != null) {
                appetizerTextView.Dispose ();
                appetizerTextView = null;
            }

            if (forwardButton != null) {
                forwardButton.Dispose ();
                forwardButton = null;
            }
        }
    }
}