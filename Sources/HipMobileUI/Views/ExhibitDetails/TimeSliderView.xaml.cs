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
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views.ExhibitDetails;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Views.ExhibitDetails
{
    public partial class TimeSliderView : IViewFor<TimeSliderViewModel>
    {
        private DeviceOrientation orientation;

        public TimeSliderView()
        {
            orientation = DeviceOrientation.Undefined;
            InitializeComponent();
        }

        /// <summary>
        /// Size changed, determine if we need to update the layout.
        /// </summary>
        /// <param name="width">The new width.</param>
        /// <param name="height">The new height.</param>
        protected override void OnSizeAllocated(double width, double height)
        {
            if (width > height && orientation != DeviceOrientation.Landscape)
            {
                orientation = DeviceOrientation.Landscape;
                BottomSheetView.BottomSheetVisible = false;
                ContentGrid.ColumnDefinitions.Clear();
                ContentGrid.ColumnDefinitions.Add(new ColumnDefinition());
                ContentGrid.RowDefinitions.Clear();
                ContentGrid.RowDefinitions.Add(new RowDefinition());
                ContentGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30, GridUnitType.Absolute) });
                ContentGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(80, GridUnitType.Absolute) });
            }
            else if (width < height && orientation != DeviceOrientation.Portrait)
            {
                orientation = DeviceOrientation.Portrait;
                BottomSheetView.BottomSheetVisible = true;
                ContentGrid.ColumnDefinitions.Clear();
                ContentGrid.ColumnDefinitions.Add(new ColumnDefinition());
                ContentGrid.RowDefinitions.Clear();
                ContentGrid.RowDefinitions.Add(new RowDefinition());
                ContentGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30, GridUnitType.Absolute) });
                ContentGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(80, GridUnitType.Absolute) });
                ContentGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.175, GridUnitType.Star) });
            }

            base.OnSizeAllocated(width, height);
        }
    }
}