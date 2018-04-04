using PaderbornUniversity.SILab.Hip.Mobile.UI.DesignTime;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using System;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Pages
{
    public partial class ExhibitRouteDownloadPage : IViewFor<ExhibitRouteDownloadPageViewModel>
    {
        private DeviceOrientation orientation;

        public ExhibitRouteDownloadPage()
        {
            InitializeComponent();
            DesignMode.Initialize(this);
            orientation = DeviceOrientation.Undefined;
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (width > height && orientation != DeviceOrientation.Landscape)
            {
                OuterGrid.RowDefinitions.Clear();
                OuterGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                OuterGrid.ColumnDefinitions.Clear();
                OuterGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.5, GridUnitType.Star) });
                OuterGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.5, GridUnitType.Star) });
                OuterGrid.Children.Remove(Image);
                OuterGrid.Children.Add(Image, 0, 0);
                OuterGrid.Children.Remove(InnerGrid);
                OuterGrid.Children.Add(InnerGrid, 1, 0);
                InnerGrid.RowDefinitions.Clear();
                InnerGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.8, GridUnitType.Star) });
                InnerGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.2, GridUnitType.Star) });
                Image.Margin = new Thickness(5, 5, 0, 5);
                orientation = DeviceOrientation.Landscape;
            }
            else if (width < height && orientation != DeviceOrientation.Portrait)
            {
                OuterGrid.ColumnDefinitions.Clear();
                OuterGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                OuterGrid.RowDefinitions.Clear();
                //The image got a ratio of 9:7
                var imageGridHeight = width / height / 9 * 7;
                OuterGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(imageGridHeight, GridUnitType.Star) });
                OuterGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1 - imageGridHeight, GridUnitType.Star) });
                OuterGrid.Children.Remove(Image);
                OuterGrid.Children.Add(Image, 0, 0);
                OuterGrid.Children.Remove(InnerGrid);
                OuterGrid.Children.Add(InnerGrid, 0, 1);
                InnerGrid.RowDefinitions.Clear();
                InnerGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.7, GridUnitType.Star) });
                InnerGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.3, GridUnitType.Star) });
                Image.Margin = new Thickness(5, 5, 5, 0);
                orientation = DeviceOrientation.Portrait;
            }
        }
    }
}