using Foundation;
using System;
using UIKit;

namespace HiPMobile.iOS
{
    public partial class MenuTableViewCell : UITableViewCell
    {
        public static string key = "MenuCell";
        public MenuTableViewCell(IntPtr handle) : base(handle)
        {
        }

        public void InitCell(string labelText, string imageName)
        {
            ImageView.Image = UIImage.FromBundle(imageName);
            TextLabel.Text = labelText;
        }

    }
}