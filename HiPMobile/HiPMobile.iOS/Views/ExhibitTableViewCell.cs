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

        public void SetUpimageAppearance(nfloat cornerRadius)
        {
            this.ImageView.Layer.CornerRadius = cornerRadius;
            this.ImageView.Layer.MasksToBounds = true;
            //exhibitImageView.Image = UIImage.FromFile("hiphop.jpg");
        }

        public void PopulateCell(UIImage image, string text)
        {
           UIImage smallImage = image.Scale(new CGSize(this.Frame.Size.Height, this.Frame.Size.Height));
            
            this.ImageView.Image = smallImage;
            nfloat n = this.ImageView.Frame.Height;
            SetUpimageAppearance(22);
            this.TextLabel.Text = text;
          }
    }
}