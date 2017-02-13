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
using System;
using System.Collections;
using System.Linq;
using Xamarin.Forms;

namespace HipMobileUI.Controls
{
    public class CarouselIndicators : Grid
    {
        private ImageSource unselectedImageSource = null;
        private ImageSource selectedImageSource = null;
        private readonly StackLayout indicators = new StackLayout() { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.CenterAndExpand };

        public CarouselIndicators()
        {
            this.HorizontalOptions = LayoutOptions.CenterAndExpand;
            this.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            this.Children.Add(indicators);
        }

        public static readonly BindableProperty PositionProperty = BindableProperty.Create(nameof(Position), typeof(int), typeof(CarouselIndicators), 0, BindingMode.TwoWay, propertyChanging: PositionChanging);
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(CarouselIndicators), Enumerable.Empty<object>(), BindingMode.OneWay, propertyChanged: ItemsChanged);
        public static readonly BindableProperty SelectedIndicatorProperty = BindableProperty.Create(nameof(SelectedIndicator), typeof(string), typeof(CarouselIndicators), "", BindingMode.OneWay);
        public static readonly BindableProperty UnselectedIndicatorProperty = BindableProperty.Create(nameof(UnselectedIndicator), typeof(string), typeof(CarouselIndicators), "", BindingMode.OneWay);
        public static readonly BindableProperty IndicatorWidthProperty = BindableProperty.Create(nameof(IndicatorWidth), typeof(double), typeof(CarouselIndicators), 0.0, BindingMode.OneWay);
        public static readonly BindableProperty IndicatorHeightProperty = BindableProperty.Create(nameof(IndicatorHeight), typeof(double), typeof(CarouselIndicators), 0.0, BindingMode.OneWay);

        public string SelectedIndicator
        {
            get { return (string)this.GetValue(SelectedIndicatorProperty); }
            set { this.SetValue(SelectedIndicatorProperty, value); }
        }

        public string UnselectedIndicator
        {
            get { return (string)this.GetValue(UnselectedIndicatorProperty); }
            set { this.SetValue(UnselectedIndicatorProperty, value); }
        }

        public double IndicatorWidth
        {
            get { return (double)this.GetValue(IndicatorWidthProperty); }
            set { this.SetValue(IndicatorWidthProperty, value); }
        }

        public double IndicatorHeight
        {
            get { return (double)this.GetValue(IndicatorHeightProperty); }
            set { this.SetValue(IndicatorHeightProperty, value); }
        }

        public int Position
        {
            get { return (int)this.GetValue(PositionProperty); }
            set { this.SetValue(PositionProperty, value); }
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)this.GetValue(ItemsSourceProperty); }
            set { this.SetValue(ItemsSourceProperty, (object)value); }
        }

        private void Clear()
        {
            indicators.Children.Clear();
        }

        private void Init(int position)
        {

            if (unselectedImageSource == null)
                unselectedImageSource = ImageSource.FromFile(UnselectedIndicator);

            if (selectedImageSource == null)
                selectedImageSource = ImageSource.FromFile(SelectedIndicator);

            if (indicators.Children.Count > 0)
            {
                for (int i = 0; i < indicators.Children.Count; i++)
                {
                    if (((Image)indicators.Children[i]).ClassId == nameof(State.Selected) && i != position)
                        indicators.Children[i] = BuildImage(State.Unselected, i);
                    else if (((Image)indicators.Children[i]).ClassId == nameof(State.Unselected) && i == position)
                        indicators.Children[i] = BuildImage(State.Selected, i);
                }
            }
            else
            {
                var enumerator = ItemsSource.GetEnumerator();
                int count = 0;
                while (enumerator.MoveNext())
                {
                    Image image = null;
                    if (position == count)
                        image = BuildImage(State.Selected, count);
                    else
                        image = BuildImage(State.Unselected, count);

                    indicators.Children.Add(image);

                    count++;
                }
            }
        }

        private Image BuildImage(State state, int position)
        {
            var image = new Image()
            {
                WidthRequest = IndicatorWidth,
                HeightRequest = IndicatorHeight,
                ClassId = state.ToString()
            };

            switch (state)
            {
                case State.Selected:
                    image.Source = selectedImageSource;
                    break;
                case State.Unselected:
                    image.Source = unselectedImageSource;
                    break;
                default:
                    throw new Exception("Invalid state selected");
            }

            image.GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command(() => { Position = position; }) });

            return image;
        }

        private static void PositionChanging(object bindable, object oldValue, object newValue)
        {
            var carouselIndicators = bindable as CarouselIndicators;

            carouselIndicators.Init(Convert.ToInt32(newValue));
        }

        private static void ItemsChanged(object bindable, object oldValue, object newValue)
        {
            var carouselIndicators = bindable as CarouselIndicators;

            carouselIndicators.Clear();
            carouselIndicators.Init(0);
        }

        public enum State
        {
            Selected,
            Unselected
        }
    }
}
