using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExhibitRouteDownloadView : ContentPage, IViewFor<ExhibitRouteDownloadViewModel>
    {
        public ExhibitRouteDownloadView()
        {
            InitializeComponent();
        }

        protected override void OnAppearing ()
        {
            base.OnAppearing ();
            ((ExhibitRouteDownloadViewModel)BindingContext).StartDownload.Execute(null);
        }

    }
}