using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using de.upb.hip.mobile.pcl.BusinessLayer.InteractiveSources;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using HipMobileUI.Viewmodels.ExhibitDetailsViewmodels;
using Xamarin.Forms;

namespace HipMobileUI.Pages
{
    public partial class CaptionTextPage : ContentPage {

        public CaptionTextPage(string formatedText)
        {
            InitializeComponent();
            ((CaptionTextViewmodel)BindingContext).Init(formatedText);
        }


        async void SwitchToOtherPage (object o, EventArgs e)
        {
            var tabbedPage = this.Parent as CaptionDialogPage;
            tabbedPage.SwitchToLinkPage ();
        }
    }
}
