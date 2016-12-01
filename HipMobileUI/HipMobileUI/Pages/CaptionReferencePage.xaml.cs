using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using de.upb.hip.mobile.pcl.BusinessLayer.InteractiveSources;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using HipMobileUI.Viewmodels.DialogViewmodels;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace HipMobileUI.Pages
{
    public partial class CaptionReferencePage : ContentPage {

        public CaptionReferencePage(List<Source> references)
        {
            InitializeComponent();
            ((CaptionReferenceViewmodel)BindingContext).Init(references);

        }
    }
}
