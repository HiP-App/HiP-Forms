using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using HiPMobileUI.Viewmodels;
using Xamarin.Forms;

namespace HipMobileUI.Views.ExhibitDetailsViews
{
    public partial class AppetizerView : ContentView
    {
        public AppetizerView(AppetizerPage page)
        {
            InitializeComponent();
            ((AppetizerViewmodel)BindingContext).Init (page);
        }
    }
}
