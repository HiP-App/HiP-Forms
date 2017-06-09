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
namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views
{
	public class RegisterViewModel : NavigationViewModel
	{
		private String email;
		private String password;
		private String repassword;
		private String errorMessage;

		void clearErrorMessage()
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
		public String RepeatPassword
		{
			get { return repassword; }
			set { SetProperty(ref repassword, value); }
		}
	}
}
