using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views
{
    public class AchievementsScreenViewModel : NavigationViewModel
    {
        public AchievementsScreenViewModel()
        {
            // TODO need to make changes so that achievements screen asks user to log in when no user logged in
            Achievements = new ObservableCollection<AchievementViewModel>();
            Device.BeginInvokeOnMainThread(async () => await InitAchievements());
        }

        private async Task InitAchievements()
        {
            Achievements.Clear();

            await IoCManager.Resolve<IAchievementFetcher>().UpdateAchievements(); // TODO Use return value
            var achievements = AchievementManager.GetAchievements();
            foreach (var achievement in achievements)
            {
                Achievements.Add(await AchievementViewModel.CreateFrom(achievement));
            }
        }

        private ObservableCollection<AchievementViewModel> achievements;

        public ObservableCollection<AchievementViewModel> Achievements
        {
            get => achievements;
            set => SetProperty(ref achievements, value);
        }
    }
}