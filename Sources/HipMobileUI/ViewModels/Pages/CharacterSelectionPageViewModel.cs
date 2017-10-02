using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Contracts;
using System.Windows.Input;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;
using Xamarin.Forms;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages
{
    class CharacterSelectionPageViewModel : NavigationViewModel
    {
        private NavigationViewModel parentViewModel;

        public ICommand AdventurerGridTappedCommand { get; }
        public ICommand ProfessorGridTappedCommand { get; }

        public CharacterSelectionPageViewModel(NavigationViewModel parentViewModel)
        {
            this.parentViewModel = parentViewModel;

            AdventurerGridTappedCommand = new Command(OnAdventurerGridTapped);
            ProfessorGridTappedCommand = new Command(OnProfessorGridTapped);
        }

        private void OnAdventurerGridTapped()
        {
            Settings.AdventurerMode = true;
            SwitchToNextPage();
        }

        private void OnProfessorGridTapped()
        {
            Settings.AdventurerMode = false;
            SwitchToNextPage();
        }

        /// <summary>
        /// Switches to the next page after a character has been selected. If the parent view is the Settings- or ProfileScreenView, the next page is the previous page.
        /// </summary>
        public void SwitchToNextPage()
        {
            IStatusBarController statusBarController = IoCManager.Resolve<IStatusBarController>();
            statusBarController.ShowStatusBar();

            if (parentViewModel.GetType() == typeof(UserOnboardingPageViewModel))
            {
                Navigation.StartNewNavigationStack(new LoadingPageViewModel());
            }
            else if (parentViewModel.GetType() == typeof(ProfileScreenViewModel))
            {
                MainPageViewModel mainPageViewModel = new MainPageViewModel();
                Navigation.StartNewNavigationStack(mainPageViewModel);
                mainPageViewModel.SwitchToProfileView();
            }
            else if (parentViewModel.GetType() == typeof(SettingsScreenViewModel))
            {
                MainPageViewModel mainPageViewModel = new MainPageViewModel();
                Navigation.StartNewNavigationStack(mainPageViewModel);
                mainPageViewModel.SwitchToSettingsScreenView();
            }
        }

        public NavigationViewModel ParentViewModel
        {
            get { return parentViewModel; }
        }
    }
}