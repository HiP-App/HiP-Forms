using System.Diagnostics;
using System.Windows.Input;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using Xamarin.Forms;
using System;
using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.User;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.UserManagement;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.Exceptions;
using Acr.UserDialogs;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views
{
    public class LoginScreenViewModel : NavigationViewModel
    {

        private readonly MainPageViewModel mainPageViewModel;
        private String email;
        private String password;
        private String errorMessage;

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
            if (String.IsNullOrWhiteSpace(Email) && String.IsNullOrWhiteSpace(Password))
            {
                DisplayEmptyEmailAndPasswordErrorMessage();
            } else if(String.IsNullOrWhiteSpace(Email))
            {
                DisplayEmptyEmailErrorMessage();
            } else if(String.IsNullOrWhiteSpace(Password))
            {
                DisplayEmptyPasswordErrorMessage();
            } else 
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
            UserStatus userStatus = await IoCManager.Resolve<IUserManager> ().LoginUser (new User (email, password));
            UserDialogs.Instance.HideLoading();

            switch(userStatus)
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
                default:
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

        public String ErrorMessage
        {
            get { return errorMessage; }
            set { SetProperty(ref errorMessage, value); }
        }

        public String Email
        {
            get { return email; }
            set { SetProperty(ref email, value); }
        }

        public String Password
        {
            get { return password; }
            set { SetProperty(ref password, value); }
        }
    }
}
