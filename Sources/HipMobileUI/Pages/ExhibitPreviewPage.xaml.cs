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
    public partial class ExhibitPreviewView : IViewFor<ExhibitPreviewPageViewModel>
    {
        private double thisWidth, thisHeight;
        private DeviceOrientation deviceOrientation;

        public ExhibitPreviewView()
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
                PageGrid.RowDefinitions.Clear();
                PageGrid.ColumnDefinitions.Clear();

                PageGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.5, GridUnitType.Star) });
                PageGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.35, GridUnitType.Star) });
                PageGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.15, GridUnitType.Star) });

                PageGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(width / 2) });
                PageGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(width / 2) });

                Grid.SetRow(ImageView, 0);
                Grid.SetRow(QuestionView, 1);
                Grid.SetRow(ButtonView, 2);

                Grid.SetColumn(ImageView, 0);
                Grid.SetColumnSpan(ImageView, 2);
                Grid.SetColumn(QuestionView, 0);
                Grid.SetColumnSpan(QuestionView, 2);
                Grid.SetColumn(ButtonView, 0);
                Grid.SetColumnSpan(ButtonView, 2);

                QuestionView.Padding = new Thickness(10, 0, 10, 0);
                ImageView.Aspect = Aspect.AspectFill;

                deviceOrientation = DeviceOrientation.Portrait;
            }
            else if (width > height)
            {
                // Orientation changes to Landscape
                if (deviceOrientation == DeviceOrientation.Landscape)
                    return;
                PageGrid.RowDefinitions.Clear();
                PageGrid.ColumnDefinitions.Clear();

                PageGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(width / 2) });
                PageGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(width / 2) });

                PageGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.6, GridUnitType.Star) });
                PageGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.2, GridUnitType.Star) });
                PageGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.2, GridUnitType.Star) });

                Grid.SetColumn(ImageView, 0);
                Grid.SetColumnSpan(ImageView, 2);
                Grid.SetColumn(QuestionView, 0);
                Grid.SetColumnSpan(QuestionView, 2);
                Grid.SetColumn(ButtonView, 0);
                Grid.SetColumnSpan(ButtonView, 2);

                Grid.SetRow(ImageView, 0);
                Grid.SetRow(QuestionView, 1);
                Grid.SetRow(ButtonView, 2);

                QuestionView.Padding = new Thickness(10, 0);
                ImageView.Aspect = Aspect.AspectFit;

                deviceOrientation = DeviceOrientation.Landscape;
            }
        }
    }
}