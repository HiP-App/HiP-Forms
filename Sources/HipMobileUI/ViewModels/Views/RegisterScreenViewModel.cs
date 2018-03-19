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

using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.User;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.UserManagement;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using System.Windows.Input;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views
{
    public class RegisterScreenViewModel : NavigationViewModel
    {
        private string email;
        private string password;
        private string repassword;
        private string errorMessage;
        private readonly MainPageViewModel mainPageViewModel;

        public RegisterScreenViewModel(MainPageViewModel mainPageVm)
        {
            mainPageViewModel = mainPageVm;

            Register = new Command(OnRegisterClicked);
            ReturnCommand = new Command(ReturnToLogin);
        }

        public ICommand Register { get; }
        public ICommand ReturnCommand { get; }

        async void RegisterUser()
        {
            var user = new User(Email, password);
            var userStatus = await IoCManager.Resolve<IUserManager>().Register(user);
            if (userStatus == UserStatus.Registered)
            {
                mainPageViewModel.SwitchToLoginView();
                await Application.Current.MainPage.DisplayAlert(Strings.RegisterScreenView_Alert_Registered, Strings.RegisterScreenView_Alert_Description, Strings.Ok);
            }
            else
            {
                DisplayRegisterFailedErrorMessage();
            }
        }

        private void DisplayEmptyEmailAndPasswordErrorMessage()
        {
            ErrorMessage = Strings.RegisterScreenView_Error_Empty_Email_And_Password;
        }

        private void DisplayRegisterFailedErrorMessage()
        {
            ErrorMessage = Strings.RegisterScreenView_Error_Register_Fail;
        }

        private void DisplayEmptyPasswordErrorMessage()
        {
            ErrorMessage = Strings.RegisterScreenView_Error_Empty_Password;
        }
        private void DisplayPasswordMismatchErrorMessage()
        {
            ErrorMessage = Strings.RegisterScreenView_Error_Mismatch_Password;
        }
        private void DisplayEmptyEmailErrorMessage()
        {
            ErrorMessage = Strings.RegisterScreenView_Error_Empty_Email;
        }
        private void DisplayInvalidEmailErrorMessage()
        {
            ErrorMessage = Strings.RegisterScreenView_Error_Invalid_Email;
        }

        private void OnRegisterClicked()
        {
            if (string.IsNullOrWhiteSpace(Email) && (string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(RepeatPassword)))
                DisplayEmptyEmailAndPasswordErrorMessage();
            else if (string.IsNullOrWhiteSpace(Email))
                DisplayEmptyEmailErrorMessage();
            else if (string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(RepeatPassword))
                DisplayEmptyPasswordErrorMessage();
            else if (!Email.Contains("@"))
                DisplayInvalidEmailErrorMessage();
            else if (Password != RepeatPassword)
                DisplayPasswordMismatchErrorMessage();
            else
                RegisterUser();
        }

        private void ReturnToLogin()
        {
            mainPageViewModel.SwitchToLoginView();
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
        public string RepeatPassword
        {
            get { return repassword; }
            set { SetProperty(ref repassword, value); }
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            ErrorMessage = "";
        }
    }
}
