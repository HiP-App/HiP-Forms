using System;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExhibitRoutePreviewPage : IViewFor<ExhibitRoutePreviewPageViewModel>
    {

        private DeviceOrientation orientation;

        public ExhibitRoutePreviewPage()
        {
            InitializeComponent();
            orientation = DeviceOrientation.Undefined;
        }

        /// <summary>
        /// Size changed, determine if we need to update the layout.
        /// </summary>
        /// <param name="width">The new width.</param>
        /// <param name="height">The new height.</param>
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (width > height && orientation != DeviceOrientation.Landscape)
            {
                orientation = DeviceOrientation.Landscape;
                OuterGrid.RowDefinitions.Clear();
                OuterGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                OuterGrid.ColumnDefinitions.Clear();
                OuterGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.5, GridUnitType.Star) });
                OuterGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.5, GridUnitType.Star) });
                OuterGrid.Children.Remove(Image);
                OuterGrid.Children.Add(Image, 0, 0);
                OuterGrid.Children.Remove(InnerGrid);
                OuterGrid.Children.Add(InnerGrid, 1, 0);
                Image.Margin = new Thickness(5, 5, 0, 5);
                InnerGrid.RowDefinitions.Clear();
                InnerGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.8, GridUnitType.Star) });
                InnerGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.2, GridUnitType.Star) });
            }
            else if (width < height && orientation != DeviceOrientation.Portrait)
            {
                orientation = DeviceOrientation.Portrait;
                OuterGrid.ColumnDefinitions.Clear();
                OuterGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                OuterGrid.RowDefinitions.Clear();
                OuterGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.53, GridUnitType.Star) });
                OuterGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.47, GridUnitType.Star) });
                OuterGrid.Children.Remove(InnerGrid);
                OuterGrid.Children.Add(InnerGrid, 0, 0);
                OuterGrid.Children.Remove(InnerGrid);
                OuterGrid.Children.Add(InnerGrid, 0, 1);
                Image.Margin = new Thickness(5, 5, 5, 0);
                InnerGrid.RowDefinitions.Clear();
                InnerGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.8, GridUnitType.Star) });
                InnerGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.2, GridUnitType.Star) });
            }
        }
    }
}