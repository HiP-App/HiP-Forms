using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using HipMobileUI.Viewmodels.ExhibitDetailsViewmodels;
using Xamarin.Forms;

namespace HipMobileUI.Views
{
    public partial class ImageView : ContentView
    {
        public ImageView(ImagePage page)
        {
            InitializeComponent();
            ((ImageViewmodel)BindingContext).Init (page);
        }
    }
}
