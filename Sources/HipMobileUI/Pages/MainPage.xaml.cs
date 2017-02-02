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
using HipMobileUI.Navigation;
using HipMobileUI.ViewModels.Pages;
using Xamarin.Forms;

namespace HipMobileUI.Pages
{
    public partial class MainPage : IViewFor<MainPageViewModel>
    {
        private bool isShown = true;
        private Page page;

        private MainPageViewModel ViewModel => ((MainPageViewModel) BindingContext);

        public MainPage()
        {
            InitializeComponent();
            ViewModel.SelectedViewModel = ViewModel.MainScreenViewModels[0];
        }

        private void ListView_OnItemTapped (object sender, ItemTappedEventArgs e)
        {
            if (Device.Idiom == TargetIdiom.Phone)
            {
                IsPresented = false;
            }
        }

        private void NavigationPage_OnPushed (object sender, NavigationEventArgs e)
        {
            // Disable the swipe gesture when a page is pushed
            IsGestureEnabled = false;
            if (isShown)
            {
                page = e.Page;
                page.Disappearing += PageOnDisappearing;
            }
        }

        private void PageOnDisappearing(object sender, EventArgs eventArgs)
        {
            // enable the swipe gesture once this page becomes visible again
            IsGestureEnabled = true;
            page.Disappearing -= PageOnDisappearing;
            page = null;
        }

    }
}
