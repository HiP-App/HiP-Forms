using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views {
    public class ProfileScreenViewModel : NavigationViewModel {

        private readonly MainPageViewModel mainPageViewModel;

        public ProfileScreenViewModel (MainPageViewModel mainPageVm)
        {
            mainPageViewModel = mainPageVm;

            Tabs = new ObservableCollection<string> {Strings.MainPageViewModel_OverviewPage, "Erfolge", "Statistik"};

            Achievements = new ObservableCollection<string> {"Achievement 1", "Achievement 2", "Achievement 3", "Achievement 4", "Achievement 5", "Achievement 6", "Achievement 7"};

            Logout = new Command (LogoutDummy);
        }

        public ICommand Logout { get; }

        public ImageSource Avatar => ImageSource.FromFile ("ic_account_circle.png");
        public String Username => Settings.Username;
        public String EMail => Settings.EMail;
        public int Score => Settings.Score;
        public String AchievementCount => Settings.Achievements+" / 30";
        public String Completeness => Settings.Completeness + "%";

        async void LogoutDummy ()
        {
            var result = await Navigation.DisplayAlert ("Abmeldung", "Wollen Sie sich wirklich abmelden?", "Ja", "Nein");
            if (!result)
                return;
            Settings.IsLoggedIn = false;
            mainPageViewModel.UpdateAccountViews ();
        }

        private ObservableCollection<String> achievements;
        public ObservableCollection<String> Achievements
        {
            get { return achievements; }
            set { SetProperty(ref achievements, value); }
        }

        private ObservableCollection<string> tabs;

        public ObservableCollection<string> Tabs {
            get { return tabs; }
            set { SetProperty (ref tabs, value); }
        }

    }
}