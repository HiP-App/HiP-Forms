﻿// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
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

using Acr.UserDialogs;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using System.Windows.Input;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.User;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.UserManagement;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views
{
    public class ForgotPasswordScreenViewModel : NavigationViewModel
    {
        private readonly MainPageViewModel mainPageViewModel;
        private string email;
        private string errorMessage;

        public ForgotPasswordScreenViewModel(MainPageViewModel mainPageVm)
        {
            mainPageViewModel = mainPageVm;

            ResetPasswordCommand = new Command(OnResetPasswordClicked);
            ReturnCommand = new Command(ReturnToLogin);
        }

        public ICommand ResetPasswordCommand { get; }
        public ICommand ReturnCommand { get; }

        private void OnResetPasswordClicked()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                DisplayEmptyEmailErrorMessage();
            }
            else if (!Email.Contains("@"))
            {
                DisplayInvalidEmailMessage();
            }
            else
            {
                SendResetPasswordEmail();
            }
        }

        private async void SendResetPasswordEmail()
        {
            var userStatus = await IoCManager.Resolve<IUserManager>().ForgotPassword(new User(Email, "",""));

            if (userStatus == UserStatus.PasswordResetEmailSent)
            {
                mainPageViewModel.SwitchToLoginView();
                UserDialogs.Instance.Alert(Strings.ForgotPasswordScreenView_Alert_Description, Strings.ForgotPasswordScreenView_Alert_Password_Resetted);
            }
            else if (userStatus == UserStatus.NetworkConnectionFailed)
            {
                UserDialogs.Instance.Alert(Strings.ForgotPasswordScreenView_Alert_No_Connection_Description, Strings.ForgotPasswordScreenView_Alert_No_Connection_Title);
            }
            else
            {
                UserDialogs.Instance.Alert(Strings.ForgotPasswordScreenView_Alert_Unknown_Error_Description, Strings.ForgotPasswordScreenView_Alert_Unknown_Error_Title);
            }
        }

        private void DisplayEmptyEmailErrorMessage()
        {
            ErrorMessage = Strings.ForgotPasswordScreenView_Error_Empty_Email;
        }

        private void DisplayInvalidEmailMessage()
        {
            ErrorMessage = Strings.ForgotPasswordScreenView_Error_Invalid_Email;
        }

        private void ReturnToLogin()
        {
            mainPageViewModel.SwitchToLoginView();
        }

        public string Email
        {
            get { return email; }
            set { SetProperty(ref email, value); }
        }

        public string ErrorMessage
        {
            get { return errorMessage; }
            set { SetProperty(ref errorMessage, value); }
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            ErrorMessage = "";
        }
    }
}