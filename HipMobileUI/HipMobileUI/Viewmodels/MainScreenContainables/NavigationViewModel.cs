using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using HipMobileUI.Viewmodels.NavigationPageViewModels;
using Xamarin.Forms;

namespace HipMobileUI.Viewmodels.MainScreenContainables
{
    public class NavigationViewModel : BaseViewModel, INotifyPropertyChanged
    {

        public NavigationViewModel ()
        {
            NavigateTo = new Command (() => Navigation.PushAsync (new VmBindingViewModel (), true));
        }

        private ICommand _navigateTo;

        public ICommand NavigateTo {
            get { return _navigateTo; }
            set {
                if (_navigateTo == value)
                    return;
                _navigateTo = value;
                OnPropertyChanged ();
            }
        }

    }
}
