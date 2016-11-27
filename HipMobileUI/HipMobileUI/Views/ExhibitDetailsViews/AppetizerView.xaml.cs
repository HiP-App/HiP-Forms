using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using HipMobileUI.Viewmodels;
using Xamarin.Forms;

namespace HipMobileUI.Views.ExhibitDetailsViews
{
    public partial class AppetizerView : ContentView
    {
        public AppetizerView(AppetizerPage page)
        {
            InitializeComponent();
            ((AppetizerViewmodel)BindingContext).Init (page);
        }
    }
}
