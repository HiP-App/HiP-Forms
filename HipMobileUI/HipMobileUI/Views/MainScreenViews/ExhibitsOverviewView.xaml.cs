using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HipMobileUI.Viewmodels.MainScreenContainables;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace HipMobileUI.Views
{
    public partial class ExhibitsOverviewView : ContentView
    {
        public ExhibitsOverviewView(string exhibitSetId)
        {
            InitializeComponent();
            ((ExhibitsOverviewViewmodel)BindingContext).Init (exhibitSetId, Navigation);
        }
    }
}
