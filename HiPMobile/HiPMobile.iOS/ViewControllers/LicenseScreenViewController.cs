using Foundation;
using System;
using CoreGraphics;
using UIKit;

namespace HiPMobile.iOS
{
    public partial class LicenseScreenViewController : UIViewController {

        private int NumberOfLinebreaks = 2;

        public LicenseScreenViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

            for (int i = 0; i < 30; i++)
            {
                AddLicense("Dies ist ein Test mit einem Link: www.google.de");
            }
            LicenseTextView.FlashScrollIndicators ();
        }


        public override void ViewDidLayoutSubviews ()
        {
            base.ViewDidLayoutSubviews ();

            // force the view to scroll to the top
            LicenseTextView.SetContentOffset (CGPoint.Empty, false);
        }

        private void AddLicense (string text)
        {
            if (!string.IsNullOrEmpty (LicenseTextView.Text))
            {
                for (var i = 0; i < NumberOfLinebreaks; i++)
                {
                    this.LicenseTextView.Text += Environment.NewLine;
                }
            }
            this.LicenseTextView.Text += text;
        }

    }
}