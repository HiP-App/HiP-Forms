using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LeaderboardView : IViewFor<LeaderboardViewModel>
    {
        public LeaderboardView()
        {
            InitializeComponent();
            ScrollToOwnRanking ();
        }

        private async void ScrollToOwnRanking ()
        {
            await Task.Delay (100);
            LeaderboardListView.ScrollTo (LeaderboardListView.SelectedItem, ScrollToPosition.MakeVisible, false);
        }
    }
}