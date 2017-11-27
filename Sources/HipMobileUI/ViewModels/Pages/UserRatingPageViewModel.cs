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


using Acr.UserDialogs;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using System;
using System.Windows.Input;
using Xamarin.Forms;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.ModelClasses;
using System.IO;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.DtoToModelConverters;
using SixLabors.ImageSharp;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages {
    public class UserRatingPageViewModel : NavigationViewModel {

        #region Variabels
        private ImageSource image;
        private Exhibit exhibit;
        private string headline;
        private ICommand sendRatingCommand;
        private ICommand selectStarCommand;

        private string star1;
        private string star2;
        private string star3;
        private string star4;
        private string star5;

        private GridLength star1Bar;
        private GridLength star2Bar;
        private GridLength star3Bar;
        private GridLength star4Bar;
        private GridLength star5Bar;

        private string star1BarCount;
        private string star2BarCount;
        private string star3BarCount;
        private string star4BarCount;
        private string star5BarCount;

        private string ratingAverage;
        private string ratingStars;
        private string ratingCount;

        private UserRatingManager userRatingManager;

        private int rating = 0;

        private int testUserRatingCount = 0;
        private int[] testUserRatings = new int[] { 47, 7, 7, 1, 1 };
        #endregion


        public UserRatingPageViewModel(Exhibit exhibit) {
            Exhibit = exhibit;
            Headline = exhibit.Name;
            userRatingManager = UserRatingManager.GetInstance();

            foreach (int i in testUserRatings)
                testUserRatingCount += i;

            var imageData = exhibit.AppetizerPage.Image.Data;
            Image = imageData != null ? ImageSource.FromStream(() => new MemoryStream(imageData)) : ImageSource.FromStream(() => new MemoryStream(BackupData.BackupImageData));

            SetUserRating();
            SetRatingBarsAndCount();
            SetRatingStars();
            SendRatingCommand = new Command(SendUserRating);
            SelectStarCommand = new Command(SelectStar);

        }

        private void SendUserRating() {
            if (IoCManager.Resolve<INetworkAccessChecker>().GetNetworkAccessStatus() == NetworkAccessStatus.NoAccess) {
                UserDialogs.Instance.Alert(new AlertConfig() {
                    Title = "Titel",
                    Message = "Die Nachricht",
                    OkText = "Jo"
                });
                return;
            }
            if (!Settings.IsLoggedIn) {
                UserDialogs.Instance.Alert(new AlertConfig() {
                    Title = "Titel",
                    Message = "Die Nachricht",
                    OkText = "Jo"
                });
                return;
            }
            if (rating == 0) {
                UserDialogs.Instance.Alert(new AlertConfig() {
                    Title = "Titel",
                    Message = "Die Nachricht",
                    OkText = "Jo"
                });
                return;
            }
            UserRatingManager.GetInstance().SetUserRating();
        }

        private void SetRatingBarsAndCount() {
            Star5BarCount = testUserRatings[0].ToString();
            Star4BarCount = testUserRatings[1].ToString();
            Star3BarCount = testUserRatings[2].ToString();
            Star2BarCount = testUserRatings[3].ToString();
            Star1BarCount = testUserRatings[4].ToString();
            Star5Bar = CalculateBarProportion(testUserRatings[0], testUserRatingCount);
            Star4Bar = CalculateBarProportion(testUserRatings[1], testUserRatingCount);
            Star3Bar = CalculateBarProportion(testUserRatings[2], testUserRatingCount);
            Star2Bar = CalculateBarProportion(testUserRatings[3], testUserRatingCount);
            Star1Bar = CalculateBarProportion(testUserRatings[4], testUserRatingCount);
        }

        private GridLength CalculateBarProportion(double value, double totalCount) {
            if (value == 0)
                return new GridLength(0, GridUnitType.Absolute);
            if (value == totalCount)
                return new GridLength(short.MaxValue, GridUnitType.Star);
            double prop = (1 / (1 - value / totalCount) - 1);
            return new GridLength(prop, GridUnitType.Star);
        }

        private void SetRatingStars() {
            Star1 = "☆";
            Star2 = "☆";
            Star3 = "☆";
            Star4 = "☆";
            Star5 = "☆";
            switch (rating) {
                case 5:
                    Star5 = "★";
                    goto case 4;
                case 4:
                    Star4 = "★";
                    goto case 3;
                case 3:
                    Star3 = "★";
                    goto case 2;
                case 2:
                    Star2 = "★";
                    goto case 1;
                case 1:
                    Star1 = "★";
                    break;
            }
        }

        private void SelectStar(object s) {
            rating = Convert.ToInt32(s);
            SetRatingStars();
        }

        private async void SetUserRating() {
            UserRating userRating = await userRatingManager.GetUserRating(exhibit);
            string stars = "";
            for (int i = 1; i <= 5; i++) {
                if (userRating.Average >= i)
                    stars += "★";
                else
                    stars += "☆";
            }
            RatingAverage = userRating.Average.ToString("0.#");
            RatingStars = stars;
            RatingCount = userRating.Count.ToString() + " " + Strings.UserRating_Rate_Count;
        }


        #region Properties

        public ImageSource Image {
            get { return image; }
            set { SetProperty(ref image, value); }
        }

        public Exhibit Exhibit {
            get { return exhibit; }
            set { SetProperty(ref exhibit, value); }
        }

        /// <summary>
        /// The headline of the description.
        /// </summary>
        public string Headline {
            get { return headline; }
            set { SetProperty(ref headline, value); }
        }

        public ICommand SendRatingCommand {
            get { return sendRatingCommand; }
            set { SetProperty(ref sendRatingCommand, value); }
        }

        public ICommand SelectStarCommand {
            get { return selectStarCommand; }
            set { SetProperty(ref selectStarCommand, value); }
        }

        public GridLength Star1Bar {
            get { return star1Bar; }
            set { SetProperty(ref star1Bar, value); }
        }

        public GridLength Star2Bar {
            get { return star2Bar; }
            set { SetProperty(ref star2Bar, value); }
        }

        public GridLength Star3Bar {
            get { return star3Bar; }
            set { SetProperty(ref star3Bar, value); }
        }

        public GridLength Star4Bar {
            get { return star4Bar; }
            set { SetProperty(ref star4Bar, value); }
        }

        public GridLength Star5Bar {
            get { return star5Bar; }
            set { SetProperty(ref star5Bar, value); }
        }

        public string Star1BarCount {
            get { return star1BarCount; }
            set { SetProperty(ref star1BarCount, value); }
        }

        public string Star2BarCount {
            get { return star2BarCount; }
            set { SetProperty(ref star2BarCount, value); }
        }

        public string Star3BarCount {
            get { return star3BarCount; }
            set { SetProperty(ref star3BarCount, value); }
        }

        public string Star4BarCount {
            get { return star4BarCount; }
            set { SetProperty(ref star4BarCount, value); }
        }

        public string Star5BarCount {
            get { return star5BarCount; }
            set { SetProperty(ref star5BarCount, value); }
        }

        public string Star1 {
            get { return star1; }
            set { SetProperty(ref star1, value); }
        }

        public string Star2 {
            get { return star2; }
            set { SetProperty(ref star2, value); }
        }

        public string Star3 {
            get { return star3; }
            set { SetProperty(ref star3, value); }
        }

        public string Star4 {
            get { return star4; }
            set { SetProperty(ref star4, value); }
        }

        public string Star5 {
            get { return star5; }
            set { SetProperty(ref star5, value); }
        }

        /// <summary>
        /// The average user rating text
        /// </summary>
        public string RatingAverage {
            get { return ratingAverage; }
            set { SetProperty(ref ratingAverage, value); }
        }

        /// <summary>
        /// The text which cotains the stars of the user rating
        /// </summary>
        public string RatingStars {
            get { return ratingStars; }
            set { SetProperty(ref ratingStars, value); }
        }

        /// <summary>
        /// The user rating count text
        /// </summary>
        public string RatingCount {
            get { return ratingCount; }
            set { SetProperty(ref ratingCount, value); }
        }
        #endregion
    }
}