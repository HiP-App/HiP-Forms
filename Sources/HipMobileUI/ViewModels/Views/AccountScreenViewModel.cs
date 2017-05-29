using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views
{
    public class AccountScreenViewModel : NavigationViewModel
    {

        public AccountScreenViewModel ()
        {
            Settings.IsLoggedIn = false;
            Settings.IsLoggedIn = true;
            if (Settings.IsLoggedIn)
            {
                SwitchToProfileScreen ();
            }
            else
            {
                // Insert your registration / login code here
            }

        }

        public async void SwitchToProfileScreen ()
        {
            await Navigation.PushModalAsync (new ProfilePageViewModel ());
        }
    }

}
