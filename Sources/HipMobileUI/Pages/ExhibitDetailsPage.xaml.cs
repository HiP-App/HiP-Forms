using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExhibitDetailsPage : OrientationContentPage, IViewFor<ExhibitDetailsPageViewModel>
    {
        public ExhibitDetailsPage()
        {
            InitializeComponent();
        }
    }
}