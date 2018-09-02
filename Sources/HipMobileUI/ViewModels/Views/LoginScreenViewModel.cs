// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Windows.Input;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using Xamarin.Forms;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.User;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.UserManagement;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using Acr.UserDialogs;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.AuthenticationApiAccess;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views
{
    public class LoginScreenViewModel : NavigationViewModel
    {
        private readonly MainPageViewModel mainPageViewModel;
        private string email;
        private string password;
        private string errorMessage;

        public LoginScreenViewModel(MainPageViewModel mainPageVm)
        {
            mainPageViewModel = mainPageVm;

            LoginCommand = new Command(OnLoginClicked);
            DebugLoginCommand = new Command(OnDebugLoginClicked);
            RegisterCommand = new Command(OnRegisterClicked);
            ForgotPasswordCommand = new Command(OnForgotPasswordClicked);
        }

        public ICommand LoginCommand { get; }
        public ICommand DebugLoginCommand { get; }
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

        private void OnDebugLoginClicked()
        {
#if DEBUG
            Email = Constants.DebugEmail;
            Password = Constants.DebugPassword;
            PerformLogin();
#else
            throw new System.Exception("This button must not be visible in non-debug mode!");
#endif
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
            UserStatus userStatus = await IoCManager.Resolve<IUserManager>().Login(new User(email, password,""));
            UserDialogs.Instance.HideLoading();

            switch (userStatus)
            {
                
                case UserStatus.IncorrectEmailAndPassword:
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
                case UserStatus.UnknownError:
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

        public override void OnAppearing()
        {
            base.OnAppearing();
            ErrorMessage = "";
        }
    }
}