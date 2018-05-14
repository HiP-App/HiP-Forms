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
using PaderbornUniversity.SILab.Hip.Mobile.UI.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers;
using Xamarin.Forms;
using Settings = PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers.Settings;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Appearance
{
    public interface IThemeManager
    {
        void UpdateViewStyle(ResourceDictionary resourceDictionary, IEnumerable<string> styleProperties);
        void AdjustTheme();
        void AdjustTheme(bool adventurererMode);
    }

    public class ThemeManager : IThemeManager
    {
        private ApplicationResourcesProvider resourceProvider = IoCManager.Resolve<ApplicationResourcesProvider>();
        private readonly IBarsColorsChanger barsColorsChanger = IoCManager.Resolve<IBarsColorsChanger>();
        private string modeSuffix;

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

        private object GetThemedPropertyFor(string propertyName)
        {
            modeSuffix = Settings.AdventurerMode ? "AdventurerMode" : "ProfessorMode";
            return resourceProvider.GetResourceValue(propertyName + modeSuffix);
        }

        /// <summary>
        /// Adjust Theme according to current value of PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers.Settings.
        /// </summary>
        public void AdjustTheme()
        {
            if (Settings.AdventurerMode)
                ChangeToAdventurerTheme();
            else
                ChangeToProfessorTheme();
        }

        /// <summary>
        /// Adjust Theme according to given value in <paramref name="adventurererMode"/>.
        /// </summary>
        /// <param name="adventurererMode">True if AdventurererMode theme should be enabled. False if ProfessorMode theme should be enabled.</param>
        public void AdjustTheme(bool adventurererMode)
        {
            if (adventurererMode)
                ChangeToAdventurerTheme();
            else
                ChangeToProfessorTheme();
        }

        private void ChangeToAdventurerTheme()
        {
            //switching colors, so we can later still use the other colors (i.e. on switching the Mode)
            resourceProvider.ChangeResourceValue("PrimaryColor", Color.FromHex("FFE57F")); //light yellow #FFE57F
            resourceProvider.ChangeResourceValue("PrimaryDarkColor", Color.FromRgb(255, 204, 0)); //dark yellow; "#FFCC00"

            barsColorsChanger.ChangeToolbarColor(GetResourceColor("PrimaryDarkColor"), GetResourceColor("PrimaryDarkColor"));   //repaint toolbar
        }

        private void ChangeToProfessorTheme()
        {
            resourceProvider.ChangeResourceValue("PrimaryColor", Color.FromRgb(127, 172, 255)); //default light blue; "#7facff"
            resourceProvider.ChangeResourceValue("PrimaryDarkColor", Color.FromRgb(1, 73, 209)); //default dark blue; "#0149D1"

            barsColorsChanger.ChangeToolbarColor(GetResourceColor("PrimaryDarkColor"), GetResourceColor("PrimaryDarkColor"));  //repaint toolbar
        }

        private Color GetResourceColor(string color)
        {
            return (Color)resourceProvider.GetResourceValue(color);
        }
    }
}