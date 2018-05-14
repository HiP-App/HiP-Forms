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

using System.Windows.Input;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using Xamarin.Forms;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Appearance;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers;
using Settings = PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers.Settings;
using System;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages
{
    public class CharacterDetailsPageViewModel : NavigationViewModel
    {
        private readonly NavigationViewModel parentViewModel;

        public CharacterDetailsPageViewModel(NavigationViewModel parentViewModel, bool adventurerModeSelected)
        {
            this.parentViewModel = parentViewModel;
            AdventurerModeSelected = adventurerModeSelected;

            UpdateView();

            SelectModeCommand = new Command(OnSelectModeTapped);
            ChangeModeCommand = new Command(OnChangeModeTapped);
        }

        public ICommand SelectModeCommand { get; }
        public ICommand ChangeModeCommand { get; }

        private bool adventurerModeSelected;
        public bool AdventurerModeSelected
        {
            get => adventurerModeSelected;
            set => SetProperty(ref adventurerModeSelected, value);
        }
        
        private void OnSelectModeTapped()
        {
            Settings.AdventurerMode = AdventurerModeSelected;

            AdjustThemeAndContinue();
        }

        private void OnChangeModeTapped()
        {
            AdventurerModeSelected = !AdventurerModeSelected;
            UpdateView();
        }

        private void UpdateView()
        {
            //Settings.AdventurerMode = AdventurerModeSelected;
            IoCManager.Resolve<IThemeManager>().AdjustTheme(AdventurerModeSelected);

            Helpers.ApplicationResourcesProvider resources = IoCManager.Resolve<ApplicationResourcesProvider>();
            //string color = AdventurerModeSelected ? (string)resources.GetResourceValue("SecondaryDarkColor") : (string)resources.GetResourceValue("PrimaryDarkColor"); //for painting it dark yellow or dark blue
            //var color = AdventurerModeSelected ? resources.TryGetResourceColorvalue("PrimaryDarkColor") : resources.TryGetResourceColorvalue("SecondaryDarkColor"); //Settings.AdventurerMode not up to date if ChangeModeTapped and not yet selected
            var color = resources.TryGetResourceColorvalue("PrimaryDarkColor");
            int colorRed = (int)(color.R * 255);
            int colorGreen = (int)(color.G * 255);
            int colorBlue = (int)(color.B * 255);
            string colorHex = String.Format("#{0:X2}{1:X2}{2:X2}", colorRed, colorGreen, colorBlue);

            PageTitle = AdventurerModeSelected ? "Adventurer" : "Professor";
            Image = AdventurerModeSelected ? ImageSource.FromFile("ic_adventurer.png") : ImageSource.FromFile("ic_professor.png");
            MainScreenColor = colorHex;
            SelectModeButton = colorHex;
            ChangeModeButton = colorHex;
            PageDetails = AdventurerModeSelected ? Strings.CharacterDetailsPage_Adventurer_Text : Strings.CharacterDetailsPage_Professor_Text;
        }

        private void AdjustThemeAndContinue()
        {
            SwitchToNextPage();

            // Make sure all related components are already initialized before adjusting theme
            if (Settings.InitialThemeSelected)
                IoCManager.Resolve<IThemeManager>().AdjustTheme();
        }

        /// <summary>
        /// Switches to the next page after a character has been selected.
        /// If the parent view is the Settings- or ProfileScreenView, the next page is the previous page.
        /// </summary>
        private void SwitchToNextPage()
        {
            if (parentViewModel.GetType() == typeof(UserOnboardingPageViewModel))
            {
                Navigation.StartNewNavigationStack(new LoadingPageViewModel());
            }
            else if (parentViewModel.GetType() == typeof(ProfileScreenViewModel))
            {
                var mainPageViewModel = new MainPageViewModel();
                Navigation.StartNewNavigationStack(mainPageViewModel);
                mainPageViewModel.SwitchToProfileView();
            }
            else if (parentViewModel.GetType() == typeof(SettingsScreenViewModel))
            {
                var mainPageViewModel = new MainPageViewModel();
                Navigation.StartNewNavigationStack(mainPageViewModel);
                mainPageViewModel.SwitchToSettingsScreenView();
            }
        }

        private ImageSource image;
        public ImageSource Image
        {
            get => image;
            set => SetProperty(ref image, value);

        }
        private string mainScreenColor;
        public string MainScreenColor
        {
            get => mainScreenColor;
            set => SetProperty(ref mainScreenColor, value);

        }
        private string selectModeButton;
        public string SelectModeButton
        {
            get => selectModeButton;
            set => SetProperty(ref selectModeButton, value);
        }
        private string changeModeButton;
        public string ChangeModeButton
        {
            get => changeModeButton;
            set => SetProperty(ref changeModeButton, value);
        }
        private string pageTitle;
        public string PageTitle
        {
            get => pageTitle;
            set => SetProperty(ref pageTitle, value);

        }
        private string pageDetails;
        public string PageDetails
        {
            get => pageDetails;
            set => SetProperty(ref pageDetails, value);
        }
    }
}