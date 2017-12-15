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

using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.DtoToModelConverters;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;
using Xamarin.Forms;
using Page = PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.Page;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.ModelClasses;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages
{
    public class AppetizerPageViewModel : NavigationViewModel, IDownloadableListItemViewModel
    {
        private Exhibit exhibit;
        private ImageSource image;
        private string text;
        private string headline;
        private readonly IList<Page> pages;
        private bool nextVisible;
        private bool nextViewAvailable;
        private ICommand nextViewCommand;
        private bool exhibitUnblocked;

        private ICommand userRatingCommand;
        private string ratingAverage;
        private string ratingStars;
        private string ratingCount;

        public AppetizerPageViewModel(string exhibitId) : this(ExhibitManager.GetExhibit(exhibitId)) { }

        public AppetizerPageViewModel(Exhibit exhibit)
        {
            Exhibit = exhibit;
#if (DEBUG)
            exhibitUnblocked = true;
#else
            exhibitUnblocked = exhibit.Unlocked;
#endif
            pages = exhibit.Pages;

            Headline = exhibit.Name;
            Text = exhibit.Name;

            if (pages.Count > 1 && Exhibit.DetailsDataLoaded)
                NextViewAvailable = true;
            // workaround for realm bug
            var imageData = exhibit.Image.GetDataBlocking();
            Image = imageData != null ? ImageSource.FromStream(() => new MemoryStream(imageData)) : ImageSource.FromStream(() => new MemoryStream(BackupData.BackupImageData));

            IsDownloadButtonVisible = !Exhibit.DetailsDataLoaded;
            NextViewCommand = new Command(GotoNextView);
            DownloadCommand = new Command(OpenDownloadDialog);
            UserRatingCommand = new Command(GoToUserRatingPage);
            SetUserRating();
        }

        /// <summary>
        /// If there is more than just an appetizer page show the first details page.
        /// </summary>
        /// <returns></returns>
        private async void GotoNextView()
        {
            if (pages.Count > 1)
            {
                if (exhibitUnblocked)
                {
                    await Navigation.PushAsync(new ExhibitDetailsViewModel(Exhibit));

                }
                else
                {
                    await Navigation.DisplayAlert(Strings.ExhibitDetailsPage_Distance_Title, Strings.ExhibitDetailsPage_Distance_Text, Strings.ExhibitDetailsPage_Distance_alert_confirm);
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
        private async void SetUserRating()
        {
            if (IoCManager.Resolve<INetworkAccessChecker>().GetNetworkAccessStatus() == NetworkAccessStatus.NoAccess)
            {
                RatingAverage = "-";
                RatingStars = "?????";
                RatingCount = "- " + Strings.UserRating_Rate_Count;
            }
            else
            {
                var userRating = await IoCManager.Resolve<IUserRatingManager>().GetUserRatingAsync(exhibit);
                var stars = "";
                for (int i = 1; i <= 5; i++)
                {
                    if (userRating.Average >= i)
                        stars += "★";
                    else
                        stars += "☆";
                }
                if (userRating.Count > 0)
                    RatingAverage = userRating.Average.ToString("0.#");
                else
                    RatingAverage = "-";
                RatingStars = stars;
                RatingCount = userRating.Count.ToString() + " " + Strings.UserRating_Rate_Count;
            }
        }

        private async void GoToUserRatingPage()
        {
            if (IoCManager.Resolve<INetworkAccessChecker>().GetNetworkAccessStatus() != NetworkAccessStatus.NoAccess)
            {
                await Navigation.PushAsync(new UserRatingPageViewModel(Exhibit));
            }
        }

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

        public void SetDetailsAvailable(bool available)
        {
            if (!available)
                return;

            using (DbManager.StartTransaction())
            {
                Exhibit.DetailsDataLoaded = true;
            }
            IsDownloadButtonVisible = !Exhibit.DetailsDataLoaded;

            NextViewAvailable = pages.Count > 1;
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

        /// <summary>
        /// The command for switch to the user rating
        /// </summary>
        public ICommand UserRatingCommand
        {
            get { return userRatingCommand; }
            set { SetProperty(ref userRatingCommand, value); }
        }

        /// <summary>
        /// The average user rating text
        /// </summary>
        public string RatingAverage
        {
            get { return ratingAverage; }
            set { SetProperty(ref ratingAverage, value); }
        }

        /// <summary>
        /// The text which cotains the stars of the user rating
        /// </summary>
        public string RatingStars
        {
            get { return ratingStars; }
            set { SetProperty(ref ratingStars, value); }
        }

        /// <summary>
        /// The user rating count text
        /// </summary>
        public string RatingCount
        {
            get { return ratingCount; }
            set { SetProperty(ref ratingCount, value); }
        }
    }

    #endregion
}