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

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Controls
{
    /// <summary>
    /// A control that displays images and can fade between them. The displayed image is set by the selectedvalue property.
    /// A value between two integers will result in both images beeing shown with transparency according to the digits after comma.
    /// </summary>
    class FadeoverImageView : ContentView
    {
        public FadeoverImageView()
        {
            displayedImages = new List<CachedImage>();
        }

        public static readonly BindableProperty ImagesProperty =
            BindableProperty.Create("Images", typeof(ObservableCollection<ImageSource>), typeof(FadeoverImageView), new ObservableCollection<ImageSource>(),
                                    propertyChanged: ImagesPropertyChanged);

        public static readonly BindableProperty SelectedValueProperty =
            BindableProperty.Create("SelectedValue", typeof(double), typeof(FadeoverImageView), 0.0, propertyChanged: SelectedValuePropertyChanged);

        private static void SelectedValuePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (oldValue != null && oldValue != newValue)
            {
                ((FadeoverImageView) bindable).UpdateTransparencies();
            }
        }

        private static void ImagesPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (oldValue != null && oldValue != newValue)
            {
                ((FadeoverImageView) bindable).UpdateLayout();
            }
        }

        /// <summary>
        /// The displayed images of this control.
        /// </summary>
        public ObservableCollection<ImageSource> Images
        {
            get { return (ObservableCollection<ImageSource>) GetValue(ImagesProperty); }
            set { SetValue(ImagesProperty, value); }
        }

        /// <summary>
        /// The selected value of the control influencing which images are shown with what transparency.
        /// </summary>
        public double SelectedValue
        {
            get { return (double) GetValue(SelectedValueProperty); }
            set { SetValue(SelectedValueProperty, value); }
        }

        private readonly List<CachedImage> displayedImages;

        /// <summary>
        /// Forces the control to update its layout, by adding the Images via an AbsoluteLayout, 
        /// with maximised properties (see <see cref="AbsoluteLayout.SetLayoutBounds"/> with a Rectangle of width and height of 1). 
        /// </summary>
        private void UpdateLayout()
        {
            AbsoluteLayout layout = new AbsoluteLayout();
            displayedImages.Clear();
            foreach (ImageSource imageSource in Images)
            {
                CachedImage img = new CachedImage() { Source = imageSource, Aspect = Aspect.AspectFit }; // CachedImage do not DownsampleToViewSize, as it becomes blurry
                AbsoluteLayout.SetLayoutFlags(img, AbsoluteLayoutFlags.All);
                AbsoluteLayout.SetLayoutBounds(img, new Rectangle(0, 0, 1, 1));
                layout.Children.Add(img);
                displayedImages.Add(img);
            }
            Content = layout;
            UpdateTransparencies();
        }

        /// <summary>
        /// Updates the images transparency according to the <see cref="SelectedValue"/>.
        /// </summary>
        private void UpdateTransparencies()
        {
            int i = 0;
            foreach (CachedImage displayedImage in displayedImages)
            {
                var distance = Math.Abs(i - SelectedValue);
                if (distance < 1)
                {
                    displayedImage.Opacity = 1 - distance;
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