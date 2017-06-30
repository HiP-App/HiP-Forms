using System.Diagnostics;
using System.Windows.Input;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using Xamarin.Forms;
using System;
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

            Login = new Command(PerformLogin);
            Register = new Command(GoToRegisterScreenView);
            ForgotPassword = new Command(GoToForgotPasswordScreenView);
        }

        public ICommand Login { get; }
        public ICommand Register { get; }
        public ICommand ForgotPassword { get; }

        void PerformLogin()
        {
            Debug.WriteLine("##### LOGIN #####");
            Debug.WriteLine("Email: " + Email + " Password: " + Password);
            Settings.IsLoggedIn = true;
            mainPageViewModel.UpdateAccountViews();
        }

        void GoToRegisterScreenView()
        {
            //Go to RegisterScreenView here
        }

        void GoToForgotPasswordScreenView()
        {
            //Go to ForgotPasswordScreenView here
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
