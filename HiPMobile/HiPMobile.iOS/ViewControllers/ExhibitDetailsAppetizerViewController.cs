using Foundation;
using System;
using UIKit;
using MaterialControls;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;

namespace HiPMobile.iOS
{
    public partial class ExhibitDetailsAppetizerViewController : UIViewController
    {
        public string ExhibitID { get; set; }
        public string ExhibitTitle { get; set; }
        public Exhibit Exhibit { get; set; }
        public AppetizerPage ExhibitAppetizer { get; set; }

        public ExhibitDetailsAppetizerViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Exhibit = ExhibitManager.GetExhibit(ExhibitID);
            ExhibitAppetizer = Exhibit.Pages[0].AppetizerPage; // each exhibit page list should have for first element an appetizer page
            if (ExhibitAppetizer != null)
            {
                NavigationItem.Title = ExhibitTitle;
                NSData imageData = NSData.FromArray(ExhibitAppetizer.Image.Data);
                appetizerImageView.Image = new UIImage(imageData);

                var titleAttributes = new UIStringAttributes
                {
                    Font = UIFont.BoldSystemFontOfSize(13)
                };

                NSMutableAttributedString attributedString = new NSMutableAttributedString(ExhibitTitle + "\n\n" + ExhibitAppetizer.Text);
                attributedString.SetAttributes(titleAttributes, new NSRange(0, ExhibitTitle.Length));
                appetizerTextView.AttributedText = attributedString;
            }
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            
            if (segue.Identifier != null && segue.Identifier.Equals("ForwardToExhibitDetailsSegue"))
            {
                ExhibitDetailsViewController exhibitDetailsViewContrroller = segue.DestinationViewController as ExhibitDetailsViewController;
                exhibitDetailsViewContrroller.Exhibit = Exhibit;
            }
           
        }

    }
}