using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HipMobileUI.Navigation;
using HipMobileUI.Viewmodels.NavigationPageViewModels;
using Xamarin.Forms;

namespace HipMobileUI.Views
{
    public partial class VmBoundColorView : ContentView, IViewFor<ColorViewModel>
    {
        public VmBoundColorView()
        {
            InitializeComponent();
        }

        object IViewFor.ViewModel {
            get { return ViewModel; }
            set { ViewModel = (ColorViewModel)value; }
        }

        public ColorViewModel ViewModel {
            get { return (ColorViewModel) BindingContext; }
            set { BindingContext = value; }
        }

    }
}
