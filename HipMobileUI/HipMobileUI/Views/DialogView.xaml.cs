using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HipMobileUI.Pages;
using Xamarin.Forms;

namespace HipMobileUI.Views
{
    public partial class DialogView : ContentView
    {
        public DialogView()
        {
            InitializeComponent();
        }

        async void OnAlertDialogDisplay(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CaptionDialogPage(1));

        }

        async void OnCaptionDialogDisplay(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CaptionDialogPage(2));
        }
    }
}
