using System.Windows.Input;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using Xamarin.Forms;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.User;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.UserManagement;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using Acr.UserDialogs;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views
{
    public class LoginScreenViewModel : ExtendedNavigationViewModel
    {
        private readonly MainPageViewModel mainPageViewModel;
        private string email;
        private string password;
        private string errorMessage;

        public LoginScreenViewModel(MainPageViewModel mainPageVm)
        {
            mainPageViewModel = mainPageVm;

            LoginCommand = new Command(OnLoginClicked);
            RegisterCommand = new Command(OnRegisterClicked);
            ForgotPasswordCommand = new Command(OnForgotPasswordClicked);
        }

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }
        public ICommand ForgotPasswordCommand { get; }

        private void OnLoginClicked()
        {
            if (string.IsNullOrWhiteSpace(Email) && string.IsNullOrWhiteSpace(Password))
            {
                DisplayEmptyEmailAndPasswordErrorMessage();
            }
            else if (string.IsNullOrWhiteSpace(Email))
            {
                DisplayEmptyEmailErrorMessage();
            }
            else if (string.IsNullOrWhiteSpace(Password))
            {
                DisplayEmptyPasswordErrorMessage();
            }
            else
            {
                PerformLogin();
            }
        }

        private void OnRegisterClicked()
        {
            GoToRegisterScreenView();
        }

        private void OnForgotPasswordClicked()
        {
            GoToForgotPasswordScreenView();
        }

        private async void PerformLogin()
        {
            UserDialogs.Instance.ShowLoading(Strings.LoginScreenView_Dialog_Login, MaskType.Black);
            UserStatus userStatus = await IoCManager.Resolve<IUserManager>().Login(new User(email, password));
            UserDialogs.Instance.HideLoading();

            switch (userStatus)
            {
                case UserStatus.InCorrectUserNameandPassword:
                    DisplayWrongCredentialsErrorMessage();
                    break;
                case UserStatus.LoggedIn:
                    Settings.IsLoggedIn = true;
                    break;
                case UserStatus.NetworkConnectionFailed:
                    UserDialogs.Instance.Alert(Strings.Alert_No_Internet_Description, Strings.Alert_No_Internet_Title);
                    break;
                case UserStatus.ServerError:
                    UserDialogs.Instance.Alert(Strings.Alert_Server_Error_Description, Strings.Alert_Server_Error_Title);
                    break;
                case UserStatus.UnkownError:
                    UserDialogs.Instance.Alert(Strings.Alert_Unknown_Error_Description, Strings.Alert_Unknown_Error_Title);
                    break;
            }

            mainPageViewModel.UpdateAccountViews();
        }

        private void GoToRegisterScreenView()
        {
            mainPageViewModel.SwitchToRegisterView();
        }

        private void GoToForgotPasswordScreenView()
        {
            mainPageViewModel.SwitchToForgotPasswordView();
        }

        private void DisplayWrongCredentialsErrorMessage()
        {
            ErrorMessage = Strings.LoginScreenView_Error_Wrong_Credentials;
        }

        private void DisplayEmptyEmailErrorMessage()
        {
            ErrorMessage = Strings.LoginScreenView_Error_Empty_Email;
        }

        private void DisplayEmptyPasswordErrorMessage()
        {
            ErrorMessage = Strings.LoginScreenView_Error_Empty_Password;
        }

        private void DisplayEmptyEmailAndPasswordErrorMessage()
        {
            ErrorMessage = Strings.LoginScreenView_Error_Empty_Email_And_Password;
        }

        public string ErrorMessage
        {
            get { return errorMessage; }
            set { SetProperty(ref errorMessage, value); }
        }

        public string Email
        {
            get { return email; }
            set { SetProperty(ref email, value); }
        }

        public string Password
        {
            get { return password; }
            set { SetProperty(ref password, value); }
        }
    }
}