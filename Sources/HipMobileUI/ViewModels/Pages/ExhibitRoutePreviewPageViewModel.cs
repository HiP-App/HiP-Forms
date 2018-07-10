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

using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.DtoToModelConverters;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Location;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages
{
    public class ExhibitRoutePreviewPageViewModel : NavigationViewModel, IDownloadableListItemViewModel
    {
        private readonly Exhibit exhibit;
        private readonly Route route;

        public ExhibitRoutePreviewPageViewModel(Exhibit exhibit, INearbyExhibitManager nearbyExhibitManager)
        {
            this.exhibit = exhibit;
            Question = Strings.ExhibitOrRouteNearby_Question_Part1 + " \"" + this.exhibit.Name + "\" " + Strings.ExhibitOrRouteNearby_Question_Part2;
            SetImage(exhibit.Image);

            NearbyExhibitManager = nearbyExhibitManager;
            ExhibitRouteNearby = Strings.ExhibitRoutePreviewPage_Exhibit_Nearby_Title;
            ExhibitRouteTitle = exhibit.Name;
            Description = exhibit.Description == "" ? exhibit.Name : exhibit.Description;

            Confirm = new Command(AcceptExhibit);
            Decline = new Command(Deny);
        }

        public ExhibitRoutePreviewPageViewModel(Route route, INearbyRouteManager nearbyRouteManager)
        {
            this.route = route;

            Question = Strings.ExhibitOrRouteNearby_Question_Part1 + " \"" + route.Name + "\" " + Strings.ExhibitOrRouteNearby_Question_Part2;
            SetImage(route.Image);

            NearbyRouteManager = nearbyRouteManager;
            ExhibitRouteNearby = Strings.ExhibitRoutePreviewPage_Route_Nearby_Title;
            ExhibitRouteTitle = route.Name;
            Description = route.Description == "" ? route.Name : route.Description;

            Confirm = new Command(AcceptRoute);
            Decline = new Command(Deny);
        }

        private async void SetImage(Shared.BusinessLayer.Models.Image image)
        {
            var imageData = await image.GetDataAsync();
            Image = imageData != null ? ImageSource.FromStream(() => new MemoryStream(imageData)) : ImageSource.FromStream(() => new MemoryStream(BackupData.BackupImageData));
        }

        private INearbyExhibitManager NearbyExhibitManager { get; set; }

        private INearbyRouteManager NearbyRouteManager { get; set; }

        public string ExhibitRouteNearby { set; get; }
        public string Description { set; get; }
        public string Question { set; get; }
        public ImageSource Image { set; get; }
        public ICommand Confirm { get; }
        public ICommand Decline { get; }
        public string ExhibitRouteTitle { get; }

        private async void AcceptExhibit()
        {
            MessagingCenter.Send<NavigationViewModel, bool>(this, "ReturnValue", true);
            await Navigation.ClearModalStack();
            await Navigation.PushAsync(new AppetizerPageViewModel(exhibit));
            NearbyExhibitManager.InvokeExhibitVisitedEvent(exhibit);
        }

        private async void AcceptRoute()
        {
            await Navigation.ClearModalStack();
            if (route.DetailsDataLoaded)
            {
                NearbyRouteManager.OpenRouteDetailsView(route.Id);
            }
            else
            {
                await Navigation.PushAsync(new ExhibitRouteDownloadPageViewModel(route, this));
            }
        }

        private async void Deny()
        {
            await Navigation.PopModalAsync();
        }

        public async void CloseDownloadPage()
        {
            await Navigation.PopAsync();
        }

        public async void OpenDetailsView(string id)
        {
            await Navigation.PopAsync();
            NearbyRouteManager.OpenRouteDetailsView(route.Id);
        }

        public async Task SetDetailsAvailable(bool available)
        {
            if (!available)
                return;

            DbManager.InTransaction(transaction =>
            {
                route.DetailsDataLoaded = true;
            });
        }
    }
}