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


using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using System;
using System.Diagnostics;
using System.Windows.Input;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages {


    public class CreateUserRatingPageViewModel : NavigationViewModel {
        private string headline;
        private string star1;
        private string star2;
        private string star3;
        private string star4;
        private string star5;

        private ICommand selectStarCommand;
        private ICommand sendRatingCommand;

        private int rating;

        public CreateUserRatingPageViewModel(Exhibit exhibit) {
            Headline = exhibit.Name;
            rating = 0;
            SetStars();
            SelectStarCommand = new Command(SelectStar);
            sendRatingCommand = new Command(SendRating);
        }

        private void SetStars() {
            Star1 = "☆";
            Star2 = "☆";
            Star3 = "☆";
            Star4 = "☆";
            Star5 = "☆";
            switch (rating) {
                case 1:
                    Star1 = "★";
                    break;
                case 2:
                    Star1 = "★";
                    Star2 = "★";
                    break;
                case 3:
                    Star1 = "★";
                    Star2 = "★";
                    Star3 = "★";
                    break;
                case 4:
                    Star1 = "★";
                    Star2 = "★";
                    Star3 = "★";
                    Star4 = "★";
                    break;
                case 5:
                    Star1 = "★";
                    Star2 = "★";
                    Star3 = "★";
                    Star4 = "★";
                    Star5 = "★";
                    break;
            }
        }

        private void SelectStar(object s) {
            rating = Convert.ToInt32(s);
            SetStars();
        }

        private void SendRating() {
            if (rating != 0) {
                UserRatingManager.GetInstance().SetUserRating();
            } else {

            }

        }

        public string Headline {
            get { return headline; }
            set { SetProperty(ref headline, value); }
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

        public ICommand SelectStarCommand {
            get { return selectStarCommand; }
            set { SetProperty(ref selectStarCommand, value); }
        }

        public ICommand SendRatingCommand {
            get { return sendRatingCommand; }
            set { SetProperty(ref sendRatingCommand, value); }
        }
    }
}
