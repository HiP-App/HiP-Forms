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
        void AdjustTopBarTheme();
        object GetThemedPropertyFor(string propertyName);
    }

    public class ThemeManager : IThemeManager
    {
        private readonly ApplicationResourcesProvider resourceProvider = IoCManager.Resolve<ApplicationResourcesProvider>();
        private string modeSuffix;

        public void AdjustTopBarTheme()
        {
            if (Settings.AdventurerMode)
                ChangeToAdventurerTheme();
            else
                ChangeToProfessorTheme();
        }

        public object GetThemedPropertyFor(string propertyName)
        {
            modeSuffix = Settings.AdventurerMode ? "AdventurerMode" : "ProfessorMode";
            return resourceProvider.GetResourceValue(propertyName + modeSuffix);
        }

        private void ChangeToAdventurerTheme()
        {
            IoCManager.Resolve<IBarsColorsChanger>().ChangeToolbarColor(GetResourceColor("AccentDarkColor"), GetResourceColor("AccentColor"));
        }

        private void ChangeToProfessorTheme()
        {
            IoCManager.Resolve<IBarsColorsChanger>().ChangeToolbarColor(GetResourceColor("PrimaryDarkColor"), GetResourceColor("PrimaryColor"));
        }

        private Color GetResourceColor(string color)
        {
            return (Color)resourceProvider.GetResourceValue(color);
        }
    }
}