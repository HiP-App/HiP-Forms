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

using System.IO;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using MvvmHelpers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Location;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using Plugin.Geolocator.Abstractions;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views
{
    class ExhibitsOverviewListItemViewModel : BaseViewModel, DownloadableListItemViewModel
    {

        public ExhibitsOverviewListItemViewModel (Exhibit exhibit, double distance = -1)
        {
            ExhibitName = exhibit.Name;
            Distance = distance;
            var data = exhibit.Image.Data;
            Image = ImageSource.FromStream (() => new MemoryStream (data));
            Exhibit = exhibit;
        }

        private Exhibit exhibit;
        private string exhibitName;
        private double distance;
        private ImageSource image;

        /// <summary>
        /// The name of the exhibit.
        /// </summary>
        public string ExhibitName {
            get { return exhibitName; }
            set { SetProperty (ref exhibitName, value); }
        }

        /// <summary>
        /// The distance to the exhibit.
        /// </summary>
        public double Distance {
            get { return distance; }
            set {
                if (SetProperty (ref distance, value))
                {
                    OnPropertyChanged (nameof (FormatedDistance));
                }
            }
        }

        /// <summary>
        /// The appetizer image for teh exhibit.
        /// </summary>
        public ImageSource Image {
            get { return image; }
            set { SetProperty (ref image, value); }
        }

        /// <summary>
        /// The id of the exhibit.
        /// </summary>
        public Exhibit Exhibit {
            get { return exhibit; }
            set { SetProperty (ref exhibit, value); }
        }

        /// <summary>
        /// The Formated distance string.
        /// </summary>
        public string FormatedDistance {
            get {
                if (Distance < 1000)
                {
                    return $"{Distance:F0} m";
                }
                else
                {
                    return $"{Distance/1000:0.##} km";
                }
            }
        }

        /// <summary>
        /// Update the displayed distance according to the position.
        /// </summary>
        /// <param name="position">The new position from which the distance is measured.</param>
        public void UpdateDistance (Position position)
        {
            Distance = MathUtil.CalculateDistance (exhibit.Location, new GeoLocation(position.Latitude,position.Longitude));
        }

        public void CloseDownloadPage()
        {
            IoCManager.Resolve<INavigationService>().PopModalAsync();
        }
        public void OpenDetailsView(string id)
        {
            // IoCManager.Resolve<INavigationService>().PushAsync(new InsertYourDetailsViewModelHere(id));
        }
        public void SetDetailsAvailable(bool available)
        {
            if (!available)
                return;

            // Add the changes you make after the download is finished here; below is the code I execute as a dummy in my corresponding class
            //using (DbManager.StartTransaction())
            //{
            //    Route.DetailsDataLoaded = true;   // Details loaded
            //}
            //IsDownloadPanelVisible = !Route.DetailsDataLoaded;    // Download-button is now hidden
        }


        public override bool Equals (object obj)
        {
            var otherItem = obj as ExhibitsOverviewListItemViewModel;
            if (otherItem != null)
            {
                return ExhibitName.Equals (otherItem.ExhibitName);
            }
            return base.Equals (obj);
        }

    }
}
