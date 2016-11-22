using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HipMobileUI.Viewmodels
{
    class ColorViewmodel: INotifyPropertyChanged
    {

        public ColorViewmodel ()
        {
            Color = Color.Default;
        }

        public ColorViewmodel(Color color)
        {
            Color = color;
        }

        private Color color;

        public Color Color {
            get { return color; }
            set {
                color = value;
                OnPropertyChanged ();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged ([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke (this, new PropertyChangedEventArgs (propertyName));
        }

    }
}
