using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views
{
    public class AchievementsDetailsExhibitViewModel : NavigationViewModel
    {
        public class ExhibitViewModel
        {
            public string Name { get; set; }
            public ImageSource Image { get; set; }
            public bool Unlocked { get; set; }
        }

        public AchievementsDetailsExhibitViewModel(ExhibitsVisitedAchievement exhibitsVisitedAchievement)
        {
            var exhibits = DbManager.DataAccess.Exhibits().GetExhibits().ToList();
            var visited = exhibits.Count(it => it.Unlocked);
            var total = exhibits.Count;
            
            Exhibits = new ObservableCollection<ExhibitViewModel>(exhibits.Select(it => new ExhibitViewModel
            {
                Name = it.Name,
                Image = it.Image.GetImageSource(),
                Unlocked = it.Unlocked
            }));
            Title = exhibitsVisitedAchievement.Title;
            Score = $"{Strings.AchievementsScreenView_Score} {AppSharedData.CurrentAchievementsScore()}";
            VisitedText = string.Format(Strings.AchievementsDetailsExhibitView_VisitedMOfNExhibits, visited, total);
        }
        
        private ObservableCollection<ExhibitViewModel> exhibits;

        public ObservableCollection<ExhibitViewModel> Exhibits
        {
            get => exhibits;
            set => SetProperty(ref exhibits, value);
        }

        private string visitedText;

        public string VisitedText
        {
            get => visitedText;
            set => SetProperty(ref visitedText, value);
        }
        
        private string score;

        public string Score
        {
            get => score;
            set => SetProperty(ref score, value);
        }
    }
}