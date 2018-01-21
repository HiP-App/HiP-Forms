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
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Pages
{
    public partial class AppetizerPage : IViewFor<AppetizerPageViewModel>
    {
        private DeviceOrientation orientation;

        public AppetizerPage()
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
                InnerGrid.RowDefinitions.Clear();
                InnerGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.8, GridUnitType.Star) });
                InnerGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.2, GridUnitType.Star) });
                Image.Margin = new Thickness(5, 5, 0, 5);
                AbsoluteLayout.Children.Remove(NextSideButton);
                AbsoluteLayout.Children.Add(NextSideButton, new Rectangle(1, 0.5, 0.15, 0.25), AbsoluteLayoutFlags.All);
            }
            else if (width < height && orientation != DeviceOrientation.Portrait)
            {
                orientation = DeviceOrientation.Portrait;
                OuterGrid.ColumnDefinitions.Clear();
                OuterGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                OuterGrid.RowDefinitions.Clear();
                OuterGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.6, GridUnitType.Star) });
                OuterGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.4, GridUnitType.Star) });
                OuterGrid.Children.Remove(Image);
                OuterGrid.Children.Add(Image, 0, 0);
                OuterGrid.Children.Remove(InnerGrid);
                OuterGrid.Children.Add(InnerGrid, 0, 1);
                InnerGrid.RowDefinitions.Clear();
                InnerGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.7, GridUnitType.Star) });
                InnerGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.3, GridUnitType.Star) });
                Image.Margin = new Thickness(5, 5, 5, 0);
                AbsoluteLayout.Children.Remove(NextSideButton);
                AbsoluteLayout.Children.Add(NextSideButton, new Rectangle(1, 0.5, 0.2, 0.1), AbsoluteLayoutFlags.All);
            }
        }
    }
}
