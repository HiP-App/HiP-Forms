using System;
using HipMobileUI.Viewmodels;
using Xamarin.Forms;

namespace HipMobileUI.Pages
{
    public partial class MainPage : MasterDetailPage {

        private bool isShown = true;
        private Page page;

        public MainPage()
        {
            InitializeComponent();
            ((MainScreenViewmodel) BindingContext).SetStartItem ();
        }

        private void ListView_OnItemSelected (object sender, SelectedItemChangedEventArgs e)
        {
            this.IsPresented = false;
        }

        private void NavigationPage_OnPushed (object sender, NavigationEventArgs e)
        {
            IsGestureEnabled = false;
            if (isShown)
            {
                page = e.Page;
                page.Disappearing += PageOnDisappearing;
            }
        }

        private void PageOnDisappearing (object sender, EventArgs eventArgs)
        {
            IsGestureEnabled = true;
            page.Disappearing -= PageOnDisappearing;
            page = null;
        }
    }
}
