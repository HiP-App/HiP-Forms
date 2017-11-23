using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using Xamarin.Forms;
using System.ComponentModel;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views
{
	public class AchievementsDetailsExhibitViewModel : NavigationViewModel
	{
		public AchievementsDetailsExhibitViewModel()
		{
			//TODO add details Change this to exhibit overview and 
			//then depending on the achievement type, have different details. 
			Exhibits = new ObservableCollection<string>
			{
			"Exhibit 1",
			"Exhibit 2",
			"Exhibit 3",
			"Exhibit 4",
			"Exhibit 5"
			};
			Score = $"{Strings.AchievementsScreenView_Score} {Settings.Score}";
		}

		private ObservableCollection<string> exhibits;
		public ObservableCollection<string> Exhibits
		{
			get => exhibits;
			set => SetProperty(ref exhibits, value);

			}
		private string score;

		public string Score
		{
			get => score;
			set => SetProperty(ref score, value);
			}


		}
	}