// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//       http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.IO;
using System.Collections.Generic;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using Xamarin.Forms;
using System;
using System.Windows.Input;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.DtoToModelConverters;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;
using Page = PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.Page;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views.ExhibitDetails;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages
{
	public class AppetizerPageViewModel : NavigationViewModel, IDownloadableListItemViewModel
	{

		private Exhibit exhibit;
		private ImageSource image;
		private string text;
		private string headline;
		private readonly IList<Page> pages;
		private int currentViewIndex;
		private AppetizerPage page;


		public AppetizerPageViewModel(Exhibit exhibit)
		//string exhibitName)
		//AppetizerPage page)
		{
			//if (page != null)
			//{
				Exhibit = exhibit;
				Headline = exhibit.Name;
				currentViewIndex = 0;
				pages = exhibit.Pages; 
				Page currentPage = pages[currentViewIndex];
				page = currentPage.AppetizerPage;
				Text = page.Text;

				// workaround for realm bug
				var imageData = page.Image.Data;
				if (imageData != null)
				{
					Image = ImageSource.FromStream(() => new MemoryStream(imageData));
				}
				else
				{
					Image = ImageSource.FromStream(() => new MemoryStream(BackupData.BackupImageData));
				}

				IsDownloadButtonVisible = !Exhibit.DetailsDataLoaded;

				DownloadCommand = new Command(OpenDownloadDialog);
			//}
		}

		private async void OpenDownloadDialog()
		{
			// Open the download dialog
			downloadPage = new ExhibitRouteDownloadPageViewModel(Exhibit, this);
			await Navigation.PushAsync(downloadPage);
		}

		public void CloseDownloadPage()
		{
			IoCManager.Resolve<INavigationService>().PopAsync();
		}

		public void OpenDetailsView(string id)
		{
			//Do nothing. Never called
		}

		private Boolean isDownloadButtonVisible;
		public Boolean IsDownloadButtonVisible
		{
			get { return isDownloadButtonVisible; }
			set { SetProperty(ref isDownloadButtonVisible, value); }
		}

		public void SetDetailsAvailable(bool available)
		{
			if (!available)
				return;

			using (DbManager.StartTransaction())
			{
				Exhibit.DetailsDataLoaded = true;
			}
			IsDownloadButtonVisible = !Exhibit.DetailsDataLoaded;
		}

		private ExhibitRouteDownloadPageViewModel downloadPage;

		public ICommand DownloadCommand { get; set; }

		public Exhibit Exhibit
		{
			get { return exhibit; }
			set { SetProperty(ref exhibit, value); }
		}

		/// <summary>
		/// The appetizer image.
		/// </summary>
		public ImageSource Image
		{
			get { return image; }
			set { SetProperty(ref image, value); }
		}

		/// <summary>
		/// The headline of the description.
		/// </summary>
		public string Headline
		{
			get { return headline; }
			set { SetProperty(ref headline, value); }
		}

		/// <summary>
		/// The text of the description.
		/// </summary>
		public string Text
		{
			get { return text; }
			set { SetProperty(ref text, value); }
		}

	}
}
