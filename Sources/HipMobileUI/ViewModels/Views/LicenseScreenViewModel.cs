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

using System;
using System.Windows.Input;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views
{
    public class LicenseScreenViewModel : NavigationViewModel
    {
        public LicenseScreenViewModel()
        {
            UnlockExhibits = new Command(Unlock);
        }

        private int tapCounter;

        /// <summary>
        /// Unlocks all exhibits for testing purposes
        /// </summary>
        private async void Unlock()
        {
            tapCounter++;

            if (tapCounter == 10)
            {
                using (DbManager.StartTransaction())
                {
                    foreach (var exhibit in ExhibitManager.GetExhibits())
                    {
                        exhibit.Unlocked = true;
                    }
                }
                await Navigation.DisplayAlert(Strings.LicenseScreenViewModel_UnlockExhibits_Title,
                                              Strings.LicenseScreenViewModel_UnlockExhibits_Text, Strings.LicenseScreenViewModel_UnlockExhibits_Confirm);
            }
        }

        public ICommand UnlockExhibits { get; }
    }
}