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
using System.Threading.Tasks;
using FFImageLoading.Forms;
using Xamarin.Forms;

namespace HipMobileUI.Controls
{
    class ViewSlider : ContentView {

        public static readonly BindableProperty ImagesProperty =
            BindableProperty.Create ("Images", typeof (ObservableCollection<ImageSource>), typeof (ViewSlider), new ObservableCollection<ImageSource> (), propertyChanged:ImagePropertyChanged);
        public static readonly BindableProperty SelectedValueProperty =
            BindableProperty.Create("SelectedValue", typeof(double), typeof(ViewSlider), 0.0);
        public static readonly BindableProperty TextsProperty =
            BindableProperty.Create("Texts", typeof(ObservableCollection<string>), typeof(ViewSlider), new ObservableCollection<string>(), propertyChanged: TextsPropertyChanged);
        public static readonly BindableProperty ItemWidthProperty =
            BindableProperty.Create("ItemWidth", typeof(int), typeof(ViewSlider), 100, propertyChanged: ItemWidthPropertyChanged);
        public static readonly BindableProperty SafezoneFractionProperty =
            BindableProperty.Create("SafezoneFraction", typeof(double), typeof(ViewSlider), 0.5, propertyChanged: SafezonePropertyChanged);
        public static readonly BindableProperty FadezoneFractionProperty =
            BindableProperty.Create("FadezoneFraction", typeof(double), typeof(ViewSlider), 0.5, propertyChanged: SafezonePropertyChanged);
        public static readonly BindableProperty SelectorColorProperty =
            BindableProperty.Create("SelectorColor", typeof(Color), typeof(ViewSlider), Color.Blue, propertyChanged: SelectorColorPropertyChanged);

        private static void SelectorColorPropertyChanged (BindableObject bindable, object oldValue, object newValue)
        {
            if (oldValue != null && oldValue != newValue)
            {
                ((ViewSlider)bindable).UpdateSeparatorColor();
            }
        }


        private static void SafezonePropertyChanged (BindableObject bindable, object oldValue, object newValue)
        {
            if (oldValue != null && oldValue != newValue)
            {
                ((ViewSlider)bindable).RecalculateCaches();
            }
        }

        

        private static void ItemWidthPropertyChanged (BindableObject bindable, object oldValue, object newValue)
        {
            if (oldValue != null && oldValue != newValue)
            {
                ((ViewSlider)bindable).UpdateLayout();
            }
        }

        private static void TextsPropertyChanged (BindableObject bindable, object oldValue, object newValue)
        {
            if (oldValue != null && oldValue != newValue)
            {
                ((ViewSlider)bindable).UpdateLayout();
            }
        }

        private void RecalculateCaches ()
        {
            safezoneWidth = Convert.ToInt32 (ItemWidth * SafezoneFraction);
            fadezoneWidth = Convert.ToInt32(ItemWidth * FadezoneFraction);
        }

        private void UpdateSeparatorColor ()
        {
            box.Color = SeparatorColor;
        }

        private int safezoneWidth = 50;
        private int fadezoneWidth = 50;

        private static void ImagePropertyChanged (BindableObject bindable, object oldValue, object newValue)
        {
            if (oldValue != null && oldValue != newValue)
            {
                ((ViewSlider)bindable).UpdateLayout ();
            }
        }

        public ObservableCollection<ImageSource> Images
        {
            get { return (ObservableCollection<ImageSource>)GetValue(ImagesProperty); }
            set { SetValue(ImagesProperty, value); }
        }

        public ObservableCollection<string> Texts
        {
            get { return (ObservableCollection<string>)GetValue(TextsProperty); }
            set { SetValue(TextsProperty, value); }
        }

        public double SelectedValue
        {
            get { return (double)GetValue(SelectedValueProperty); }
            set { SetValue(SelectedValueProperty, value); }
        }

        public int ItemWidth
        {
            get { return (int)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        public double SafezoneFraction
        {
            get { return (double)GetValue(SafezoneFractionProperty); }
            set { SetValue(SafezoneFractionProperty, value); }
        }

        public double FadezoneFraction
        {
            get { return (double)GetValue(FadezoneFractionProperty); }
            set { SetValue(FadezoneFractionProperty, value); }
        }

        public Color SeparatorColor
        {
            get { return (Color)GetValue(SelectorColorProperty); }
            set { SetValue(SelectorColorProperty, value); }
        }

        private Grid slider;
        private BoxView box;
        private double SliderX => slider.X + slider.TranslationX;
        public ViewSlider ()
        {
            UpdateLayout ();
        }

        /// <summary>
        /// Force the layout to be set.
        /// </summary>
        private void UpdateLayout ()
        {
            RelativeLayout layout = new RelativeLayout();
            layout.BackgroundColor = Color.Gray;

            box = new BoxView { Color = SeparatorColor };

            slider = new Grid() {ColumnSpacing = 0, RowSpacing = 0, BackgroundColor = Color.White};
            int gridRows = 0;
            int gridColumns = 0;
            if (Images != null)
            {
                gridColumns = Math.Max (Images.Count, gridColumns);
                if(Images.Count>0)gridRows++;
            }
            if (Texts != null)
            {
                gridColumns = Math.Max(Texts.Count, gridColumns);
                if (Texts.Count > 0) gridRows++;
            }
            for(int i=0;i<gridColumns;i++)
            {
                 slider.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(ItemWidth) });
            }
            for (int i = 0; i < gridRows; i++)
            {
                if (gridRows == 2)
                {
                    if (i == 0)
                        slider.RowDefinitions.Add (new RowDefinition {Height = new GridLength (HeightRequest * 0.8)});
                    else
                        slider.RowDefinitions.Add (new RowDefinition {Height = new GridLength (HeightRequest * 0.2)});
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
                    CachedImage image = new CachedImage () {Source = imageSource, DownsampleToViewSize = true, WidthRequest = 50, Aspect = Aspect.AspectFill};
                    slider.Children.Add (image, i, 0);
                    i++;
                }
            }
            if (Texts != null)
            {
                int i = 0;
                foreach (string text in Texts)
                {
                    Label label = new Label() { Text = text, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center};
                    slider.Children.Add(label, i, 1);
                    i++;
                }
            }

            AddListener(slider);
            slider.ChildAdded += BottomSheetContentViewOnChildAdded;

            layout.Children.Add(slider, Constraint.RelativeToView(box, (relativeLayout, view) => view.X + box.Width - ItemWidth/2), Constraint.RelativeToView(box, (relativeLayout, view) => view.Y ));
            layout.Children.Add(box, Constraint.RelativeToParent(parent => parent.Width / 2), Constraint.Constant(0), Constraint.Constant(2), Constraint.RelativeToParent(parent => parent.Height));

            SelectedValue = 0;

            Content = layout;
        }

        /// <summary>
        /// Calculate the selected value according to the slider position.
        /// </summary>
        /// <returns>The currently selected value.</returns>
        private double CalculateSelectedValue ()
        {
            double dx = box.X - (box.Width / 2) - SliderX;

            if (dx < fadezoneWidth / 2)
            {
                return 0;
            }
            else
            {
                double result = Math.Floor((dx - fadezoneWidth / 2) / (fadezoneWidth + safezoneWidth));
                double mod = (dx - fadezoneWidth / 2) % (fadezoneWidth + safezoneWidth);
                if (mod > safezoneWidth)
                {
                    result += (mod-safezoneWidth) / fadezoneWidth;
                }
                
                System.Diagnostics.Debug.WriteLine (result);
                return result;
            }
        }

        /// <summary>
        /// Animates the slider to the value.
        /// </summary>
        /// <param name="value">The value to animate to.</param>
        private void UpdateSliderAccordingToValue (int value)
        {
            double x = (value) * (fadezoneWidth + safezoneWidth) *(-1);
            slider.TranslateTo (x, 0, 100);
        }

        private void AddListener (Layout layout)
        {
            swipeGestureRecognizer = new PanGestureRecognizer ();
            swipeGestureRecognizer.PanUpdated+=RecognizerOnPanUpdated;
            if (Device.OS == TargetPlatform.Android)
            {
                // on android recursively add the gesture recognizer to all childs
                AddGestureRecognizer (layout, swipeGestureRecognizer);
            }
            else
            {
                // on ios only to the slider itself
                slider.GestureRecognizers.Add(swipeGestureRecognizer);
            }
        }

        private PanGestureRecognizer swipeGestureRecognizer;

        private void RecognizerOnPanUpdated (object sender, PanUpdatedEventArgs panUpdatedEventArgs)
        {      
            double dx;
            if (Device.OS == TargetPlatform.Android)
            {
                dx = slider.TranslationX + panUpdatedEventArgs.TotalX;
            }
            else
            {
                // on ios the animations needs to be scaled
                dx = slider.TranslationX + panUpdatedEventArgs.TotalX / 15;
            }
            if (slider.X + dx > box.X-box.Width/2)
            {
                // left side of the slider
                dx = ItemWidth/2;
            }
            else if (slider.X + slider.Width +dx < box.X + box.Width/2)
            {
                // right side of the slider
                dx = ItemWidth/2- slider.Width;
            }

            if (Device.OS == TargetPlatform.Android)
            {
                // use an animation for smooth scrolling
                ViewExtensions.CancelAnimations (slider);
                slider.TranslateTo(dx, 0, 50U);
            }
            else
            {
                // workaround for ios as it doesn't work with translationx
                slider.TranslationX = dx;
            }

            // set the new selected value
            SelectedValue = CalculateSelectedValue ();
            if (panUpdatedEventArgs.StatusType == GestureStatus.Completed)
            {
                // gesture is finished, go back to nearest value
                int value = Convert.ToInt32 (Math.Round (SelectedValue));
                UpdateSliderAccordingToValue (value);
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
