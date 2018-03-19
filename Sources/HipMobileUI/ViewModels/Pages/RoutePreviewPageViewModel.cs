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
using System.Windows.Input;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Location;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages
{
    public class RoutePreviewPageViewModel : NavigationViewModel
    {
        public RoutePreviewPageViewModel(Route route, INearbyRouteManager nearbyRouteManager)
        {
            RouteTitle = route.Name;
            RouteId = route.Id;
            Question = Strings.ExhibitOrRouteNearby_Question_Part1 + " \"" + RouteTitle + "\" " + Strings.ExhibitOrRouteNearby_Question_Part2;
            RouteDescription = route.Description;
            var data = route.Image.GetDataBlocking();
            Image = ImageSource.FromStream(() => new MemoryStream(data));
            NearbyRouteManager = nearbyRouteManager;

            Confirm = new Command(Accept);
            Decline = new Command(Deny);
        }

        private INearbyRouteManager NearbyRouteManager { get; set; }

        public string Question { set; get; }

        public string RouteTitle { get; set; }

        public string RouteId { get; set; }

        public string RouteDescription { get; set; }

        public ImageSource Image { set; get; }

        public ICommand Confirm { get; }

        public ICommand Decline { get; }

        private void Accept()
        {
            Navigation.ClearModalStack();
            NearbyRouteManager.OpenRouteDetailsView(RouteId);
        }

        private void Deny()
        {
            NearbyRouteManager.ClosePopUp();
        }
    }
}