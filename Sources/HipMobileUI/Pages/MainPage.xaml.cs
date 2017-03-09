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
using HipMobileUI.ViewModels;
using HipMobileUI.ViewModels.Pages;
using Xamarin.Forms;

namespace HipMobileUI.Pages
{
    public partial class MainPage : IViewFor<MainPageViewModel>
    {

        private MainPageViewModel ViewModel => ((MainPageViewModel) BindingContext);

        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged ()
        {
            base.OnBindingContextChanged ();
            ViewModel.SelectedViewModel = ViewModel.MainScreenViewModels[0];
        }

        /// <summary>
        /// Hide the menu on phones once menu item was tapped.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_OnItemTapped (object sender, ItemTappedEventArgs e)
        {
            if (Device.Idiom == TargetIdiom.Phone)
            {
                IsPresented = false;
            }
        }

        /// <summary>
        /// New page pushed to the navigation stack.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event parameters</param>
        private void NavigationPage_OnPushed (object sender, NavigationEventArgs e)
        {
            // Disable the swipe gesture when a page is pushed
            IsGestureEnabled = false;
        }

        /// <summary>
        /// page is popped from the navigation stack.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event parameters.</param>
        private void NavigationPage_OnPopped (object sender, NavigationEventArgs e)
        {
            // enable the swipe gesture once this page becomes visible again
            if (NavigationPage.CurrentPage == ContentPage)
            {
                IsGestureEnabled = true;
            }

            // inform the popped page in case it can listen to this event
            if (e.Page is IPagePoppedListener)
            {
                ((IPagePoppedListener)e.Page).PagePopped ();
            }
        }

    }
}
