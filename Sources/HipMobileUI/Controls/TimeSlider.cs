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
using System.Collections.ObjectModel;
using System.Diagnostics;
using FFImageLoading.Forms;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Controls
{
    /// <summary>
    /// A slider where the slider can display images and/or texts. The slider returns the currently selected value indicating which item is currently under the middle separator.
    /// The slider doesn't use a linear scale as each subelement has a so called safezone. Safezones have a constant value and only in between them the value changes with a linear scale.
    /// The width of the zones can be modified with the respective properties. Just make sure the fractions add up to 1.
    /// When using this class, make sure to give it a height request, otherwise layouting might not work.
    /// </summary>
    class TimeSlider : ContentView
    {

        public static readonly BindableProperty ImagesProperty =
            BindableProperty.Create("Images", typeof(ObservableCollection<ImageSource>), typeof(TimeSlider), new ObservableCollection<ImageSource>(),
                                     propertyChanged: ImagePropertyChanged);

        public static readonly BindableProperty SelectedValueProperty =
            BindableProperty.Create("SelectedValue", typeof(double), typeof(TimeSlider), 0.0);

        public static readonly BindableProperty TextsProperty =
            BindableProperty.Create("Texts", typeof(ObservableCollection<string>), typeof(TimeSlider), new ObservableCollection<string>(), propertyChanged: TextsPropertyChanged);

        public static readonly BindableProperty ItemWidthProperty =
            BindableProperty.Create("ItemWidth", typeof(int), typeof(TimeSlider), 100, propertyChanged: ItemWidthPropertyChanged);

        public static readonly BindableProperty SafezoneFractionProperty =
            BindableProperty.Create("SafezoneFraction", typeof(double), typeof(TimeSlider), 0.5, propertyChanged: SafezonePropertyChanged);

        public static readonly BindableProperty FadezoneFractionProperty =
            BindableProperty.Create("FadezoneFraction", typeof(double), typeof(TimeSlider), 0.5, propertyChanged: SafezonePropertyChanged);

        public static readonly BindableProperty SelectorColorProperty =
            BindableProperty.Create("SelectorColor", typeof(Color), typeof(TimeSlider), Color.Blue, propertyChanged: SelectorColorPropertyChanged);

        private static void SelectorColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (oldValue != null && oldValue != newValue)
            {
                ((TimeSlider)bindable).UpdateSeparatorColor();
            }
        }


        private static void SafezonePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (oldValue != null && oldValue != newValue)
            {
                ((TimeSlider)bindable).RecalculateCaches();
            }
        }


        private static void ItemWidthPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (oldValue != null && oldValue != newValue)
            {
                ((TimeSlider)bindable).UpdateLayout(((TimeSlider)bindable).Width, ((TimeSlider)bindable).Height);
            }
        }

        private static void TextsPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (oldValue != null && oldValue != newValue)
            {
                ((TimeSlider)bindable).UpdateLayout(((TimeSlider)bindable).Width, ((TimeSlider)bindable).Height);
            }
        }

        /// <summary>
        /// Recalculate the cached safgezonewidth and fadezonewidth.
        /// </summary>
        private void RecalculateCaches()
        {
            safezoneWidth = Convert.ToInt32(ItemWidth * SafezoneFraction);
            fadezoneWidth = Convert.ToInt32(ItemWidth * FadezoneFraction);
        }

        /// <summary>
        /// Update the separator color.
        /// </summary>
        private void UpdateSeparatorColor()
        {
            box.Color = SeparatorColor;
        }

        private int safezoneWidth = 50;
        private int fadezoneWidth = 50;

        private static void ImagePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (oldValue != null && oldValue != newValue)
            {
                ((TimeSlider)bindable).UpdateLayout(((TimeSlider)bindable).Width, ((TimeSlider)bindable).Height);
            }
        }

        /// <summary>
        /// The collection of displayed images in the slider.
        /// </summary>
        public ObservableCollection<ImageSource> Images
        {
            get { return (ObservableCollection<ImageSource>)GetValue(ImagesProperty); }
            set { SetValue(ImagesProperty, value); }
        }

        /// <summary>
        /// The collection of displayed texts in the slider.
        /// </summary>
        public ObservableCollection<string> Texts
        {
            get { return (ObservableCollection<string>)GetValue(TextsProperty); }
            set { SetValue(TextsProperty, value); }
        }

        /// <summary>
        /// The currently selected value.
        /// </summary>
        public double SelectedValue
        {
            get { return (double)GetValue(SelectedValueProperty); }
            set { SetValue(SelectedValueProperty, value); }
        }

        /// <summary>
        /// The width of one single element of the slider.
        /// </summary>
        public int ItemWidth
        {
            get { return (int)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        /// <summary>
        /// How much of one subelement of the slider should be used for not fading between values.
        /// </summary>
        public double SafezoneFraction
        {
            get { return (double)GetValue(SafezoneFractionProperty); }
            set { SetValue(SafezoneFractionProperty, value); }
        }

        /// <summary>
        /// How much of one subelement of the slider should be used for fading between values.
        /// </summary>
        public double FadezoneFraction
        {
            get { return (double)GetValue(FadezoneFractionProperty); }
            set { SetValue(FadezoneFractionProperty, value); }
        }

        /// <summary>
        /// The color of the separator indicating the currently selected value.
        /// </summary>
        public Color SeparatorColor
        {
            get { return (Color)GetValue(SelectorColorProperty); }
            set { SetValue(SelectorColorProperty, value); }
        }

        private double SliderX => slider.X + slider.TranslationX;
        private Grid slider;
        private BoxView box;
        private readonly TapGestureRecognizer tapGestureRecognizer;
        private readonly PanGestureRecognizer swipeGestureRecognizer;
        private bool disableTap;
        private bool initialLayout = true;

        public TimeSlider()
        {
            // init gesture recognizers
            swipeGestureRecognizer = new PanGestureRecognizer();
            swipeGestureRecognizer.PanUpdated += RecognizerOnPanUpdated;
            tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += TapGestureRecognizerOnTapped;

            InitControls();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            if (initialLayout)
            {
                initialLayout = false;
                // do initial layout
                UpdateLayout(width, height);
            }

            base.OnSizeAllocated(width, height);
        }

        private void InitControls()
        {
            // the separator
            box = new BoxView { Color = SeparatorColor };
            // construct the slider
            slider = new Grid() { ColumnSpacing = 0, RowSpacing = 0, BackgroundColor = Color.White };
        }


        /// <summary>
        /// Force the layout to be set.
        /// </summary>
        private void UpdateLayout(double width, double height)
        {
            if (initialLayout)
            {
                return;
            }

            AbsoluteLayout absoluteLayout = new AbsoluteLayout {BackgroundColor = Color.Gray};
            
            // background image
            FadeInImage background = new FadeInImage() { Source = ImageSource.FromFile("timeslider_background.png"), DownsampleToViewSize = true, Aspect = Aspect.Fill };

            absoluteLayout.Children.Add(background, new Rectangle(0, 0, width, height));

            int gridRows = 0;
            int gridColumns = 0;
            if (Images != null)
            {
                gridColumns = Math.Max(Images.Count, gridColumns);
                if (Images.Count > 0)
                    gridRows++;
            }
            if (Texts != null)
            {
                gridColumns = Math.Max(Texts.Count, gridColumns);
                if (Texts.Count > 0)
                    gridRows++;
            }
            for (int i = 0; i < gridColumns; i++)
            {
                slider.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(ItemWidth) });
            }
            for (int i = 0; i < gridRows; i++)
            {
                if (gridRows == 2)
                {
                    if (i == 0)
                        slider.RowDefinitions.Add(new RowDefinition { Height = new GridLength(HeightRequest * 0.8) });
                    else
                        slider.RowDefinitions.Add(new RowDefinition { Height = new GridLength(HeightRequest * 0.2) });
                }
                else
                {
                    slider.RowDefinitions.Add(new RowDefinition { Height = new GridLength(HeightRequest) });
                }
            }
            if (Images != null)
            {
                int i = 0;
                foreach (ImageSource imageSource in Images)
                {
                    CachedImage image = new CachedImage() { Source = imageSource, DownsampleToViewSize = true, WidthRequest = 50, Aspect = Aspect.AspectFill };
                    image.GestureRecognizers.Add(tapGestureRecognizer);
                    slider.Children.Add(image, i, 0);
                    i++;
                }
            }
            if (Texts != null)
            {
                int i = 0;
                foreach (string text in Texts)
                {
                    Label label = new Label() { Text = text, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
                    label.GestureRecognizers.Add(tapGestureRecognizer);
                    slider.Children.Add(label, i, 1);
                    i++;
                }
            }

            // put the pieces together
            AddSwipeGestureListener(slider);
            slider.ChildAdded += BottomSheetContentViewOnChildAdded;

            absoluteLayout.Children.Add(slider, new Point(width / 2 + 2 - (double)ItemWidth / 2, 0));
            absoluteLayout.Children.Add (box, new Rectangle(width/2, 0, 2, height));
            
            SelectedValue = 0;

            Content = absoluteLayout;
        }

        /// <summary>
        /// Calculate the selected value according to the slider position.
        /// </summary>
        /// <returns>The currently selected value.</returns>
        private double CalculateSelectedValue()
        {
            double dx = box.X - (box.Width / 2) - SliderX;

            if (dx < (double)fadezoneWidth / 2)
            {
                return 0;
            }
            else if (dx > slider.Width - (double)ItemWidth / 2)
            {
                return Images.Count - 1;
            }
            else
            {
                double result = Math.Floor((dx - (double)fadezoneWidth / 2) / (fadezoneWidth + safezoneWidth));
                double mod = (dx - (double)fadezoneWidth / 2) % (fadezoneWidth + safezoneWidth);
                if (mod > safezoneWidth)
                {
                    result += (mod - safezoneWidth) / fadezoneWidth;
                }

                return result;
            }
        }

        /// <summary>
        /// Animates the slider to the value.
        /// </summary>
        /// <param name="value">The value to animate to.</param>
        private void UpdateSliderAccordingToValue(int value)
        {
            double x = (value) * ItemWidth * (-1);
            slider.TranslateTo(x, 0, 100);
        }

        private void AddSwipeGestureListener(Layout layout)
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                // on android recursively add the gesture recognizer to all childs
                AddGestureRecognizer(layout, swipeGestureRecognizer);
            }
            else
            {
                // on ios only to the slider itself
                slider.GestureRecognizers.Add(swipeGestureRecognizer);
            }
        }


        private void TapGestureRecognizerOnTapped(object sender, EventArgs eventArgs)
        {
            if (!disableTap)
            {
                // move to the tapped element
                int column = Grid.GetColumn((BindableObject)sender);
                UpdateSliderAccordingToValue(column);
                SelectedValue = column;
            }
            else
            {
                // reenable tap recognizer for the next event
                disableTap = false;
            }
        }


        private void RecognizerOnPanUpdated(object sender, PanUpdatedEventArgs panUpdatedEventArgs)
        {
            if (Math.Abs(panUpdatedEventArgs.TotalX) > 1)
            {
                // disable the tap when detecting a swipe
                disableTap = true;
            }

            double dx;
            if (Device.RuntimePlatform == Device.Android)
            {
                // calculate the offset when the slider would be moved
                dx = slider.TranslationX + panUpdatedEventArgs.TotalX;
            }
            else
            {
                // on ios the animations needs to be scaled
                dx = slider.TranslationX + panUpdatedEventArgs.TotalX / 15;
            }
            if (slider.X + dx > box.X - box.Width / 2)
            {
                // indicator at the left side of the slider
                dx = (double)ItemWidth / 2;
            }
            else if (slider.X + slider.Width + dx < box.X + box.Width / 2)
            {
                // indicator at the right side of the slider
                dx = (double)ItemWidth / 2 - slider.Width;
            }

            if (Device.RuntimePlatform == Device.Android)
            {
                // use an animation for smooth scrolling
                ViewExtensions.CancelAnimations(slider);
                slider.TranslateTo(dx, 0, 50U);
            }
            else
            {
                // workaround for ios as it doesn't work with translation
                slider.TranslationX = dx;
            }

            // set the new selected value
            SelectedValue = CalculateSelectedValue();
            if (panUpdatedEventArgs.StatusType == GestureStatus.Completed)
            {
                // gesture is finished, go back to nearest value
                int value = Convert.ToInt32(Math.Round(SelectedValue));
                UpdateSliderAccordingToValue(value);
                SelectedValue = value;
            }
        }

        /// <summary>
        /// Add gesture recognizers to all existing children.
        /// </summary>
        /// <param name="view">The view to which teh gesture recognizer should be added.</param>
        /// <param name="recognizer">The gesture recognizer to be added.</param>
        private void AddGestureRecognizer(View view, GestureRecognizer recognizer)
        {
            if (!view.GestureRecognizers.Contains(recognizer))
                view.GestureRecognizers.Add(recognizer);
            if (view is Layout<View>)
            {
                foreach (View childView in ((Layout<View>)view).Children)
                {
                    AddGestureRecognizer(childView, recognizer);
                }
            }
        }

        /// <summary>
        /// Make sure children of the bottomsheet listen to swipe gestures.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="elementEventArgs">The event params.</param>
        private void BottomSheetContentViewOnChildAdded(object sender, ElementEventArgs elementEventArgs)
        {
            View view = (View)elementEventArgs.Element;
            AddGestureRecognizer(view, swipeGestureRecognizer);
            view.GestureRecognizers.Add(swipeGestureRecognizer);
            view.ChildAdded += BottomSheetContentViewOnChildAdded;
        }

    }
}