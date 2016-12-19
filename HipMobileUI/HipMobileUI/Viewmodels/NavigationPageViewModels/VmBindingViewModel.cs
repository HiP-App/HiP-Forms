using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace HipMobileUI.Viewmodels.NavigationPageViewModels
{
    public class VmBindingViewModel : BaseViewModel, INotifyPropertyChanged {

        private bool state = false;

        public VmBindingViewModel ()
        {
            this.SwitchSubview = new Command (Execute);
            Subview = new ColorViewModel(Color.Aqua);
        }

        private void Execute ()
        {
            if (state)
            {
                Subview = new ColorViewModel (Color.Aqua);
            }
            else
            {
                Subview = new ColorViewModel (Color.Green);
            }
            state = !state;
        }

        private BaseViewModel _subview;

        public BaseViewModel Subview {
            get { return _subview; }
            set {
                if (_subview == value)
                    return;
                _subview = value;
                OnPropertyChanged ();
            }
        }

        private ICommand _switchSubview;

        public ICommand SwitchSubview {
            get { return _switchSubview; }
            set {
                if (_switchSubview == value)
                    return;
                _switchSubview = value;
                OnPropertyChanged ();
            }
        }


    }
}
