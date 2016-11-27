using HipMobileUI.Viewmodels;
using Xamarin.Forms;

namespace HipMobileUI.Pages
{
    public partial class MainPage : MasterDetailPage
    {
        public MainPage()
        {
            InitializeComponent();
            ((MainScreenViewmodel) BindingContext).SetStartItem ();
        }

        private void ListView_OnItemSelected (object sender, SelectedItemChangedEventArgs e)
        {
            this.IsPresented = false;
        }

    }
}
