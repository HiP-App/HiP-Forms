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
            BindableProperty.Create("SelectedValueProperty", typeof(double), typeof(ViewSlider), 0.0);

        private int s = 25;
        private int f = 25;

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

        public double SelectedValue
        {
            get { return (double)GetValue(SelectedValueProperty); }
            set { SetValue(SelectedValueProperty, value); }
        }

        private StackLayout stacklayout;
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
            

            stacklayout = new StackLayout() { Orientation = StackOrientation.Horizontal, Spacing = 0, BackgroundColor = Color.Aqua, HeightRequest = this.HeightRequest};

            foreach (ImageSource imageSource in Images)
            {
                CachedImage image = new CachedImage () {Source = imageSource, DownsampleToViewSize = true, WidthRequest = 50, Aspect = Aspect.AspectFill};
                stacklayout.Children.Add (image);
            }

/*            BoxView[] views = new BoxView[3];
            views[0] = new BoxView() { WidthRequest = 50, Color = Color.Red };
            views[1] = new BoxView() { WidthRequest = 50, Color = Color.Blue };
            views[2] = new BoxView() { WidthRequest = 50, Color = Color.White };

            
            foreach (BoxView boxView in views)
            {
                stacklayout.Children.Add(boxView);
            }*/
            AddListener(stacklayout);
            stacklayout.ChildAdded += BottomSheetContentViewOnChildAdded;

            layout.Children.Add(stacklayout, Constraint.RelativeToView(box, (relativeLayout, view) => view.X + box.Width), Constraint.RelativeToView(box, (relativeLayout, view) => view.Y ));
            layout.Children.Add(box, Constraint.RelativeToParent(parent => parent.Width / 2), Constraint.Constant(0), Constraint.Constant(2), Constraint.RelativeToParent(parent => parent.Height));

            SelectedValue = 0;

            Content = layout;
        }

        private double CalculateSelectedValue ()
        {
            double dx = box.X - (box.Width / 2) - stacklayout.X;

            if (dx < f / 2)
            {
                System.Diagnostics.Debug.WriteLine(0);
                return 0;
            }
            else
            {
                double result = Math.Floor((dx - f / 2) / (f + s));
                result += ((dx - f / 2) % (f + s) / (f + s));
                //System.Diagnostics.Debug.WriteLine (result);
                return result;
            }
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
            if (stacklayout.X + panUpdatedEventArgs.TotalX >= 0 && stacklayout.X + stacklayout.Width + panUpdatedEventArgs.TotalX <= Width)
            {
                //System.Diagnostics.Debug.WriteLine (sender.ToString ()+ ": " + panUpdatedEventArgs.TotalX + ", "+ panUpdatedEventArgs.GestureId);

                var rect = stacklayout.Bounds;
                rect.X = rect.X + panUpdatedEventArgs.TotalX;
                stacklayout.LayoutTo(rect, 50U);
                SelectedValue = CalculateSelectedValue ();
            }
            if (panUpdatedEventArgs.StatusType == GestureStatus.Completed)
            {
                
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
