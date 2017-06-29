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

using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Pages
{
    public partial class MainPage : IViewFor<MainPageViewModel>
    {

        private MainPageViewModel ViewModel => ((MainPageViewModel)BindingContext);

        /// <summary>
        /// Accessor to get the navigation page from other classes.
        /// </summary>
        public Xamarin.Forms.NavigationPage Navigationpage => NavigationPage;

        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            ViewModel.SelectedViewModel = ViewModel.MainScreenViewModels[0];
        }

        /// <summary>
        /// Hide the menu on phones once menu item was tapped.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_OnItemTapped(object sender, ItemTappedEventArgs e)
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
        private void NavigationPage_OnPushed(object sender, NavigationEventArgs e)
        {
            // Disable the swipe gesture when a page is pushed
            IsGestureEnabled = false;
            ((NavigationViewModel)e.Page.BindingContext).OnAppearing();
        }

        /// <summary>
        /// page is popped from the navigation stack.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event parameters.</param>
        private void NavigationPage_OnPopped(object sender, NavigationEventArgs e)
        {
            // enable the swipe gesture once this page becomes visible again
            if (NavigationPage.CurrentPage == ContentPage)
            {
                IsGestureEnabled = true;
            }

            // inform the viewmodel...
            // ... of popped page that it was popped
            ((NavigationViewModel)e.Page.BindingContext).OnDisappearing();
            // ... of underlaying page that it is visible again
            ((NavigationViewModel)NavigationPage.CurrentPage.BindingContext).OnRevealed();
        }

        protected override bool OnBackButtonPressed()
        {
			if(ViewModel.SelectedViewModel.GetType() == typeof(ForgotPasswordScreenViewModel))
            {
                ViewModel.SwitchToLoginView();
                return true;
            }
			else if(ViewModel.SelectedViewModel.GetType() == typeof(RegisterScreenViewModel))
			{
                ViewModel.SwitchToLoginView();
                return true;
            }	
			else
            {
                return base.OnBackButtonPressed();
            }
        }

    }
}
