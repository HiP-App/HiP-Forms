using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HipMobileUI.Navigation;
using HipMobileUI.ViewModels.Pages;
using Xamarin.Forms;

namespace HipMobileUI.Pages
{
    public partial class MainPage : MasterDetailPage, IViewFor<MainPageViewModel>
    {

        private MainPageViewModel ViewModel => ((MainPageViewModel) BindingContext);

        public MainPage()
        {
            InitializeComponent();
            ViewModel.SelectedViewModel = ViewModel.MainScreenViewModels[0];
        }

        private void ListView_OnItemSelected (object sender, SelectedItemChangedEventArgs e)
        {
            if (Device.Idiom == TargetIdiom.Phone)
            {
                this.IsPresented = false;
            }
        }
    }
}
