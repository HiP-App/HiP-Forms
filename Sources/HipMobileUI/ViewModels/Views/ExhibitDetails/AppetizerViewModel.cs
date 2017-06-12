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
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using Xamarin.Forms;
using System;
using System.Windows.Input;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views.ExhibitDetails
{
    public class AppetizerViewModel : ExhibitSubviewViewModel, IDownloadableListItemViewModel
    {

        private Exhibit exhibit;
        private ImageSource image;
        private string text;
        private string headline;

        public AppetizerViewModel (Exhibit exhibit, string exhibitName, AppetizerPage page)
        {
            if (page != null)
            {
                Exhibit = exhibit;
                Headline = exhibitName;
                Text = page.Text;

                // workaround for realm bug
                var imageData = page.Image.Data;
                Image = ImageSource.FromStream (() => new MemoryStream (imageData));

                IsDownloadButtonVisible = !Exhibit.DetailsDataLoaded;

                DownloadCommand = new Command(OpenDownloadDialog);
            }
        }

        private async void OpenDownloadDialog()
        {
            // Open the download dialog
            downloadPage = new ExhibitRouteDownloadViewModel(Exhibit, this);
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

        private ExhibitRouteDownloadViewModel downloadPage;

        public ICommand DownloadCommand { get; set; }

        public Exhibit Exhibit
        {
            get { return exhibit; }
            set { SetProperty(ref exhibit, value); }
        }

        /// <summary>
        /// The appetizer image.
        /// </summary>
        public ImageSource Image {
            get { return image; }
            set { SetProperty (ref image, value); }
        }

        /// <summary>
        /// The headline of the description.
        /// </summary>
        public string Headline {
            get { return headline; }
            set { SetProperty (ref headline, value); }
        }

        /// <summary>
        /// The text of the description.
        /// </summary>
        public string Text {
            get { return text; }
            set { SetProperty (ref text, value); }
        }

    }
}
