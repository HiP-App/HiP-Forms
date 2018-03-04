using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;
using Plugin.GoogleAnalytics;
using Xamarin.Forms.Xaml;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AchievementsDetailsRoutePage : IViewFor<AchievementsDetailsRouteViewModel>
    {
        public AchievementsDetailsRoutePage()
        {
            InitializeComponent();
            GoogleAnalytics.Current.Tracker.SendView("AchievementsDetailsRoutePage");
        }
    }
}