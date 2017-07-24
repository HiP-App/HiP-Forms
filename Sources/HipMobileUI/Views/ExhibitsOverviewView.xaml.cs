// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//       http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Views
{
    public partial class ExhibitsOverviewView : ContentView, IViewFor<ExhibitsOverviewViewModel> {

        private DeviceOrientation deviceOrientation;
        public ExhibitsOverviewView()
        {
            InitializeComponent();
            deviceOrientation = DeviceOrientation.Undefined;
        }

        protected override void OnSizeAllocated (double width, double height)
        {
            base.OnSizeAllocated (width, height);

            if (width <= height)
            {
                if (deviceOrientation != DeviceOrientation.Portrait)
                {
                    // portrait mode
                    Grid.RowDefinitions.Clear();
                    Grid.ColumnDefinitions.Clear();

                    Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.5, GridUnitType.Star) });
                    if (Label.IsVisible)
                    {
                        // hide empty list and show hint label
                        Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0, GridUnitType.Star) });
                        Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.5, GridUnitType.Star) });
                    }
                    else
                    {
                        // show list and hide hint label
                        Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.5, GridUnitType.Star) });
                        Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0, GridUnitType.Star) });
                    }
                    Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                    Grid.SetRow(Map, 0);
                    Grid.SetRow(List, 1);
                    Grid.SetRow(Label, 2);
                    Grid.SetColumn(Map, 0);
                    Grid.SetColumn(List, 0);
                    Grid.SetColumn(Label, 0);

                    deviceOrientation = DeviceOrientation.Portrait;
                }
            }
            else if (width > height)
            {
                if (deviceOrientation != DeviceOrientation.Landscape)
                {
                    // landscape mode
                    Grid.RowDefinitions.Clear();
                    Grid.ColumnDefinitions.Clear();

                    Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.5, GridUnitType.Star) });
                    if (Label.IsVisible)
                    {
                        // hide empty list and show hint label
                        Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0, GridUnitType.Star) });
                        Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.5, GridUnitType.Star) });
                    }
                    else
                    {
                        // show list and hide hint label
                        Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.5, GridUnitType.Star) });
                        Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0, GridUnitType.Star) });
                    }

                    Grid.SetRow(Map, 0);
                    Grid.SetRow(List, 0);
                    Grid.SetRow(Label, 0);
                    Grid.SetColumn(Map, 0);
                    Grid.SetColumn(List, 1);
                    Grid.SetColumn(Label, 2);

                    deviceOrientation = DeviceOrientation.Landscape;
                }
            }
        }

    }
}
