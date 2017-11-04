using System.Collections.ObjectModel;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views
{
    public class AchievementsScreenViewModel : NavigationViewModel
    {
        public AchievementsScreenViewModel()
        {
            //need to make changes so that achievements screen asks user to log in when no user logged in
            Achievements = new ObservableCollection<string>
            {
                "Achievement 1",
                "Achievement 2",
                "Achievement 3",
                "Achievement 4",
                "Achievement 5",
                "Achievement 6",
                "Achievement 7"
            };
        }

        public string Username => Settings.Username;
        public int Score => Settings.Score;
        public string Completeness => Settings.Completeness + "%";

        private ObservableCollection<string> achievements;

        public ObservableCollection<string> Achievements
        {
            get => achievements;
            set => SetProperty(ref achievements, value);
        }

        private ObservableCollection<string> tabs;

        public ObservableCollection<string> Tabs
        {
            get => tabs;
            set => SetProperty(ref tabs, value);
        }
    }
}