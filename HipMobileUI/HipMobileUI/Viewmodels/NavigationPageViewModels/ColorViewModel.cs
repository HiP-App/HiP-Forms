using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HipMobileUI.Viewmodels.NavigationPageViewModels
{
    public class ColorViewModel : BaseViewModel
    {

        public ColorViewModel ()
        {
        }

        public ColorViewModel (Color c)
        {
            this.Color = c;
        }

        private Color _color;

        public Color Color {
            get { return _color; }
            set {
                if (_color == value)
                    return;
                _color = value;
                OnPropertyChanged ();
            }
        }

    }
}
