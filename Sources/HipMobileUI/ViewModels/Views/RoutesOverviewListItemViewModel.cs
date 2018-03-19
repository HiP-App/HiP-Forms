// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
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

using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views
{
    /// <summary>
    /// View model of a list item in the <see cref="RoutesOverviewViewModel"/> screen
    /// </summary>
    public class RoutesOverviewListItemViewModel : NavigationViewModel, IDownloadableListItemViewModel
    {
        /// <summary>
        /// Route data displayed by this list item
        /// </summary>
        public Route Route { get; }

        private ExhibitRouteDownloadPageViewModel downloadPage;

        /// <summary>
        /// Creates a list item using the provided route data
        /// </summary>
        /// <param name="route"></param>
        public RoutesOverviewListItemViewModel(Route route)
        {
            Route = route;
            RouteTitle = Route.Name;
            RouteDescription = Route.Description;
            Duration = GetRouteDurationText(Route.Duration);
            Distance = GetRouteDistanceText(Route.Distance);

            IsDownloadPanelVisible = !Route.DetailsDataLoaded;

            Tags = new ObservableCollection<ImageSource>();

            foreach (var tag in Route.Tags)
            {
                // Required to reference first due to threading problems in Realm
                byte[] currentTagImageData = tag.Image.GetDataBlocking();

                Tags.Add(ImageSource.FromStream(() => new MemoryStream(currentTagImageData)));
            }

            // Required to reference first due to threading problems in Realm
            var imageData = Route.Image.GetDataBlocking();
            Image = ImageSource.FromStream(() => new MemoryStream(imageData));

            DownloadCommand = new Command(OpenDownloadDialog);
        }

        public ICommand DownloadCommand { get; set; }

        public string GetRouteDistanceText(double routeDistance)
        {
            return string.Format(Strings.RoutesOverviewListItemViewModel_Distance, routeDistance);
        }

        public string GetRouteDurationText(int routeDuration)
        {
            var durationInMinutes = routeDuration / 60;
            return string.Format(Strings.RoutesOverviewListItemViewModel_Duration, durationInMinutes);
        }

        private async void OpenDownloadDialog()
        {
            downloadPage = new ExhibitRouteDownloadPageViewModel(Route, this);
            await Navigation.PushAsync(downloadPage);
        }

        public void CloseDownloadPage()
        {
            IoCManager.Resolve<INavigationService>().PopAsync();
        }

        public async void OpenDetailsView(string id)
        {
            Navigation.InsertPageBefore(new RouteDetailsPageViewModel(id), downloadPage);
            await Navigation.PopAsync();
        }

        public void SetDetailsAvailable(bool available)
        {
            if (!available)
                return;

            DbManager.InTransaction(transaction => { Route.DetailsDataLoaded = true; });

            IsDownloadPanelVisible = !Route.DetailsDataLoaded;
        }

        private ImageSource image;

        public ImageSource Image
        {
            get { return image; }
            set { SetProperty(ref image, value); }
        }

        private string routeTitle;

        public string RouteTitle
        {
            get { return routeTitle; }
            set { SetProperty(ref routeTitle, value); }
        }

        private string routeDescription;

        public string RouteDescription
        {
            get { return routeDescription; }
            set { SetProperty(ref routeDescription, value); }
        }

        private string duration;

        public string Duration
        {
            get { return duration; }
            set { SetProperty(ref duration, value); }
        }

        private string distance;

        public string Distance
        {
            get { return distance; }
            set { SetProperty(ref distance, value); }
        }

        private ObservableCollection<ImageSource> tags;

        public ObservableCollection<ImageSource> Tags
        {
            get { return tags; }
            set { SetProperty(ref tags, value); }
        }

        private bool isDownloadPanelVisible;

        public bool IsDownloadPanelVisible
        {
            get { return isDownloadPanelVisible; }
            set { SetProperty(ref isDownloadPanelVisible, value); }
        }
    }
}