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

        private void ListView_OnItemSelected (object sender, SelectedItemChangedEventArgs e)
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
