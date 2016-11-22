using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HipMobileUI.Viewmodels;
using Xamarin.Forms;

namespace HipMobileUI.Views
{
    public partial class ColorView : ContentView
    {
        public ColorView(Color color)
        {
            InitializeComponent();
            ((ColorViewmodel) BindingContext).Color = color;
        }
    }
}
