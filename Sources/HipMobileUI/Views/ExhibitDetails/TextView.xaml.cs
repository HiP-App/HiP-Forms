using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HipMobileUI.Navigation;
using HipMobileUI.ViewModels.Views.ExhibitDetails;
using Xamarin.Forms;

namespace HipMobileUI.Views.ExhibitDetails
{
    public partial class TextView : IViewFor<TextViewModel>
    {
        public TextView()
        {
            InitializeComponent();
        }
    }
}
