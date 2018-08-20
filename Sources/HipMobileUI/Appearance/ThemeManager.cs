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
    }

    /// <summary>
    /// <see cref="ThemeManager"/> is supposed to handle color changes during the switch between AdventurerMode 
    /// and ProfessorMode (if <see cref="Settings.AdventurerMode"/> is false).
    /// In <see cref="ThemeManager"/> the colors PrimaryColor, PrimaryDarkColor, SecondaryColor and SecondaryDarkColor 
    /// get redefined with colors actually being used on runtime instead of the initially defined coloring in <see cref="App"/>.xaml.
    /// 
    /// Local decisions between Primary and Secondary coloring are done using <see cref="Settings.AdventurerMode"/> explicitly.
    /// </summary>
    public class ThemeManager : IThemeManager
    {
        //actual coloring used during runtime
        private static readonly Color AdventurerModeColor = Color.FromHex("FFE57F"); //light yellow "#FFE57F"
        private static readonly Color AdventurerModeColorDarker = Color.FromRgb(255, 204, 0); //dark yellow; "#FFCC00"
        private static readonly Color ProfessorModeColor = Color.FromRgb(127, 172, 255); //default light blue; "#7facff"
        private static readonly Color ProfessorModeColorDarker = Color.FromRgb(1, 73, 209); //default dark blue; "#0149D1"

        private readonly ApplicationResourcesProvider resourceProvider = IoCManager.Resolve<ApplicationResourcesProvider>();
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
        /// Adjust Theme according to current value of <see cref="Settings.AdventurerMode"/>.
        /// If true, the AdventurererMode will be enabled.
        /// If false, the ProfessorMode will be enabled.
        /// </summary>
        public void AdjustTheme()
        {
            if (Settings.AdventurerMode)
            {
                //switching colors, so we can later still use the other colors (i.e. on switching the Mode)
                resourceProvider.ChangeResourceValue("PrimaryColor", AdventurerModeColor);
                resourceProvider.ChangeResourceValue("PrimaryDarkColor", AdventurerModeColorDarker);
                resourceProvider.ChangeResourceValue("SecondaryColor", ProfessorModeColor);
                resourceProvider.ChangeResourceValue("SecondaryDarkColor", ProfessorModeColorDarker);
            }
            else
            {
                //switching colors, so we can later still use the other colors (i.e. on switching the Mode)
                resourceProvider.ChangeResourceValue("PrimaryColor", ProfessorModeColor);
                resourceProvider.ChangeResourceValue("PrimaryDarkColor", ProfessorModeColorDarker);
                resourceProvider.ChangeResourceValue("SecondaryColor", AdventurerModeColor);
                resourceProvider.ChangeResourceValue("SecondaryDarkColor", AdventurerModeColorDarker);
            }

            Color currentPrimaryDarkColor = resourceProvider.TryGetResourceColorvalue("PrimaryDarkColor");
            barsColorsChanger.ChangeToolbarColor(currentPrimaryDarkColor, currentPrimaryDarkColor);   //repaint toolbar
        }
    }
}