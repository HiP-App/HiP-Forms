using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;
using Xamarin.Forms.Xaml;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AchievementsDetailsExhibitPage : IViewFor<AchievementsDetailsExhibitPageViewModel>
    {
        public AchievementsDetailsExhibitPage()
        {
            InitializeComponent();
        }
    }
}