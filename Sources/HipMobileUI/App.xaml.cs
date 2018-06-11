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

using System.Diagnostics;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Appearance;
using PaderbornUniversity.SILab.Hip.Mobile.UI.DesignTime;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Settings = PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers.Settings;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace PaderbornUniversity.SILab.Hip.Mobile.UI
{
    public partial class App
    {
        public App()
        {
            DesignMode.Initialize(InitializeComponent);

            if (DesignMode.IsEnabled)
                return;

            // Handle when your app starts
            Settings.AdventurerMode = Settings.AdventurerMode && !Settings.DisableAdventurerMode;

            // setup content for being able to use consistent dynamic coloring
            IoCManager.RegisterInstance(typeof(ApplicationResourcesProvider), new ApplicationResourcesProvider(Application.Current.Resources.ToDictionary(x => x.Key, x => x.Value)));
            IoCManager.RegisterInstance(typeof(IThemeManager), new ThemeManager());

            // set the first page that is shown
            var navigationService = IoCManager.Resolve<INavigationService>();
            if (Settings.RepeatIntro)
            {
                navigationService.StartNewNavigationStack(new UserOnboardingPageViewModel());
            }
            else
            {
                navigationService.StartNewNavigationStack(new LoadingPageViewModel());
            }
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            MessagingCenter.Send(this, AppSharedData.WillSleepMessage);
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
            MessagingCenter.Send(this, AppSharedData.WillWakeUpMessage);
        }
    }
}