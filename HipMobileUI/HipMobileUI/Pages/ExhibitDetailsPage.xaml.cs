using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiPMobileUI.Viewmodels;
using Xamarin.Forms;

namespace HiPMobileUI.Pages
{
    public partial class ExhibitDetailsPage : ContentPage {

        public ExhibitDetailsPage(string exhibitId)
        {
            InitializeComponent();
            ((ExhibitDetailsViewmodel)this.BindingContext).Init (exhibitId);
        }
    }
}
