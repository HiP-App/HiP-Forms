using System;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfileScreenView : IViewFor<ProfileScreenViewModel>
    {
        private double thisWidth, thisHeight;
        private DeviceOrientation deviceOrientation;
        public ProfileScreenView()
        {
            InitializeComponent();
            deviceOrientation = DeviceOrientation.Undefined;
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
                // Orientation changes to Portrait
                if (deviceOrientation == DeviceOrientation.Portrait)
                    return;

                // Overview
                OverviewGrid.RowDefinitions.Clear();
                OverviewGrid.ColumnDefinitions.Clear();

                OverviewGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.4, GridUnitType.Star) });
                OverviewGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.4, GridUnitType.Star) });
                OverviewGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.2, GridUnitType.Star) });

                OverviewGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                Grid.SetRow(NameAndIcon, 0);
                Grid.SetRow(Scores, 1);
                Grid.SetRow(Logout, 2);
                Grid.SetRowSpan(Scores, 1);

                Grid.SetColumn(NameAndIcon, 0);
                Grid.SetColumn(Scores, 0);
                Grid.SetColumn(Logout, 0);

                // Achievements
                AchievementsGrid.RowDefinitions.Clear();
                AchievementsGrid.ColumnDefinitions.Clear();

                AchievementsGrid.RowDefinitions.Add (new RowDefinition { Height = new GridLength(0.5, GridUnitType.Star) });
                AchievementsGrid.RowDefinitions.Add (new RowDefinition { Height = new GridLength(0.5, GridUnitType.Star) });

                AchievementsGrid.ColumnDefinitions.Add (new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                Grid.SetRow (AchievementsTitle, 0);
                Grid.SetRow (ScrollView, 1);

                Grid.SetColumn (AchievementsTitle, 0);
                Grid.SetColumn (ScrollView, 0);


                deviceOrientation = DeviceOrientation.Portrait;
            }
            else if (width > height)
            {
                // Orientation changes to Landscape
                if (deviceOrientation == DeviceOrientation.Landscape)
                    return;

                // Overview
                OverviewGrid.RowDefinitions.Clear();
                OverviewGrid.ColumnDefinitions.Clear();

                OverviewGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.5, GridUnitType.Star) });
                OverviewGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.5, GridUnitType.Star) });

                OverviewGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.8, GridUnitType.Star) });
                OverviewGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.2, GridUnitType.Star) });

                Grid.SetColumn(NameAndIcon, 0);
                Grid.SetColumn(Scores, 1);
                Grid.SetColumn(Logout, 0);

                Grid.SetRow(NameAndIcon, 0);
                Grid.SetRow(Scores, 0);
                Grid.SetRow(Logout, 1);
                Grid.SetRowSpan(Scores, 2);

                // Achievements
                AchievementsGrid.RowDefinitions.Clear();
                AchievementsGrid.ColumnDefinitions.Clear();

                AchievementsGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

                AchievementsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.5, GridUnitType.Star) });
                AchievementsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.5, GridUnitType.Star) });

                Grid.SetRow(AchievementsTitle, 0);
                Grid.SetRow(ScrollView, 0);

                Grid.SetColumn(AchievementsTitle, 0);
                Grid.SetColumn(ScrollView, 1);


                deviceOrientation = DeviceOrientation.Landscape;
            }
        }
    }
}