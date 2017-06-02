using System.Diagnostics;
using System.Windows.Input;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views
{
    public class AccountScreenViewModel : NavigationViewModel
    {

        private readonly MainPageViewModel mainPageViewModel;
        public AccountScreenViewModel(MainPageViewModel mainPageVm)
        {
            mainPageViewModel = mainPageVm;

            Login = new Command(LoginDummy);
        }

        public ICommand Login { get; }

        void LoginDummy()
        {
            Settings.IsLoggedIn = true;
            mainPageViewModel.UpdateAccountViews();
        }
    }
}
