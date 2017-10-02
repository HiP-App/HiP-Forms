using PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginScreenView : IViewFor<LoginScreenViewModel>
    {
        private double thisWidth, thisHeight;
        private DeviceOrientation deviceOrientation;

        public LoginScreenView()
        {
            InitializeComponent();
            deviceOrientation = DeviceOrientation.Undefined;
        }

        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            ErrorMessageLabel.Text = "";
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

                Grid.RowDefinitions.Clear();

                Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.1, GridUnitType.Star) });
                Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.1, GridUnitType.Star) });
                Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.1, GridUnitType.Star) });
                Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.1, GridUnitType.Star) });
                Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.1, GridUnitType.Star) });
                Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.1, GridUnitType.Star) });
                Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.1, GridUnitType.Star) });
                Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.1, GridUnitType.Star) });
                Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.1, GridUnitType.Star) });
                Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.1, GridUnitType.Star) });

                Grid.SetRow(EmailEntry, 4);
                Grid.SetRow(PasswordEntry, 5);
                Grid.SetRow(ErrorMessageLabel, 6);
                Grid.SetRow(RegisterLabel, 7);
                Grid.SetRow(ForgotPasswordLabel, 8);
                Grid.SetRow(LoginButton, 9);

                deviceOrientation = DeviceOrientation.Portrait;
            }
            else if (width > height)
            {
                // Orientation changes to Landscape
                if (deviceOrientation == DeviceOrientation.Landscape)
                    return;

                Grid.RowDefinitions.Clear();

                Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.1, GridUnitType.Star) });
                Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.1, GridUnitType.Star) });
                Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.1, GridUnitType.Star) });
                Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.1, GridUnitType.Star) });
                Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.1, GridUnitType.Star) });
                Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.1, GridUnitType.Star) });

                Grid.SetRow(EmailEntry, 0);
                Grid.SetRow(PasswordEntry, 1);
                Grid.SetRow(ErrorMessageLabel, 2);
                Grid.SetRow(RegisterLabel, 3);
                Grid.SetRow(ForgotPasswordLabel, 4);
                Grid.SetRow(LoginButton, 5);

                deviceOrientation = DeviceOrientation.Landscape;
            }
        }
    }
}