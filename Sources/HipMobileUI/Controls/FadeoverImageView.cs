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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FFImageLoading.Forms;
using Xamarin.Forms;

namespace HipMobileUI.Controls
{
    class FadeoverImageView : ContentView
    {

        public FadeoverImageView ()
        {
            displayedImages = new List<CachedImage> ();
        }

        public static readonly BindableProperty ImagesProperty =
            BindableProperty.Create("Images", typeof(ObservableCollection<ImageSource>), typeof(FadeoverImageView), new ObservableCollection<ImageSource>(), propertyChanged:ImagesPropertyChanged);
        public static readonly BindableProperty SelectedValueProperty =
            BindableProperty.Create("SelectedValue", typeof(double), typeof(FadeoverImageView), 0.0, propertyChanged: SelectedValuePropertyChanged);

        private static void SelectedValuePropertyChanged (BindableObject bindable, object oldValue, object newValue)
        {
            if (oldValue != null && oldValue != newValue)
            {
                ((FadeoverImageView)bindable).UpdateTransparencies();
            }
        }

        private static void ImagesPropertyChanged (BindableObject bindable, object oldValue, object newValue)
        {
            if (oldValue != null && oldValue != newValue)
            {
                ((FadeoverImageView)bindable).UpdateLayout();
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

        private List<CachedImage> displayedImages;

        private void UpdateLayout ()
        {
            AbsoluteLayout layout = new AbsoluteLayout();
            displayedImages.Clear ();
            foreach (ImageSource imageSource in Images)
            {
                CachedImage img = new CachedImage () {Source = imageSource, Aspect = Aspect.AspectFit, DownsampleToViewSize = true};
                AbsoluteLayout.SetLayoutFlags (img, AbsoluteLayoutFlags.PositionProportional);
                AbsoluteLayout.SetLayoutBounds (img, new Rectangle(.5, .5, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
                layout.Children.Add (img);
                displayedImages.Add (img);
            }
            Content = layout;
            UpdateTransparencies ();
        }

        private void UpdateTransparencies ()
        {
            int i = 0;
            foreach (CachedImage displayedImage in displayedImages)
            {
                var distance = Math.Abs (i - SelectedValue);
                if (distance < 1)
                {
                    displayedImage.Opacity = 1-distance;
                    displayedImage.IsVisible = true;
                }
                else
                {
                    displayedImage.IsVisible = false;
                }
                i++;
            }
        }

    }
}
