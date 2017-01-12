using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using HipMobileUI.Map;
using HipMobileUI.Navigation;
using HipMobileUI.ViewModels.Pages;
using Xamarin.Forms;

namespace HipMobileUI.Pages
{
    public partial class MainPage : IViewFor<MainPageViewModel>
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
                IsPresented = false;
            }
        }
    }
}
