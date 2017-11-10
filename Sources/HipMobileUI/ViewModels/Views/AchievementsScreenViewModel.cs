using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
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
            var score = 0;

            await IoCManager.Resolve<IAchievementFetcher>().UpdateAchievements(); // TODO Use return value
            foreach (var achievement in AchievementManager.GetAchievements())
            {
                if (achievement.IsUnlocked)
                {
                    score += achievement.Points;
                }
                Achievements.Add(AchievementViewModel.CreateFrom(achievement));
            }
            Score = $"{Strings.AchievementsScreenView_Score} {score}";
        }

        private ObservableCollection<AchievementViewModel> achievements;

        public ObservableCollection<AchievementViewModel> Achievements
        {
            get => achievements;
            set => SetProperty(ref achievements, value);
        }

        private string score;

        public string Score
        {
            get => score;
            set => SetProperty(ref score, value);
        }
    }
}