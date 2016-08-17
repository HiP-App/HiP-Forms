using Foundation;
using System;
using CoreGraphics;
using UIKit;

namespace HiPMobile.iOS
{
    public partial class ExhibitTableViewCell : UITableViewCell
    {
        public static string key = "exhibitCell";

        public ExhibitTableViewCell (IntPtr handle) : base (handle)
        {
            
        }

        public void SetUpimageAppearance()
        {
            exhibitImageView.Layer.CornerRadius = 25;
            exhibitImageView.Layer.MasksToBounds = true;
            exhibitImageView.Image = UIImage.FromFile("hiphop.jpg");

        }

        public void Image(UIImage image)
        {
            exhibitImageView.Image = image;
        }

        public void Text(string text)
        {
            exhibitName.Text = text;
        }

    }
}