using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HipMobileUI.Navigation;
using HipMobileUI.Viewmodels.MainScreenContainables;
using HipMobileUI.Viewmodels.NavigationPageViewModels;
using Xamarin.Forms;

namespace HipMobileUI.Pages
{
    public partial class ViewModelNavigationPage : ContentPage, IViewFor<VmBindingViewModel>
    {
        public ViewModelNavigationPage()
        {
            InitializeComponent();
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (VmBindingViewModel)value; }
        }

        public VmBindingViewModel ViewModel
        {
            get { return (VmBindingViewModel)BindingContext; }
            set { BindingContext = value; }
        }
    }
}
