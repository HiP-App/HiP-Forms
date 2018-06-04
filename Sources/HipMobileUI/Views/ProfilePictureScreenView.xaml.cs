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
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.AuthenticationApiAccess;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilePictureScreenView : IViewFor<ProfilePictureScreenViewModel>
    {
        private double thisWidth, thisHeight;
        private DeviceOrientation deviceOrientation;

        public ProfilePictureScreenView()
        {
            InitializeComponent();
            deviceOrientation = DeviceOrientation.Undefined;
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (width > height && deviceOrientation != DeviceOrientation.Landscape)
            {
                deviceOrientation = DeviceOrientation.Landscape;
                OuterGrid.RowDefinitions.Clear();
                OuterGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                OuterGrid.ColumnDefinitions.Clear();
                OuterGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                OuterGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                OuterGrid.Children.Remove(InnerGrid);
                OuterGrid.Children.Add(InnerGrid, 0, 0);
            }
            else if (width < height && deviceOrientation != DeviceOrientation.Portrait)
            {
                deviceOrientation = DeviceOrientation.Portrait;
                OuterGrid.ColumnDefinitions.Clear();
                OuterGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                OuterGrid.RowDefinitions.Clear();
                OuterGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                OuterGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                OuterGrid.Children.Remove(InnerGrid);
                OuterGrid.Children.Add(InnerGrid, 0, 0);
            }

        }
    }
}