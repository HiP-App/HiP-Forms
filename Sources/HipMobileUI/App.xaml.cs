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

using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.Common;
using de.upb.hip.mobile.pcl.Common.Contracts;
using de.upb.hip.mobile.pcl.DataAccessLayer;
using de.upb.hip.mobile.pcl.DataLayer;
using HipMobileUI.Helpers;
using HipMobileUI.Navigation;
using HipMobileUI.ViewModels.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Settings = de.upb.hip.mobile.pcl.Helpers.Settings;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace HipMobileUI
{
    public partial class App
    {
        public App()
        {
            InitializeComponent();

            // Handle when your app starts

            // set the first page that is shown
            INavigationService navigationService = IoCManager.Resolve<INavigationService> ();
            if (Settings.RepeatIntro)
            {
                navigationService.StartNewNavigationStack (new UserOnboardingPageViewModel ());
            }
            else
            {
                navigationService.StartNewNavigationStack(new LoadingPageViewModel ());
            }
        }

        protected override void OnStart()
        {
            
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            MessagingCenter.Send (this, AppSharedData.WillSleepMessage);
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
            MessagingCenter.Send<App> (this, AppSharedData.WillWakeUpMessage);
        }
    }
}
