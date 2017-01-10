using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using HipMobileUI.Helpers;
using Xamarin.Forms;

namespace HipMobileUI.ViewModels.Pages.ExhibitDetails
{
    class AppetizerViewModel : ExhibitSubviewViewModel
    {

        private ImageSource image;
        private string text;

        public AppetizerViewModel (AppetizerPage page)
        {
            if (page != null)
            {
                this.Text = page.Text;

                // workaround for realm bug
                var imageData = page.Image.Data;
                Image = ImageSource.FromStream (() => new MemoryStream (imageData));
            }
        }

        public ImageSource Image {
            get { return image; }
            set { SetProperty (ref image, value); }
        }

        public string Text {
            get { return text; }
            set { SetProperty (ref text, value); }
        }

    }
}
