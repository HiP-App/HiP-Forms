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

using Acr.UserDialogs;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using System.Windows.Input;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.User;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.UserManagement;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using Xamarin.Forms;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages
{
	public class AdventurerDetailsViewModel : NavigationViewModel
	{
		private readonly NavigationViewModel parentViewModel;

		public AdventurerDetailsViewModel(NavigationViewModel parentViewModel)
		{
			this.parentViewModel = parentViewModel;
			SelectModeButton = new Command(OnSelectModeButton);
			ChangeModeButton = new Command(OnChangeModeButton);
		}

		public ICommand SelectModeButton { get; }
		public ICommand ChangeModeButton { get; }

		private void OnSelectModeButton()
		{
			Settings.AdventurerMode = true;
		}
		private void OnChangeModeButton()
		{

		}


	}
}