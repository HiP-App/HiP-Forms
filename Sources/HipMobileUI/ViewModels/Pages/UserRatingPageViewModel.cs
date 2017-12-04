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
using System.Collections.Generic;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages
{
    public class UserRatingPageViewModel : NavigationViewModel
    {

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

        private int rating = 0;
        #endregion


        public UserRatingPageViewModel(Exhibit exhibit)
        {
            Exhibit = exhibit;
            Headline = exhibit.Name;

            var imageData = exhibit.Image.GetDataBlocking();
            Image = imageData != null ? ImageSource.FromStream(() => new MemoryStream(imageData)) : ImageSource.FromStream(() => new MemoryStream(BackupData.BackupImageData));

            SetUserRatingUi();
            SendRatingCommand = new Command(SendUserRating);
            SelectStarCommand = new Command(OnSelectStar);

        }
        private async void SetUserRatingUi()
        {
            UserRating userRating = await IoCManager.Resolve<IUserRatingManager>().GetUserRating(exhibit);
            SetRatingAverageAndCount(userRating.Average, userRating.Count);
            SetRatingBars(userRating.RatingTable, userRating.Count);
            SetRatingStars();
        }

        private void SetRatingAverageAndCount(double average, int count)
        {
            string stars = "";
            for (int i = 1; i <= 5; i++)
            {
                if (average >= i)
                    stars += "★";
                else
                    stars += "☆";
            }
            if (count > 0)
                RatingAverage = average.ToString("0.#");
            else
                RatingAverage = "-";
            RatingStars = stars;
            RatingCount = count.ToString() + " " + Strings.UserRating_Rate_Count;
        }


        private void SetRatingBars(Dictionary<int, int> ratingTable, int count)
        {
            Star5BarCount = ratingTable[5].ToString();
            Star4BarCount = ratingTable[4].ToString();
            Star3BarCount = ratingTable[3].ToString();
            Star2BarCount = ratingTable[2].ToString();
            Star1BarCount = ratingTable[1].ToString();
            Star5Bar = CalculateBarProportion(ratingTable[5], count);
            Star4Bar = CalculateBarProportion(ratingTable[4], count);
            Star3Bar = CalculateBarProportion(ratingTable[3], count);
            Star2Bar = CalculateBarProportion(ratingTable[2], count);
            Star1Bar = CalculateBarProportion(ratingTable[1], count);
        }

        private GridLength CalculateBarProportion(double value, double totalCount)
        {
            if (value == 0)
                return new GridLength(0, GridUnitType.Absolute);
            if (value == totalCount)
                return new GridLength(short.MaxValue, GridUnitType.Star);
            double prop = (1 / (1 - value / totalCount) - 1);
            return new GridLength(prop, GridUnitType.Star);
        }


        /// <summary>
        /// This method is executed when a star is tapped. 
        /// The rating comes from the command parameter defined in the xaml file.
        /// </summary>
        /// <returns></returns>
        private void OnSelectStar(object s)
        {
            rating = Convert.ToInt32(s);
            SetRatingStars();
        }


        private void SetRatingStars()
        {
            Star1 = "☆";
            Star2 = "☆";
            Star3 = "☆";
            Star4 = "☆";
            Star5 = "☆";
            switch (rating)
            {
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

        /// <summary>
        /// Sends the user rating to the server if the following conditions are given:
        /// 1. The user is connected to the internet
        /// 2. The user is logged in
        /// 3. The user haas selected a star 
        /// </summary>
        /// <returns></returns>
        private async void SendUserRating()
        {
            if (IoCManager.Resolve<INetworkAccessChecker>().GetNetworkAccessStatus() == NetworkAccessStatus.NoAccess)
            {
                ShowDialog(Strings.UserRating_Dialog_Title_No_Internet, Strings.UserRating_Dialog_Message_No_Internet);
            }
            else if (!Settings.IsLoggedIn)
            {
                ShowDialog(Strings.UserRating_Dialog_Title_Not_Logged_In, Strings.UserRating_Dialog_Message_Not_Logged_In);
            }
            else if (rating == 0)
            {
                ShowDialog(Strings.UserRating_Dialog_Title_No_Rating, Strings.UserRating_Dialog_Message_No_Rating);
            }
            else if (await IoCManager.Resolve<IUserRatingManager>().SendUserRating(exhibit, rating))
            {
                SetUserRatingUi();
                ShowDialog(Strings.UserRating_Dialog_Title_Thx, Strings.UserRating_Dialog_Message_Thx);
            }
            else
            {
                ShowDialog(Strings.UserRating_Dialog_Title_Unkown_Error, Strings.UserRating_Dialog_Message_Unkown_Error);
            }
        }

        private void ShowDialog(string title, string message)
        {
            UserDialogs.Instance.Alert(new AlertConfig()
            {
                Title = title,
                Message = message,
                OkText = Strings.UserRating_Ok
            });
        }


        #region Properties

        public ImageSource Image
        {
            get { return image; }
            set { SetProperty(ref image, value); }
        }

        public Exhibit Exhibit
        {
            get { return exhibit; }
            set { SetProperty(ref exhibit, value); }
        }

        /// <summary>
        /// The headline of the description.
        /// </summary>
        public string Headline
        {
            get { return headline; }
            set { SetProperty(ref headline, value); }
        }

        public ICommand SendRatingCommand
        {
            get { return sendRatingCommand; }
            set { SetProperty(ref sendRatingCommand, value); }
        }

        public ICommand SelectStarCommand
        {
            get { return selectStarCommand; }
            set { SetProperty(ref selectStarCommand, value); }
        }

        public GridLength Star1Bar
        {
            get { return star1Bar; }
            set { SetProperty(ref star1Bar, value); }
        }

        public GridLength Star2Bar
        {
            get { return star2Bar; }
            set { SetProperty(ref star2Bar, value); }
        }

        public GridLength Star3Bar
        {
            get { return star3Bar; }
            set { SetProperty(ref star3Bar, value); }
        }

        public GridLength Star4Bar
        {
            get { return star4Bar; }
            set { SetProperty(ref star4Bar, value); }
        }

        public GridLength Star5Bar
        {
            get { return star5Bar; }
            set { SetProperty(ref star5Bar, value); }
        }

        public string Star1BarCount
        {
            get { return star1BarCount; }
            set { SetProperty(ref star1BarCount, value); }
        }

        public string Star2BarCount
        {
            get { return star2BarCount; }
            set { SetProperty(ref star2BarCount, value); }
        }

        public string Star3BarCount
        {
            get { return star3BarCount; }
            set { SetProperty(ref star3BarCount, value); }
        }

        public string Star4BarCount
        {
            get { return star4BarCount; }
            set { SetProperty(ref star4BarCount, value); }
        }

        public string Star5BarCount
        {
            get { return star5BarCount; }
            set { SetProperty(ref star5BarCount, value); }
        }

        public string Star1
        {
            get { return star1; }
            set { SetProperty(ref star1, value); }
        }

        public string Star2
        {
            get { return star2; }
            set { SetProperty(ref star2, value); }
        }

        public string Star3
        {
            get { return star3; }
            set { SetProperty(ref star3, value); }
        }

        public string Star4
        {
            get { return star4; }
            set { SetProperty(ref star4, value); }
        }

        public string Star5
        {
            get { return star5; }
            set { SetProperty(ref star5, value); }
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
        #endregion
    }
}