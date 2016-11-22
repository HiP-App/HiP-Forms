using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HipMobileUI.Viewmodels.MainScreenContainables;
using Xamarin.Forms;

namespace HipMobileUI.Views
{
    public partial class TextView : ContentView
    {
        public TextView(string text)
        {
            InitializeComponent();
            ((TextViewmodel) BindingContext).Text = text;
        }
    }
}
