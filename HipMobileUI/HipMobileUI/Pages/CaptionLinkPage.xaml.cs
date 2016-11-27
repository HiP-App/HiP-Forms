using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using de.upb.hip.mobile.pcl.BusinessLayer.InteractiveSources;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using HipMobileUI.Viewmodels.DialogViewmodels;
using Xamarin.Forms;

namespace HipMobileUI.Pages
{
    public partial class CaptionLinkPage : ContentPage {

        public CaptionLinkPage(List<Source> references)
        {
            InitializeComponent();
            ((CaptionLinksViewmodel)BindingContext).Init(references);
        }
    }
}
