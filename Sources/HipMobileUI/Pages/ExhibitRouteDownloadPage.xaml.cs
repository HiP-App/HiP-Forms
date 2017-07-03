using System;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Pages {

    public partial class ExhibitRouteDownloadPage : ContentPage, IViewFor<ExhibitRouteDownloadPageViewModel> {

        private double thisWidth, thisHeight;
        private DeviceOrientation deviceOrientation;
        public ExhibitRouteDownloadPage ()
        {
            InitializeComponent ();
            deviceOrientation = DeviceOrientation.Undefined;
        }

        protected override void OnSizeAllocated (double width, double height)
        {
            base.OnSizeAllocated (width, height);

            if (!(Math.Abs (width - thisWidth) > 0.4) && !(Math.Abs (height - thisHeight) > 0.4))
                return;

            thisWidth = width;
            thisHeight = height;

            if (width <= height)
            {
                // Orientation changes to Portrait
                if (deviceOrientation == DeviceOrientation.Portrait)
                    return;
                Grid.RowDefinitions.Clear ();
                Grid.ColumnDefinitions.Clear ();

                Grid.RowDefinitions.Add (new RowDefinition {Height = new GridLength (0.3, GridUnitType.Star)});
                Grid.RowDefinitions.Add (new RowDefinition {Height = new GridLength (0.1, GridUnitType.Star)});
                Grid.RowDefinitions.Add (new RowDefinition {Height = new GridLength (0.4, GridUnitType.Star)});
                Grid.RowDefinitions.Add (new RowDefinition {Height = new GridLength (0.2, GridUnitType.Star)});

                Grid.ColumnDefinitions.Add (new ColumnDefinition {Width = new GridLength (1, GridUnitType.Star)});

                Grid.SetRow (ImageView, 0);
                Grid.SetRow (DescriptionView, 1);
                Grid.SetRow (ProgressView, 2);
                Grid.SetRow (ButtonView, 3);
                Grid.SetColumnSpan(ButtonView, 1);

                Grid.SetColumn (ImageView, 0);
                Grid.SetColumn (DescriptionView, 0);
                Grid.SetColumn (ProgressView, 0);
                Grid.SetColumn (ButtonView, 0);
                Grid.SetRowSpan (ImageView, 1);

                deviceOrientation = DeviceOrientation.Portrait;
            }
            else if (width > height)
            {
                // Orientation changes to Landscape
                if (deviceOrientation == DeviceOrientation.Landscape)
                    return;
                Grid.RowDefinitions.Clear();
                Grid.ColumnDefinitions.Clear();

                Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.5, GridUnitType.Star) });
                Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.5, GridUnitType.Star) });

                Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.2, GridUnitType.Star) });
                Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.6, GridUnitType.Star) });
                Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.2, GridUnitType.Star) });

                Grid.SetColumn(ImageView, 0);
                Grid.SetColumn(DescriptionView, 1);
                Grid.SetColumn(ProgressView, 1);
                Grid.SetColumn(ButtonView, 0);
                Grid.SetColumnSpan(ButtonView, 2);

                Grid.SetRow(ImageView, 0);
                Grid.SetRowSpan(ImageView, 2);
                Grid.SetRow(DescriptionView, 0);
                Grid.SetRow(ProgressView, 1);
                Grid.SetRow(ButtonView, 2);

                deviceOrientation = DeviceOrientation.Landscape;
            }
        }

    }
}