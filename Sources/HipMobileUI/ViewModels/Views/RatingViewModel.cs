using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Acr.UserDialogs;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views
{
    public class RatingViewModel : NavigationViewModel
    {
        private Exhibit exhibit;

        private ICommand userRatingCommand;
        private string ratingAverage;
        private ImageSource star1;
        private ImageSource star2;
        private ImageSource star3;
        private ImageSource star4;
        private ImageSource star5;
        private string ratingCount;
        private string horizontal;
        private string vertical;
        private const string ImgStarEmpty = "star_empty.png";
        private const string ImgStarHalfFilled = "star_half_filled.png";
        private const string ImgStarFilled = "star_filled.png";

        public RatingViewModel(Exhibit exhibit, bool isRatingAvailable, bool isHorizontal)
        {

            Exhibit = exhibit;
            if (isRatingAvailable)
            {
                UserRatingCommand = new Command(GoToUserRatingPage);
            }

            if (isHorizontal)
            {
                Horizontal = "True";
                Vertical = "False";
            }
            else
            {
                Horizontal = "False";
                Vertical = "True";
            }
            RefreshUserRating();
        }


        private async void GoToUserRatingPage()
        {
            if (IoCManager.Resolve<INetworkAccessChecker>().GetNetworkAccessStatus() != NetworkAccessStatus.NoAccess)
            {
                await Navigation.PushAsync(new UserRatingPageViewModel(Exhibit));
            }
            else
            {
                UserDialogs.Instance.Alert(new AlertConfig()
                {
                    Title = Strings.UserRating_Dialog_Title_No_Internet,
                    Message = Strings.UserRating_Dialog_Message_No_Internet,
                    OkText = Strings.Ok
                });
            }
        }
        private async void RefreshUserRating()
        {
            if (IoCManager.Resolve<INetworkAccessChecker>().GetNetworkAccessStatus() == NetworkAccessStatus.NoAccess)
            {
                RatingAverage = "-";
                SetStarImages(0);
                RatingCount = "- " + Strings.UserRating_Rate_Count;
                UserDialogs.Instance.Alert(new AlertConfig()
                {
                    Title = Strings.UserRating_Dialog_Title_No_Internet,
                    Message = Strings.UserRating_Dialog_Message_No_Internet,
                    OkText = Strings.Ok
                });
            }
            else
            {
                var userRating = await IoCManager.Resolve<IUserRatingManager>().GetUserRatingAsync(exhibit.IdForRestApi);
                RatingAverage = userRating.Count > 0 ? userRating.Average.ToString("0.#") : "-";
                SetStarImages(userRating.Average);
                RatingCount = userRating.Count + " " + Strings.UserRating_Rate_Count;
            }
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
        public ICommand UserRatingCommand
        {
            get { return userRatingCommand; }
            set { SetProperty(ref userRatingCommand, value); }
        }

        public Exhibit Exhibit
        {
            get { return exhibit; }
            set { SetProperty(ref exhibit, value); }
        }

        public string Horizontal
        {
            get { return horizontal; }
            set { SetProperty(ref horizontal, value); }
        }

        public string Vertical
        {
            get { return vertical; }
            set { SetProperty(ref vertical, value); }
        }
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
    }
}
