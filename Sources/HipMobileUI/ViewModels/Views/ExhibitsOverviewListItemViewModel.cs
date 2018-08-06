// Copyright (C) 2016 History in Paderborn App - Universität Paderborn
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using Plugin.Geolocator.Abstractions;
using System.IO;
using System.Windows.Input;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views
{
    public class ExhibitsOverviewListItemViewModel : NavigationViewModel, IDownloadableListItemViewModel
    {
        public ExhibitsOverviewListItemViewModel(Exhibit exhibit, double distance = -1)
        {
            Exhibit = exhibit;
            Distance = distance;
            var data = Exhibit.Image.GetDataBlocking();
            Image = ImageSource.FromStream(() => new MemoryStream(data));
            Rating = new RatingViewModel(exhibit, false, false);

            ItemTappedCommand = new Command(NavigateToExhibitDetails);
        }

        private double distance;
        private ExhibitRouteDownloadPageViewModel downloadPage;

        /// <summary>
        /// The exhibit.
        /// </summary>
        public Exhibit Exhibit { get; }


        /// <summary>
        /// The appetizer image for the exhibit.
        /// </summary>
        public ImageSource Image { get; }

        public RatingViewModel Rating { get; }

        /// <summary>
        /// The distance to the exhibit.
        /// </summary>
        public double Distance
        {
            get => distance;
            set
            {
                if (SetProperty(ref distance, value))
                {
                    OnPropertyChanged(nameof(FormattedDistance));
                }
            }
        }

        /// <summary>
        /// The formatted distance string.
        /// </summary>

        public string FormattedDistance => (Distance < 1000)
                ? $"{Distance:F0} m"
                : $"{Distance / 1000:0.##} km";


        /// <summary>
        /// Update the displayed distance according to the position.
        /// </summary>
        /// <param name="position">The new position from which the distance is measured.</param>
        public void UpdateDistance(Position position)
        {
            Distance = MathUtil.CalculateDistance(Exhibit.Location, new GeoLocation(position.Latitude, position.Longitude));
        }

        public async void OpenDetailsView(string id)
        {
            Navigation.InsertPageBefore(new AppetizerPageViewModel(id), downloadPage);
            await Navigation.PopAsync();
        }

        /// <summary>
        /// Open the exhibitdetails page.
        /// </summary>
        /// <param name="item"></param>
        private async void NavigateToExhibitDetails()
        {
            await Navigation.PushAsync(new AppetizerPageViewModel(Exhibit));
        }


        public async Task SetDetailsAvailable(bool available)
        {
            if (!available)
                return;

            DbManager.InTransaction(transaction => { Exhibit.DetailsDataLoaded = true; });
        }


        public void CloseDownloadPage()
        {
            IoCManager.Resolve<INavigationService>().PopAsync();
        }

        public override bool Equals(object obj) =>
            obj is ExhibitsOverviewListItemViewModel otherItem &&
            Exhibit.Name == otherItem.Exhibit.Name;

        public override int GetHashCode() => Exhibit.Name.GetHashCode();

        /// <summary>
        /// The command for tapping on exhibits.
        /// </summary>
        public ICommand ItemTappedCommand { get; }
    }
}