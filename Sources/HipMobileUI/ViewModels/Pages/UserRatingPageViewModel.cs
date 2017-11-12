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
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages {
    public class UserRatingPageViewModel : NavigationViewModel {

        private Exhibit exhibit;
        public Exhibit Exhibit {
            get { return exhibit; }
            set { SetProperty(ref exhibit, value); }
        }

        /// <summary>
        /// The headline of the description.
        /// </summary>
        private string headline;
        public string Headline {
            get { return headline; }
            set { SetProperty(ref headline, value); }
        }

        private ObservableCollection<UserRatingListItemViewModel> ratingsList;
        public ObservableCollection<UserRatingListItemViewModel> RatingsList {
            get { return ratingsList; }
            set { SetProperty(ref ratingsList, value); }
        }

        private ICommand refreshCommand;
        public ICommand RefreshCommand {
            get { return refreshCommand; }
            set { SetProperty(ref refreshCommand, value); }
        }

        private bool isRefreshing;
        public bool IsRefreshing {
            get { return isRefreshing; }
            set { SetProperty(ref isRefreshing, value); }
        }

        private ICommand itemSelectedCommand;
        public ICommand ItemSelectedCommand {
            get { return itemSelectedCommand; }
            set { SetProperty(ref itemSelectedCommand, value); }
        }

        private ICommand createRatingCommand;
        public ICommand CreateRatingCommand {
            get { return createRatingCommand; }
            set { SetProperty(ref createRatingCommand, value); }
        }

        public UserRatingPageViewModel(string exhibitId) : this(ExhibitManager.GetExhibit(exhibitId)) { }


        public UserRatingPageViewModel(Exhibit exhibit) {
            Exhibit = exhibit;
            Headline = exhibit.Name;

            RatingsList = new ObservableCollection<UserRatingListItemViewModel>();
            for (int i = 0; i < 50; i++) {
                var listItem = new UserRatingListItemViewModel(exhibit);
                RatingsList.Add(listItem);
            }
            CreateRatingCommand = new Command(OpenCreateRatingPage);
            RefreshCommand = new Command(RefreshUserRatingList);
            ItemSelectedCommand = new Command(OnItemSelected);

        }

        private async void OpenCreateRatingPage() {
            await Navigation.PushAsync(new CreateUserRatingPageViewModel());
            // Open the download dialog
            //downloadPage = new ExhibitRouteDownloadPageViewModel(Exhibit, this);
            //await Navigation.PushAsync(downloadPage);

        }

        public void CloseDownloadPage() {
            //Navigation.PopAsync();
        }

        private void RefreshUserRatingList() {
            IsRefreshing = false;
        }

        private void OnItemSelected() {

        }
    }
}