using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using HipMobileUI.Annotations;
using HipMobileUI.Helpers;
using Xamarin.Forms;
using Image = de.upb.hip.mobile.pcl.BusinessLayer.Models.Image;

namespace HipMobileUI.Viewmodels.ExhibitDetailsViewmodels
{
    class ImageViewmodel : INotifyPropertyChanged
    {

        public void Init (ImagePage page)
        {
            this.Image = page.Image.GetImageSource ();
            this.Description = page.Image.Description;
        }

        private ImageSource image;
        private string description;

        public ImageSource Image {
            get { return image; }
            set {
                image = value;
                OnPropertyChanged ();
            }
        }

        public string Description {
            get { return description; }
            set {
                description = value;
                OnPropertyChanged ();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged ([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke (this, new PropertyChangedEventArgs (propertyName));
        }

    }
}
