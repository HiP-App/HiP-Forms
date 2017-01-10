using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HipMobileUI.Map;
using HipMobileUI.Navigation;
using Xamarin.Forms;

namespace HipMobileUI.Views
{
    public partial class MapView : IViewFor<MapViewModel>
    {
        public MapView()
        {
            InitializeComponent();
        }
    }
}
