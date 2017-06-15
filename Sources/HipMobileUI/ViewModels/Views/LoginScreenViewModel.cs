using System.Diagnostics;
using System.Windows.Input;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using Xamarin.Forms;
using System;
using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.User;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.UserManagement;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.Exceptions;

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

        void OnLoginClicked()
        {
            if(String.IsNullOrWhiteSpace(Email))
            {
                // Display Error
            }

            if(String.IsNullOrWhiteSpace(Password))
            {
                // Display Error
            }

            PerformLogin();
        }

        void OnRegisterClicked()
        {
            GoToRegisterScreenView();
        }

        void OnForgotPasswordClicked()
        {
            GoToForgotPasswordScreenView();
        }

        async void PerformLogin()
        {
            User user = new User (Email, password);
            UserStatus userStatus = await IoCManager.Resolve<IUserManager> ().LoginUser (new User (email, password));

            if (userStatus == UserStatus.LoggedIn)
                Settings.IsLoggedIn = true;

            if (userStatus == UserStatus.InCorrectUserNameandPassword)
                await Task.Run (() => {
                    this.ErrorMessage = "Wrong Password";
                });


            mainPageViewModel.UpdateAccountViews();
        }

        void GoToRegisterScreenView()
        {
            //Go to RegisterScreenView here
        }

        void GoToForgotPasswordScreenView()
        {
            mainPageViewModel.SwitchToForgotPasswordView();
        }

        void DisplayUnknownEmailErrorMessage()
        {
            ErrorMessage = Strings.LoginScreenView_Error_Unknown_Email;
        }

        void DisplayWrongPasswordErrorMessage()
        {
            ErrorMessage = Strings.LoginScreenView_Error_Wrong_Password;
        }

        void ClearErrorMessage()
        {
            ErrorMessage = "";
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
