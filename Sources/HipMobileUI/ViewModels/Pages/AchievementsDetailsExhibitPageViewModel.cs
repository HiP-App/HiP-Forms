using System.Collections.ObjectModel;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using Xamarin.Forms;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using System.Linq;
using System.Collections.ObjectModel;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages
{
    public class AchievementsDetailsExhibitPageViewModel : NavigationViewModel
    {
        public class ExhibitViewModel
        {
            public string Name { get; set; }
            public ImageSource Image { get; set; }
            public bool Unlocked { get; set; }
        }

        public AchievementsDetailsExhibitPageViewModel(ExhibitsVisitedAchievement exhibitsVisitedAchievement)
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