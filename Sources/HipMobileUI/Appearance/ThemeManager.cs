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

using System.Collections.Generic;
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
        void UpdateViewStyle(ResourceDictionary resourceDictionary, IEnumerable<string> styleProperties);
        object GetThemedPropertyFor(string propertyName);
    }

    public class ThemeManager : IThemeManager
    {
        private readonly ApplicationResourcesProvider resourceProvider = IoCManager.Resolve<ApplicationResourcesProvider>();
        private readonly IBarsColorsChanger barsColorsChanger = IoCManager.Resolve<IBarsColorsChanger>();
        private string modeSuffix;

        public void AdjustTopBarTheme()
        {
            if (Settings.AdventurerMode)
                ChangeToAdventurerTheme();
            else
                ChangeToProfessorTheme();
        }

        /// <summary>
        /// Updates the style of the view this method was called from to fit the selected mode.
        /// </summary>
        /// <param name="resourceDictionary">The resourceDictionary of the view.</param>
        /// <param name="styleProperties">Collection of properties of the view influenced by a mode change.</param>
        public void UpdateViewStyle(ResourceDictionary resourceDictionary, IEnumerable<string> styleProperties)
        {
            if (resourceDictionary == null)
                return;

            foreach (var styleProperty in styleProperties)
            {
                resourceDictionary.Add(styleProperty, GetThemedPropertyFor(styleProperty));
            }
        }

        public object GetThemedPropertyFor(string propertyName)
        {
            modeSuffix = Settings.AdventurerMode ? "AdventurerMode" : "ProfessorMode";
            return resourceProvider.GetResourceValue(propertyName + modeSuffix);
        }

        private void ChangeToAdventurerTheme()
        {
            barsColorsChanger.ChangeToolbarColor(GetResourceColor("AccentDarkColor"), GetResourceColor("AccentColor"));
        }

        private void ChangeToProfessorTheme()
        {
            barsColorsChanger.ChangeToolbarColor(GetResourceColor("PrimaryDarkColor"), GetResourceColor("PrimaryColor"));
        }

        private Color GetResourceColor(string color)
        {
            return (Color)resourceProvider.GetResourceValue(color);
        }
    }
}