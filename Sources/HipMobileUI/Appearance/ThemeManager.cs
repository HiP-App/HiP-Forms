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
        void ChangeToProfessorTheme();
        void AdjustThemeToCurrentCharacter();
    }

    public class ThemeManager : IThemeManager
    {
        private readonly ApplicationResourcesProvider resourceProvider = IoCManager.Resolve<ApplicationResourcesProvider>();

        public void AdjustThemeToCurrentCharacter()
        {
            if (Settings.AdventurerMode)
                ChangeToAdventurerTheme();
            else
                ChangeToProfessorTheme();
        }

        public void ChangeToAdventurerTheme()
        {
            IoCManager.Resolve<IBarsColorsChanger>().ChangeToolbarColor(GetResourceColor("AccentDarkColor"), GetResourceColor("AccentColor"));
            resourceProvider.SetResourceValue("BoxViewColor", "BoxViewColorAdventurerMode");
        }

        public void ChangeToProfessorTheme()
        {
            IoCManager.Resolve<IBarsColorsChanger>().ChangeToolbarColor(GetResourceColor("PrimaryDarkColor"), GetResourceColor("PrimaryColor"));
            resourceProvider.SetResourceValue("BoxViewColor", "BoxViewColorProfessorMode");
        }

        private Color GetResourceColor(string color)
        {
            return (Color)resourceProvider.GetResourceValue(color);
        }
    }
}