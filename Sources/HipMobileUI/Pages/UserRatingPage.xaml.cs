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

using PaderbornUniversity.SILab.Hip.Mobile.UI.DesignTime;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Pages
{
    public partial class UserRatingPage : IViewFor<UserRatingPageViewModel>
    {
        private UserRatingPageViewModel ViewModel => ((UserRatingPageViewModel)BindingContext);
        private DeviceOrientation orientation;

        public UserRatingPage()
        {
            InitializeComponent();
            DesignMode.Initialize(this);
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
                OuterAbsoluteLayout.Children.Remove(Image);
                OuterAbsoluteLayout.Children.Add(Image, new Rectangle(0, 0, 0.5, 1), AbsoluteLayoutFlags.All);
                OuterGrid.RowDefinitions.Clear();
                OuterGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                OuterGrid.ColumnDefinitions.Clear();
                OuterGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.5, GridUnitType.Star) });
                OuterGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.5, GridUnitType.Star) });
                OuterGrid.Children.Remove(InnerGrid1);
                OuterGrid.Children.Add(InnerGrid1, 0, 0);
                OuterGrid.Children.Remove(InnerGrid2);
                OuterGrid.Children.Add(InnerGrid2, 1, 0);
                InnerGrid2.RowDefinitions.Clear();
                InnerGrid2.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.8, GridUnitType.Star) });
                InnerGrid2.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.2, GridUnitType.Star) });
            }
            else if (width < height && orientation != DeviceOrientation.Portrait)
            {
                orientation = DeviceOrientation.Portrait;
                OuterAbsoluteLayout.Children.Remove(Image);
                OuterAbsoluteLayout.Children.Add(Image, new Rectangle(0, 0, 1, 0.6), AbsoluteLayoutFlags.All);
                OuterGrid.ColumnDefinitions.Clear();
                OuterGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                OuterGrid.RowDefinitions.Clear();
                OuterGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.6, GridUnitType.Star) });
                OuterGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.4, GridUnitType.Star) });
                OuterGrid.Children.Remove(InnerGrid1);
                OuterGrid.Children.Add(InnerGrid1, 0, 0);
                OuterGrid.Children.Remove(InnerGrid2);
                OuterGrid.Children.Add(InnerGrid2, 0, 1);
                InnerGrid2.RowDefinitions.Clear();
                InnerGrid2.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.7, GridUnitType.Star) });
                InnerGrid2.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.3, GridUnitType.Star) });
            }
        }
        protected override bool OnBackButtonPressed()
        {
            ViewModel.ReturnToAppetizerPage();
            return true;
        }

    }
}