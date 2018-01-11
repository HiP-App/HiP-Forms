﻿// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
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

        #region Variables
        private ImageSource image;
        private Exhibit exhibit;
        private string headline;
        private ICommand sendRatingCommand;
        private ICommand selectStarCommand;

        private ImageSource star1;
        private ImageSource star2;
        private ImageSource star3;
        private ImageSource star4;
        private ImageSource star5;

        private ImageSource ratingStar1;
        private ImageSource ratingStar2;
        private ImageSource ratingStar3;
        private ImageSource ratingStar4;
        private ImageSource ratingStar5;

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
        private string ratingCount;

        private int lastRating = 0;

        private const string ImgStarEmpty = "star_empty.png";
        private const string ImgStarHalfFilled = "star_half_filled.png";
        private const string ImgStarFilled = "star_filled.png";
        #endregion

        public UserRatingPageViewModel(Exhibit exhibit)
        {
            Exhibit = exhibit;
            Headline = exhibit.Name;

            SetExhibitImage();
            SetUserRatingUi();
            SendRatingCommand = new Command(SendUserRating);
            SelectStarCommand = new Command(OnSelectStar);
        }

        private async void SetExhibitImage()
        {
            var imageData = await exhibit.Image.GetDataAsync();
            Image = imageData != null ? ImageSource.FromStream(() => new MemoryStream(imageData)) : ImageSource.FromStream(() => new MemoryStream(BackupData.BackupImageData));
        }

        private async void SetUserRatingUi()
        {
            UserRating userRating = await IoCManager.Resolve<IUserRatingManager>().GetUserRatingAsync(exhibit.IdForRestApi);
            SetAverageAndCountRating(userRating.Average, userRating.Count);
            SetStarImages(userRating.Average);
            SetRatingBars(userRating.RatingTable, userRating.Count);
            RatingStar1 = ImgStarEmpty;
            RatingStar2 = ImgStarEmpty;
            RatingStar3 = ImgStarEmpty;
            RatingStar4 = ImgStarEmpty;
            RatingStar5 = ImgStarEmpty;
        }

        private void SetAverageAndCountRating(double average, int count)
        {
            if (count > 0)
                RatingAverage = average.ToString("0.#");
            else
                RatingAverage = "-";
            RatingCount = count.ToString() + " " + Strings.UserRating_Rate_Count;
        }

        private void SetStarImages(double average)
        {
            if (average < 1)
                Star1 = ImgStarEmpty;
            else
                Star1 = ImgStarFilled;
            if (average < 1.25)
                Star2 = ImgStarEmpty;
            else if ((average >= 1.25 && average < 1.75))
                Star2 = ImgStarHalfFilled;
            else
                Star2 = ImgStarFilled;
            if (average < 2.25)
                Star3 = ImgStarEmpty;
            else if ((average >= 2.25 && average < 2.75))
                Star3 = ImgStarHalfFilled;
            else
                Star3 = ImgStarFilled;
            if (average < 3.25)
                Star4 = ImgStarEmpty;
            else if ((average >= 3.25 && average < 3.75))
                Star4 = ImgStarHalfFilled;
            else
                Star4 = ImgStarFilled;
            if (average < 4.25)
                Star5 = ImgStarEmpty;
            else if ((average >= 4.25 && average < 4.75))
                Star5 = ImgStarHalfFilled;
            else
                Star5 = ImgStarFilled;
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
            SetRatingStars(Convert.ToInt32(s));
        }

        private void SetRatingStars(int rating)
        {
            if (rating > lastRating)
            {
                String img = ImgStarFilled;
                if (lastRating == 0 && rating >= 1)
                    RatingStar1 = img;
                if (lastRating <= 1 && rating >= 2)
                    RatingStar2 = img;
                if (lastRating <= 2 && rating >= 3)
                    RatingStar3 = img;
                if (lastRating <= 3 && rating >= 4)
                    RatingStar4 = img;
                if (lastRating <= 4 && rating == 5)
                    RatingStar5 = img;
            }
            else
            {
                String img = ImgStarEmpty;
                if (lastRating == 5 && rating <= 4)
                    RatingStar5 = img;
                if (lastRating >= 4 && rating <= 3)
                    RatingStar4 = img;
                if (lastRating >= 3 && rating <= 2)
                    RatingStar3 = img;
                if (lastRating >= 2 && rating <= 1)
                    RatingStar2 = img;
            }
            lastRating = rating;
        }

        /// <summary>
        /// Sends the user rating to the server if the following conditions are given:
        /// 1. The user is connected to the internet
        /// 2. The user is logged in
        /// 3. The user has selected a star 
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
            else if (lastRating == 0)
            {
                ShowDialog(Strings.UserRating_Dialog_Title_No_Rating, Strings.UserRating_Dialog_Message_No_Rating);
            }
            else if (await IoCManager.Resolve<IUserRatingManager>().SendUserRatingAsync(exhibit.IdForRestApi, lastRating))
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

        public ImageSource Star1
        {
            get { return star1; }
            set { SetProperty(ref star1, value); }
        }

        public ImageSource Star2
        {
            get { return star2; }
            set { SetProperty(ref star2, value); }
        }

        public ImageSource Star3
        {
            get { return star3; }
            set { SetProperty(ref star3, value); }
        }

        public ImageSource Star4
        {
            get { return star4; }
            set { SetProperty(ref star4, value); }
        }

        public ImageSource Star5
        {
            get { return star5; }
            set { SetProperty(ref star5, value); }
        }

        public ImageSource RatingStar1
        {
            get { return ratingStar1; }
            set { SetProperty(ref ratingStar1, value); }
        }
        public ImageSource RatingStar2
        {
            get { return ratingStar2; }
            set { SetProperty(ref ratingStar2, value); }
        }

        public ImageSource RatingStar3
        {
            get { return ratingStar3; }
            set { SetProperty(ref ratingStar3, value); }
        }

        public ImageSource RatingStar4
        {
            get { return ratingStar4; }
            set { SetProperty(ref ratingStar4, value); }
        }

        public ImageSource RatingStar5
        {
            get { return ratingStar5; }
            set { SetProperty(ref ratingStar5, value); }
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