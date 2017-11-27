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
using Xamarin.Forms;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Location;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views
{
    class ExhibitPreviewViewModel : NavigationViewModel
    {
        public ExhibitPreviewViewModel(Exhibit exhibit, INearbyExhibitManager exManager)
        {
            this.exhibit = exhibit;
            Question = Strings.ExhibitOrRouteNearby_Question_Part1 + " \"" + this.exhibit.Name + "\" " + Strings.ExhibitOrRouteNearby_Question_Part2;
            var data = exhibit.Image.GetDataBlocking();
            Image = ImageSource.FromStream(() => new MemoryStream(data));

            Confirm = new Command(Accept);
            Decline = new Command(Deny);

            exhibitManager = exManager;
            ExhibitName = exhibit.Name;
        }

        private readonly Exhibit exhibit;

        private readonly INearbyExhibitManager exhibitManager;
        public string Question { set; get; }
        public ImageSource Image { set; get; }
        public ICommand Confirm { get; }
        public ICommand Decline { get; }

        public string ExhibitName { get; }

        async void Accept()
        {
            MessagingCenter.Send<NavigationViewModel, bool>(this, "ReturnValue", true);
            await Navigation.ClearModalStack();
            await Navigation.PushAsync(new AppetizerViewModel(exhibit));
            exhibitManager.InvokeExhibitVistedEvent(exhibit);
        }

        async void Deny()
        {
            await Navigation.PopModalAsync();
        }
    }
}