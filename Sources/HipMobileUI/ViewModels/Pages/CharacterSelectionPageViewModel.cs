using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages
{
    class CharacterSelectionPageViewModel : NavigationViewModel
    {
        public ICommand AdventurerGridTappedCommand { get; }
        public ICommand ProfessorGridTappedCommand { get; }

        public CharacterSelectionPageViewModel()
        {
            AdventurerGridTappedCommand = new Command(OnAdventurerGridTapped);
            ProfessorGridTappedCommand = new Command(OnProfessorGridTapped);
        }

        private void OnAdventurerGridTapped()
        {
            Settings.AdventurerMode = true;
            SwitchToLoadingPage();
        }

        private void OnProfessorGridTapped()
        {
            Settings.AdventurerMode = false;
            SwitchToLoadingPage();
        }

        private void SwitchToLoadingPage()
        {
            IStatusBarController statusBarController = IoCManager.Resolve<IStatusBarController>();
            statusBarController.ShowStatusBar();

            Navigation.StartNewNavigationStack(new LoadingPageViewModel());
        }
    }
}
