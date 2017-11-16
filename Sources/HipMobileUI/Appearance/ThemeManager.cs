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

using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Appearance
{
    public interface IThemeManager
    {
        void ChangeToAdventurerTheme();
        void ChangeToExplorerTheme();
        void AdjustThemeToCurrentCharacter();
    }

    public class ThemeManager : IThemeManager
    {
        private readonly ApplicationResourcesProvider ressourceProvider = IoCManager.Resolve<ApplicationResourcesProvider>();

        public void AdjustThemeToCurrentCharacter()
        {
            if (Settings.AdventurerMode)
                ChangeToAdventurerTheme();
            else
                ChangeToExplorerTheme();
        }

        public void ChangeToAdventurerTheme()
        {
            IoCManager.Resolve<IBarsColorsChanger>().ChangeToolbarColor(GetRessourceColor("AccentDarkColor"), GetRessourceColor("AccentColor"));
        }

        public void ChangeToExplorerTheme()
        {
            IoCManager.Resolve<IBarsColorsChanger>().ChangeToolbarColor(GetRessourceColor("PrimaryDarkColor"), GetRessourceColor("PrimaryColor"));
        }

        private Color GetRessourceColor(string color)
        {
            return (Color)ressourceProvider.GetResourceValue(color);
        }
    }
}