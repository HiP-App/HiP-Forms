using HipMobileUI.Viewmodels;
using Xamarin.Forms;

namespace HipMobileUI.Pages
{
    public partial class ExhibitDetailsPage : ContentPage {

        public ExhibitDetailsPage(string exhibitId)
        {
            InitializeComponent();
            ((ExhibitDetailsViewmodel)this.BindingContext).Init (exhibitId);
        }

        private void PanGestureRecognizer_OnPanUpdated (object sender, PanUpdatedEventArgs e)
        {
            /*if (e.StatusType == GestureStatus.Running)
            {
                if (e.TotalX > 0 && previousButton.IsEnabled)
                {
                    previousButton.Command.Execute (null);
                }
                else if (e.TotalX < 0 && nextButton.IsEnabled)
                {
                    nextButton.Command.Execute(null);
                }
            }*/
        }

    }
}
