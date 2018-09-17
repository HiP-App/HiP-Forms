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
    public partial class ProfilePictureScreenView : IViewFor<ProfilePictureScreenViewModel>
    {
        private double thisWidth, thisHeight;
        private DeviceOrientation deviceOrientation;

        public ProfilePictureScreenView()
        {
            InitializeComponent();
            deviceOrientation = DeviceOrientation.Undefined;

            MessagingCenter.Subscribe<ProfilePictureScreenViewModel>(this, "PredAvatarList", (sender) => { CreatePredAvatarList(); });

        }


        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (width > height && deviceOrientation != DeviceOrientation.Landscape)
            {
                deviceOrientation = DeviceOrientation.Landscape;

                MainGrid.RowDefinitions.Clear();
                MainGrid.ColumnDefinitions.Clear();
                MainGrid.Children.Clear();
                LeftGrid.RowDefinitions.Clear();
                LeftGrid.ColumnDefinitions.Clear();
                LeftGrid.Children.Clear();
                RightGrid.RowDefinitions.Clear();
                RightGrid.ColumnDefinitions.Clear();
                RightGrid.Children.Clear();

                CreatePredAvatarList();

                MainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                MainGrid.Children.Add(LeftGrid, 0, 0);
                MainGrid.Children.Add(RightGrid, 1, 0);

                LeftGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.65, GridUnitType.Star) });
                LeftGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.15, GridUnitType.Star) });
                LeftGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.2, GridUnitType.Star) });
                LeftGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                LeftGrid.Children.Add(PictureGrid, 0, 0);
                LeftGrid.Children.Add(ErrorMessageLabel, 0, 1);
                LeftGrid.Children.Add(ButtonGrid, 0, 2);

                RightGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.8, GridUnitType.Star) });
                RightGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.2, GridUnitType.Star) });
                RightGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                RightGrid.Children.Add(PictureList, 0, 0);
                RightGrid.Children.Add(PickImageButton, 0, 1);
            }
            else if (width < height && deviceOrientation != DeviceOrientation.Portrait)
            {
                deviceOrientation = DeviceOrientation.Portrait;

                MainGrid.RowDefinitions.Clear();
                MainGrid.ColumnDefinitions.Clear();
                MainGrid.Children.Clear();
                LeftGrid.RowDefinitions.Clear();
                LeftGrid.ColumnDefinitions.Clear();
                LeftGrid.Children.Clear();
                RightGrid.RowDefinitions.Clear();
                RightGrid.ColumnDefinitions.Clear();
                RightGrid.Children.Clear();

                CreatePredAvatarList();

                MainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.35, GridUnitType.Star) });
                MainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.05, GridUnitType.Star) });
                MainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.4, GridUnitType.Star) });
                MainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.1, GridUnitType.Star) });
                MainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.1, GridUnitType.Star) });
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                MainGrid.Children.Add(PictureGrid, 0, 0);
                MainGrid.Children.Add(ErrorMessageLabel, 0, 1);
                MainGrid.Children.Add(PictureList, 0, 2);
                MainGrid.Children.Add(PickImageButton, 0, 3);
                MainGrid.Children.Add(ButtonGrid, 0, 4);
            }

        }


        

        private void CreatePredAvatarList()
        {
            if (BindingContext is ProfilePictureScreenViewModel vm)
            {
                if (!vm.PredAvatarGridBuilt)
                {
                    PictureListGrid.Children.Clear();
                    PictureListGrid.RowDefinitions.Clear();
                    PictureListGrid.ColumnDefinitions.Clear();

                    var avatarCount = vm.PredAvatars.Length;
                    var rowCount = (int)Math.Ceiling((double)avatarCount / 4);
                    var columnCount = 3;
                    var imageRow = 0;
                    var imageColumn = 0;

                    for (var i = 0; i < rowCount; i++)
                    {
                        PictureListGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    }

                    for (var i = 0; i < columnCount; i++)
                    {
                        PictureListGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    }

                    for (var i = 0; i < vm.PredAvatars.Length; i++)
                    {
                        var frame = new Frame
                        {
                            BackgroundColor = vm.HighlightColors[i],

                            Content = new Image
                            {
                                Source = vm.PredAvatars[i].ImageSmall,
                                HeightRequest = 100,
                                WidthRequest = 100,
                                Aspect = Aspect.AspectFit

                            }
                        };

                        var gesture = new TapGestureRecognizer
                        {
                            NumberOfTapsRequired = 1,
                            Command = vm.ImageTappedCommand,
                            CommandParameter = i
                        };
                        gesture.Tapped += (sender, e) =>
                        {
                            vm.PredAvatarGridBuilt = false;
                            CreatePredAvatarList();
                        };
                        if (frame.Content.GestureRecognizers != null)
                        {
                            frame.Content.GestureRecognizers.Add(gesture);
                        }

                        imageColumn = i;
                        imageRow = 0;
                        while (imageColumn >= columnCount)
                        {
                            imageColumn -= columnCount;
                            imageRow++;
                        }

                        PictureListGrid.Children.Add(frame, imageColumn, imageRow);
                        OnPropertyChanged();
                        vm.PredAvatarGridBuilt = true;
                    }
                }  
            }
        }
    }
}