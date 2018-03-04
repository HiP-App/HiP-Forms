using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.GoogleAnalytics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CharacterSelectionPage : IViewFor<CharacterSelectionPageViewModel>
    {
        private double thisWidth, thisHeight;
        private DeviceOrientation deviceOrientation;

        private CharacterSelectionPageViewModel ViewModel => ((CharacterSelectionPageViewModel) BindingContext);

        public CharacterSelectionPage()
        {
            InitializeComponent();
            GoogleAnalytics.Current.Tracker.SendView("characterSelectionPage");

            deviceOrientation = DeviceOrientation.Undefined;

            // hide the status bar for this page
            IStatusBarController statusBarController = IoCManager.Resolve<IStatusBarController>();
            statusBarController.HideStatusBar();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (!(Math.Abs(width - thisWidth) > 0.4) && !(Math.Abs(height - thisHeight) > 0.4))
                return;

            thisWidth = width;
            thisHeight = height;

            if (width <= height)
            {
                //Portrait
                if (deviceOrientation == DeviceOrientation.Portrait)
                    return;

                MainGrid.RowDefinitions.Clear();
                MainGrid.ColumnDefinitions.Clear();

                MainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                MainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

                MainGrid.Children.Add(AdventurerGrid, 0, 0);
                MainGrid.Children.Add(ProfessorGrid, 0, 1);

                deviceOrientation = DeviceOrientation.Portrait;
            }
            else if (width > height)
            {
                //Landscape
                if (deviceOrientation == DeviceOrientation.Landscape)
                    return;

                MainGrid.RowDefinitions.Clear();
                MainGrid.ColumnDefinitions.Clear();

                MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                MainGrid.Children.Add(AdventurerGrid, 0, 0);
                MainGrid.Children.Add(ProfessorGrid, 1, 0);

                deviceOrientation = DeviceOrientation.Landscape;
            }
        }

        protected override bool OnBackButtonPressed()
        {
            //The user cannot go back when he has to select the character on the first app start
            if (ViewModel.ParentViewModel.GetType() != typeof(UserOnboardingPageViewModel))
            {
                ViewModel.SwitchToNextPage();
            }
            return true;
        }
    }
}