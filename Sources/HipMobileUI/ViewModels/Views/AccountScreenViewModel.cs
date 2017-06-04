using System.Diagnostics;
using System.Windows.Input;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
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
			Register = new Command(RegisterDummy);
        }

        public ICommand Login { get; }
		public ICommand Register{get; }

        void LoginDummy()
        {
            Debug.WriteLine("##### LOGIN #####");
            Settings.IsLoggedIn = true;
            mainPageViewModel.UpdateAccountViews();
        }
		void RegisterDummy()
		{
			Debug.WriteLine("##### REGISTER #####");
			IoCManager.Resolve<INavigationService> ().PushAsync(new RegisterViewModel());
		}
    }
}
