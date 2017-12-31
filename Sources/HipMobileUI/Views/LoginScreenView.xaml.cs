// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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
                Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.3, GridUnitType.Star) });
                Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.1, GridUnitType.Star) });
                Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.3, GridUnitType.Star) });
                Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.1, GridUnitType.Star) });
                Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.1, GridUnitType.Star) });
                Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.1, GridUnitType.Star) });

                Grid.SetRow(EmailEntry, 0);
                Grid.SetRow(PasswordEntry, 1);
                Grid.SetRow(ErrorMessageLabel, 2);
                Grid.SetRow(RegisterLabel, 3);
                Grid.SetRow(ForgotPasswordLabel, 4);
                Grid.SetRow(LoginButton, 5);

                deviceOrientation = DeviceOrientation.Portrait;
            }
            else if (width > height)
            {
                // Orientation changes to Landscape
                if (deviceOrientation == DeviceOrientation.Landscape)
                    return;

                Grid.RowDefinitions.Clear();
                Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.2, GridUnitType.Star) });
                Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.2, GridUnitType.Star) });
                Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.1, GridUnitType.Star) });
                Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.15, GridUnitType.Star) });
                Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.15, GridUnitType.Star) });
                Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.2, GridUnitType.Star) });

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