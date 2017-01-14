using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using Xamarin.Forms;

namespace HipMobileUI.ViewModels.Pages.ExhibitDetails
{
    class ImageViewModel : ExhibitSubviewViewModel
    {

        private ImageSource image;
        private string description;
        private string headline;

        public ImageViewModel (ImagePage page)
        {
            var data = page.Image.Data;
            Image = ImageSource.FromStream (() => new MemoryStream (data));
            Headline = page.Image.Title;
            Description = page.Image.Description;
        }

        public ImageSource Image {
            get { return image; }
            set { SetProperty (ref image, value); }
        }

        public string Headline {
            get { return headline; }
            set { SetProperty (ref headline, value); }
        }

        public string Description {
            get { return description; }
            set { SetProperty (ref description, value); }
        }

    }
}
