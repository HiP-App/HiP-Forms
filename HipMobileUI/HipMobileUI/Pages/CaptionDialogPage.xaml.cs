using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using de.upb.hip.mobile.pcl.BusinessLayer.InteractiveSources;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using Xamarin.Forms;

namespace HipMobileUI.Pages
{
    public partial class CaptionDialogPage : TabbedPage {

        private readonly Page captionTextPage;
        private readonly Page captionReferencePage;

        public CaptionDialogPage(int number)
        {
            //InitializeComponent();
            ToolbarItem tbi = new ToolbarItem();
            tbi.Icon = "icon.png";
            ToolbarItems.Add(tbi);

            if (number == 1)
            {
               DisplayAlertDialog ();
            }
            else
            {
                
                var id = ExhibitManager.GetExhibitSet().Last();
                var subtitles = id.Pages[1].Audio.Caption;

                var parser = new InteractiveSourcesParser(subtitles, new ConsecutiveNumberAndConstantInteractiveSourceSubstitute(1, "Quelle"));

                string formatedText = parser.TextWithSubstitutes;
                List<Source> references = parser.Sources;
                
                captionTextPage = new CaptionTextPage (formatedText);
                captionReferencePage = new CaptionReferencePage(references);

                captionTextPage.Title = "Caption text";
                captionReferencePage.Title = "References";

                Children.Add (captionTextPage);
                Children.Add (captionReferencePage);
               
            }
        }

        async void DisplayAlertDialog ()
        {
 
            string id = ExhibitManager.GetExhibitSet().Last().Id;

            await Task.Delay(1000);

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
            CurrentPage = captionReferencePage;
        }
    }
}
