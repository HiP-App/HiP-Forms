using System.Diagnostics;
using System.Windows.Input;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views
{
    public class ProfileScreenViewModel : NavigationViewModel
    {

        private readonly MainPageViewModel mainPageViewModel;
        public ProfileScreenViewModel (MainPageViewModel mainPageVm)
        {
            mainPageViewModel = mainPageVm;

            Logout = new Command (LogoutDummy);
        }

        public ICommand Logout { get; }

        void LogoutDummy ()
        {
            Debug.WriteLine("##### LOGOUT #####");
            Settings.IsLoggedIn = false;
            mainPageViewModel.UpdateAccountViews ();
        }
    }
}
