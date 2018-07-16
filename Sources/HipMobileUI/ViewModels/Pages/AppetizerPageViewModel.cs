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
// limitations under the License

using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.DtoToModelConverters;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages
{
    public class AppetizerPageViewModel : NavigationViewModel, IDownloadableListItemViewModel
    {
        private Exhibit exhibit;
        private ImageSource image;
        private string text;
        private string headline;
        private bool nextVisible;
        private bool nextViewAvailable;
        private ICommand nextViewCommand;
        private bool exhibitUnblocked;

        public AppetizerPageViewModel(string exhibitId) : this(DbManager.DataAccess.Exhibits().GetExhibit(exhibitId))
        {
        }

        public AppetizerPageViewModel(Exhibit exhibit)
        {
            Exhibit = exhibit;
#if (DEBUG)
            exhibitUnblocked = true;
#else
            exhibitUnblocked = exhibit.Unlocked;
#endif
            Rating = new RatingViewModel(exhibit, true);

            Headline = exhibit.Name;
            Text = string.IsNullOrEmpty(exhibit.Description) ? exhibit.Name : exhibit.Description;

            if (Exhibit?.Pages != null && Exhibit.Pages.Count >= 1 && Exhibit.DetailsDataLoaded)
                NextViewAvailable = true;

            SetExhibitImage();
            IsDownloadButtonVisible = !Exhibit.DetailsDataLoaded;
            NextViewCommand = new Command(GotoNextView);
            DownloadCommand = new Command(OpenDownloadDialog);
        }

        private async void SetExhibitImage()
        {
            var imageData = (exhibit.Image != null) ? await exhibit.Image.GetDataAsync() : null;
            Image = imageData != null
                ? ImageSource.FromStream(() => new MemoryStream(imageData))
                : ImageSource.FromStream(() => new MemoryStream(BackupData.BackupImageData));
        }

        /// <summary>
        /// If there is more than just an appetizer page show the first details page.
        /// </summary>
        /// <returns></returns>
        private async void GotoNextView()
        {
            if (Exhibit.Pages.Count >= 1)
            {
                if (exhibitUnblocked)
                {
                    await Navigation.PushAsync(new ExhibitDetailsPageViewModel(Exhibit));
                }
                else
                {
                    await Navigation.DisplayAlert(Strings.ExhibitDetailsPage_Distance_Title, Strings.ExhibitDetailsPage_Distance_Text, Strings.Ok);
                }
            }
        }

        private async void OpenDownloadDialog()
        {
            // Open the download dialog
            downloadPage = new ExhibitRouteDownloadPageViewModel(Exhibit, this);
            await Navigation.PushAsync(downloadPage);
        }

        /// <summary>
        /// Sets the average user rating, the count and the stars if the user is connected to the internet.
        /// </summary>
        /// <returns></returns>

        public void CloseDownloadPage()
        {
            Navigation.PopAsync();
        }

        public void OpenDetailsView(string id)
        {
            //Do nothing. Never called
        }

        private bool isDownloadButtonVisible;

        public bool IsDownloadButtonVisible
        {
            get { return isDownloadButtonVisible; }
            set { SetProperty(ref isDownloadButtonVisible, value); }
        }

        public async Task SetDetailsAvailable(bool available)
        {
            if (!available)
                return;

            DbManager.InTransaction(transaction =>
            {
                Exhibit.DetailsDataLoaded = true;
                return Task.CompletedTask;
            });

            Exhibit = DbManager.DataAccess.Exhibits().GetExhibit(Exhibit.Id);
            IsDownloadButtonVisible = !Exhibit.DetailsDataLoaded;
            NextViewAvailable = Exhibit.Pages.Count >= 1;
        }

        private ExhibitRouteDownloadPageViewModel downloadPage;

        public ICommand DownloadCommand { get; set; }

        #region Properties

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

        /// <summary>
        /// The command for switching to the next view, if available.
        /// </summary>
        public ICommand NextViewCommand
        {
            get { return nextViewCommand; }
            set { SetProperty(ref nextViewCommand, value); }
        }

        /// <summary>
        /// Indicator if a next view is available.
        /// </summary>
        public bool NextViewAvailable
        {
            get { return nextViewAvailable; }
            set
            {
                NextVisible = value;
                SetProperty(ref nextViewAvailable, value);
            }
        }

        /// <summary>
        /// Indicator if navigation to next is visible
        /// </summary>
        public bool NextVisible
        {
            get { return nextVisible; }
            set { SetProperty(ref nextVisible, value); }
        }

        public RatingViewModel Rating { get; }
        #endregion
    }
}