using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using HiPMobileUI.Pages;
using Xamarin.Forms;

namespace HipMobileUI.Pages
{
    public partial class CaptionDialogPage : TabbedPage {

        private readonly Page captionTextPage;
        private readonly Page captionLinkPage;

        public CaptionDialogPage(int number)
        {
            //InitializeComponent();
            if (number == 1)
            {
                DisplayAlertDialog ();
            }
            else
            {

                captionTextPage = new CaptionTextPage ();
                captionLinkPage = new CaptionLinkPage ();

                captionTextPage.Title = "Caption text";
                captionLinkPage.Title = "References";

                Children.Add (captionTextPage);
                Children.Add (captionLinkPage);
            }
        }

        async void DisplayAlertDialog ()
        {
            string id = ExhibitManager.GetExhibitSet().Last().Id;

            var answer = await this.DisplayAlert(
                "Hint message",
                "The audio automatically starts when new page is opened. You can change this behaviour in Menu>>Settings",
                "Some long German word for yes",
                "Some long German word for no");
            if(answer)
                await Navigation.PushAsync(new ExhibitDetailsPage(id));
        }

        public void SwitchToCaptionPage()
        {
            CurrentPage = captionTextPage;
        }

        public void SwitchToLinkPage()
        {
            CurrentPage = captionLinkPage;
        }
    }
}
