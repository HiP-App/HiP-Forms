using Foundation;
using System;
using CoreGraphics;
using UIKit;


namespace HiPMobile.iOS
{
    public partial class ExhibitTableViewCell : UITableViewCell
    {
        public static string key = "ExhibitCell";

        public ExhibitTableViewCell(IntPtr handle) : base(handle)
        {

        }

        public void SetUpimageAppearance()
        {
            this.ImageView.Layer.CornerRadius = this.Bounds.Height / 2;
            this.ImageView.Layer.MasksToBounds = true;
        }

        public void PopulateCell(UIImage image, string text)
        {
            UIImage smallImage = image.Scale(new CGSize(this.Frame.Size.Height, this.Frame.Size.Height));

            this.ImageView.Image = smallImage;
            nfloat n = this.ImageView.Frame.Height;
            SetUpimageAppearance();
            this.TextLabel.Text = text;
        }
    }
}