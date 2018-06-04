using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExhibitDetailsPage2 : ContentPage, IViewFor<ExhibitDetailsPage2ViewModel>
    {
        public ExhibitDetailsPage2()
        {
            InitializeComponent();
        }
    }
}