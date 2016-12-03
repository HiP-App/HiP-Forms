using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HipMobileUI.Viewmodels.MainScreenContainables;
using Xamarin.Forms;

namespace HipMobileUI.Views.MainScreenViews
{
    public partial class StressTestView : ContentView
    {
        public StressTestView()
        {
            InitializeComponent();
            ((StressTestViewModel)BindingContext).Init (this.Navigation);
        }
    }
}
