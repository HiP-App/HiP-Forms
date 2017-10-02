using Acr.UserDialogs;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.User;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.UserManagement;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views
{
    class ForgotPasswordScreenViewModel : NavigationViewModel
    {
        private readonly MainPageViewModel mainPageViewModel;
        private String email;
        private String errorMessage;

        public ICommand ResetPasswordCommand { get; }

        public ForgotPasswordScreenViewModel(MainPageViewModel mainPageVm)
        {
            mainPageViewModel = mainPageVm;

            ResetPasswordCommand = new Command(OnResetPasswordClicked);
        }

        private void OnResetPasswordClicked()
        {
            if (String.IsNullOrWhiteSpace(Email))
            {
                DisplayEmptyEmailErrorMessage();
            }
            else
            {
                SendResetPasswordEmail();
            }
        }

        private async void SendResetPasswordEmail()
        {
            UserStatus userStatus = await IoCManager.Resolve<IUserManager>().ForgotPassword(new User(Email, ""));

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

        public String Email
        {
            get { return email; }
            set { SetProperty(ref email, value); }
        }

        public String ErrorMessage
        {
            get { return errorMessage; }
            set { SetProperty(ref errorMessage, value); }
        }
    }
}