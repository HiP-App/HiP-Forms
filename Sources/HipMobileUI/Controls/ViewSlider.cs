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

        private int s = 50;
        private int f = 50;

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

        private Grid slider;
        private BoxView box;
        public ViewSlider ()
        {
            UpdateLayout ();
        }

        private void UpdateLayout ()
        {
            RelativeLayout layout = new RelativeLayout();
            layout.BackgroundColor = Color.Gray;

            box = new BoxView { Color = Color.Blue };

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

        private double CalculateSelectedValue ()
        {
            double dx = box.X - (box.Width / 2) - slider.X;

            if (dx < f / 2)
            {
                System.Diagnostics.Debug.WriteLine(0);
                return 0;
            }
            else
            {
                double result = Math.Floor((dx - f / 2) / (f + s));
                double mod = (dx - f / 2) % (f + s);
                if (mod > s)
                {
                    result += (mod-s) / f;
                }
                
                System.Diagnostics.Debug.WriteLine (result);
                return result;
            }
        }

        private void UpdateSliderAccordingToValue (int value)
        {
            Rectangle sliderRect = slider.Bounds;
            sliderRect.X = box.X + box.Width / 2 - (value+1) * (f + s) + (f + s) / 2;
            slider.LayoutTo (sliderRect);
        }

        private void AddListener (Layout layout)
        {
            swipeGestureRecognizer = new PanGestureRecognizer ();
            swipeGestureRecognizer.PanUpdated+=RecognizerOnPanUpdated;
            AddGestureRecognizer(layout, swipeGestureRecognizer);
        }

        private PanGestureRecognizer swipeGestureRecognizer;

        private void RecognizerOnPanUpdated (object sender, PanUpdatedEventArgs panUpdatedEventArgs)
        {      

            if (slider.X <= box.X && slider.X + slider.Width >= box.X+box.Width)
            {
                double dx = panUpdatedEventArgs.TotalX;
                if (Device.OS == TargetPlatform.iOS)
                {
                    dx = panUpdatedEventArgs.TotalX * 0.1;
                }
                if (slider.X + dx > box.X-box.Width/2)
                {
                    dx = box.X - slider.X;
                }
                else if (slider.X + slider.Width +dx < box.X + box.Width/2)
                {
                    dx = slider.X + slider.Width - box.X + box.Width/2;
                }
                System.Diagnostics.Debug.WriteLine (sender.ToString ()+ ": " + panUpdatedEventArgs.TotalX + ", "+ panUpdatedEventArgs.GestureId);

                if (Device.OS == TargetPlatform.Android)
                {
                    var rect = slider.Bounds;
                    rect.X = rect.X + dx;
                    var a=slider.LayoutTo (rect, 50U);
                }
                else
                {
                    var rect = slider.Bounds;
                    rect.X = rect.X + dx;
                    var a = slider.LayoutTo(rect, 0U);
                }
                System.Diagnostics.Debug.WriteLine ("dx: " + dx);
                SelectedValue = CalculateSelectedValue ();
            }
            if (panUpdatedEventArgs.StatusType == GestureStatus.Completed)
            {
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
