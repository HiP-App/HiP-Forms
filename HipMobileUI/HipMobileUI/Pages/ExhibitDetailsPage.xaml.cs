using HipMobileUI.Viewmodels;
using Xamarin.Forms;

namespace HipMobileUI.Pages
{
    public partial class ExhibitDetailsPage : ContentPage {

        public ExhibitDetailsPage(string exhibitId)
        {
            InitializeComponent();
            ((ExhibitDetailsViewmodel)this.BindingContext).Init (exhibitId);
        }
    }
}
