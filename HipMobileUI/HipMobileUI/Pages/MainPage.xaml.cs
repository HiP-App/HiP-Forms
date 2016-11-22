using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
