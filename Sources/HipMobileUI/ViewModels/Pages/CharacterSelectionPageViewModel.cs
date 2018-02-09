
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

using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using System.Windows.Input;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;
using Xamarin.Forms;
using SkiaSharp.Views.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages
{
	public class CharacterSelectionPageViewModel : NavigationViewModel
	{
		private readonly NavigationViewModel parentViewModel;

		public CharacterSelectionPageViewModel(NavigationViewModel parentViewModel)
		{
			this.parentViewModel = parentViewModel;

			AdventurerGridTappedCommand = new Command(OnAdventurerGridTapped);
			ProfessorGridTappedCommand = new Command(OnProfessorGridTapped);
		}

		public ICommand AdventurerGridTappedCommand { get; }
		public ICommand ProfessorGridTappedCommand { get; }

		private async void OnAdventurerGridTapped()
		{
			await Navigation.PushAsync(new CharacterDetailsPageViewModel(parentViewModel, true), false);
		}

		private async void OnProfessorGridTapped()
		{
			await Navigation.PushAsync(new CharacterDetailsPageViewModel(parentViewModel, false), false);
		}
		private void OnPaintSample(object sender, SKPaintSurfaceEventArgs e)
		{

		}
		/// <summary>
		/// Returns to the previous page if this page was called from the profile or settings page
		/// </summary>
		public void ReturnToPreviousPage()
		{
			if (parentViewModel.GetType() == typeof(ProfileScreenViewModel))
			{
				var mainPageViewModel = new MainPageViewModel();
				Navigation.StartNewNavigationStack(mainPageViewModel);
				mainPageViewModel.SwitchToProfileView();
			}
			else if (parentViewModel.GetType() == typeof(SettingsScreenViewModel))
			{
				var mainPageViewModel = new MainPageViewModel();
				Navigation.StartNewNavigationStack(mainPageViewModel);
				mainPageViewModel.SwitchToSettingsScreenView();
			}
		}

		public NavigationViewModel ParentViewModel => parentViewModel;
	}
}