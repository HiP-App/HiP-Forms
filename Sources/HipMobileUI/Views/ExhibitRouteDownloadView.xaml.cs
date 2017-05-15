using PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Views {
    [XamlCompilation (XamlCompilationOptions.Compile)]
    public partial class ExhibitRouteDownloadView : ContentPage, IViewFor<ExhibitRouteDownloadViewModel> {

        private DeviceOrientation deviceOrientation;

        public ExhibitRouteDownloadView ()
        {
            InitializeComponent ();
            deviceOrientation = DeviceOrientation.Undefined;
        }

        protected override void OnSizeAllocated (double width, double height)
        {
            base.OnSizeAllocated (width, height);
            if (width <= height)
            {
                if (deviceOrientation != DeviceOrientation.Portrait)
                {
                    Image.IsVisible = true;
                }
                deviceOrientation = DeviceOrientation.Portrait;
            }
            else if (width > height)
            {
                if (deviceOrientation != DeviceOrientation.Landscape)
                {
                    Image.IsVisible = false;
                }
                deviceOrientation = DeviceOrientation.Landscape;
            }
        }

    }
}