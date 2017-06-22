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
using System;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.User;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.UserManagement;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using System.Windows.Input;
using Xamarin.Forms;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views
{
	public class RegisterScreenViewModel : NavigationViewModel
	{
		private String email;
		private String password;
		private String repassword;
		private String errorMessage;
		private readonly MainPageViewModel mainPageViewModel;


		public RegisterScreenViewModel(MainPageViewModel mainPageVm)
		{
			mainPageViewModel = mainPageVm;
			Register = new Command(OnRegisterClicked);

		}

		public ICommand Register { get; }





		async void RegisterUser()
		{
			
			User user = new User(Email, password);
			UserStatus userStatus = await IoCManager.Resolve<IUserManager>().RegisterUser(user);

			if (userStatus == UserStatus.Registered)
			{
				mainPageViewModel.SwitchToLoginView();
				await Application.Current.MainPage.DisplayAlert(Strings.RegisterScreenView_Alert_Registered, Strings.RegisterScreenView_Alert_Description, Strings.RegisterScreenView_Alert_Ok);

			}
			else
				DisplayRegisterFailedErrorMessage();

		}

		void DisplayEmptyEmailAndPasswordErrorMessage()
		{
			ErrorMessage = Strings.RegisterScreenView_Error_Empty_Email_And_Password;
		}

		void DisplayRegisterFailedErrorMessage()
		{
			ErrorMessage = Strings.RegisterScreenView_Error_Register_Fail; 
		}

		void DisplayEmptyPasswordErrorMessage()
		{
			ErrorMessage = Strings.RegisterScreenView_Error_Empty_Password;
		}

		void ClearErrorMessage()
		{
			ErrorMessage = "";
		}

		void OnRegisterClicked()
		{
			if (String.IsNullOrWhiteSpace(Email) && String.IsNullOrWhiteSpace(Password))

				DisplayEmptyEmailAndPasswordErrorMessage();
			
			else if (String.IsNullOrWhiteSpace(Password))

				DisplayEmptyPasswordErrorMessage();

			else

				RegisterUser();
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
		public String RepeatPassword
		{
			get { return repassword; }
			set { SetProperty(ref repassword, value); }
		}
	}
}
