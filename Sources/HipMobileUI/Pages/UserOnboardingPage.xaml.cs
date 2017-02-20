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

using de.upb.hip.mobile.pcl.Common;
using FFImageLoading.Forms;
using HipMobileUI.Contracts;
using HipMobileUI.Controls;
using HipMobileUI.Helpers;
using HipMobileUI.Navigation;
using HipMobileUI.Resources;
using HipMobileUI.ViewModels.Pages;
using Xamarin.Forms;

namespace HipMobileUI.Pages
{
    public partial class UserOnboardingPage : ContentPage, IViewFor<UserOnboardingPageViewModel> {

        private DeviceOrientation orientation;

        private UserOnboardingPageViewModel ViewModel => (UserOnboardingPageViewModel) BindingContext;

        public UserOnboardingPage()
        {
            InitializeComponent();
            orientation = DeviceOrientation.Undefined;

            // hide the status bar for this page
            IStatusBarController statusBarController = IoCManager.Resolve<IStatusBarController> ();
            statusBarController.HideStatusBar();
        }

        /// <summary>
        /// Size changed, determine if we need to update the layout.
        /// </summary>
        /// <param name="width">The new width.</param>
        /// <param name="height">The new height.</param>
        protected override void OnSizeAllocated (double width, double height)
        {
            base.OnSizeAllocated (width, height);

            if (width < height && orientation != DeviceOrientation.Portrait)
            {
                // inform the viewmodels that layout will change
                foreach (UserOnboardingItemViewModel userOnboardingItemViewModel in ViewModel.Pages)
                {
                    userOnboardingItemViewModel.OrientationChanged (DeviceOrientation.Portrait);
                }

                // change layout to portrait
                Content = CreateView (StackOrientation.Vertical);
                orientation = DeviceOrientation.Portrait;
            }
            if (width > height && orientation != DeviceOrientation.Landscape)
            {
                // inform the viewmodels that layout will change
                foreach (UserOnboardingItemViewModel userOnboardingItemViewModel in ViewModel.Pages)
                {
                    userOnboardingItemViewModel.OrientationChanged(DeviceOrientation.Landscape);
                }

                // change layout to landscape
                Content = CreateView (StackOrientation.Horizontal);
                orientation = DeviceOrientation.Landscape;
            }
            
        }

        /// <summary>
        /// Creates this view according to the orientation.
        /// </summary>
        /// <param name="newOrientation">The desired orientation.</param>
        /// <returns>The created view.</returns>
        private View CreateView (StackOrientation newOrientation)
        {
            RelativeLayout layout = new RelativeLayout();

            // create the carousel
            CarouselView carousel = new CarouselView();
            carousel.SetBinding(CarouselView.PositionProperty, "SelectedPage");
            carousel.SetBinding(ItemsView.ItemsSourceProperty, "Pages");
            if (newOrientation == StackOrientation.Vertical)
            {
                carousel.ItemTemplate = new DataTemplate (LoadPortraitTemplate);
            }
            else
            {
                carousel.ItemTemplate = new DataTemplate(LoadLandscapeTemplate);
            }

            // bottom buttons
            var skipLabel = new Label() { Text = Strings.UserOnboarding_Skip, TextColor = Color.White };
            skipLabel.SetBinding(Label.IsVisibleProperty, "IsForwardVisible");
            var finishTapGestureRecognizer = new TapGestureRecognizer();
            finishTapGestureRecognizer.SetBinding(TapGestureRecognizer.CommandProperty, "FinishCommand");
            skipLabel.GestureRecognizers.Add(finishTapGestureRecognizer);
            var forwardLabel = new Label() { Text = Strings.UserOnboarding_Forward, TextColor = Color.White };
            forwardLabel.SetBinding(Label.IsVisibleProperty, "IsForwardVisible");
            var forwardGestureRecognizer = new TapGestureRecognizer();
            forwardGestureRecognizer.SetBinding(TapGestureRecognizer.CommandProperty, "ForwardCommand");
            forwardLabel.GestureRecognizers.Add(forwardGestureRecognizer);
            var okLabel = new Label() { Text = Strings.UserOnboarding_Ok, TextColor = Color.White };
            okLabel.SetBinding(Label.IsVisibleProperty, "IsFinishVisible");
            okLabel.GestureRecognizers.Add(finishTapGestureRecognizer);
            var indicators = new CarouselIndicators() { IndicatorWidth = 10, IndicatorHeight = 10, UnselectedIndicator = "unselected_circle.png", SelectedIndicator = "selected_circle.png" };
            indicators.SetBinding(CarouselIndicators.PositionProperty, "SelectedPage");
            indicators.SetBinding(CarouselIndicators.ItemsSourceProperty, "Pages");
            var separator = new BoxView() { Color = Color.Silver, HeightRequest = 1 };

            // add pieces together
            layout.Children.Add (carousel, widthConstraint: Constraint.RelativeToParent (parent => parent.Width),
                                 heightConstraint: Constraint.RelativeToParent (parent => parent.Height));
            layout.Children.Add (skipLabel, xConstraint: Constraint.Constant (10), yConstraint: Constraint.RelativeToParent (parent => parent.Height - 30));
            layout.Children.Add (forwardLabel, xConstraint: Constraint.RelativeToParent (parent => parent.Width - 50),
                                 yConstraint: Constraint.RelativeToParent (parent => parent.Height - 30));
            layout.Children.Add(okLabel, xConstraint: Constraint.RelativeToParent(parent => parent.Width - 80),
                                 yConstraint: Constraint.RelativeToParent(parent => parent.Height - 30));
            layout.Children.Add (indicators, xConstraint: Constraint.RelativeToParent (parent => parent.Width * 0.5 - 15),
                                 yConstraint: Constraint.RelativeToParent (parent => parent.Height - 30));
            layout.Children.Add (separator, yConstraint: Constraint.RelativeToView (skipLabel, (parent, view) => view.Y - 10),
                                 widthConstraint: Constraint.RelativeToParent (parent => parent.Width));


            return layout;
        }

        /// <summary>
        /// Loads the template for the carousel in landscape layout.
        /// </summary>
        /// <returns>The view used as the template.</returns>
        private View LoadLandscapeTemplate()
        {
            // main content
            StackLayout outerStack = new StackLayout() { Orientation = StackOrientation.Horizontal, Padding = new Thickness(50, 50) };
            outerStack.SetBinding(StackLayout.BackgroundColorProperty, "BackgroundColor");
            CachedImage image = new CachedImage() { VerticalOptions = LayoutOptions.StartAndExpand, Aspect = Aspect.AspectFit, HorizontalOptions = LayoutOptions.StartAndExpand };
            image.SetBinding(CachedImage.SourceProperty, "Image");
            StackLayout innerStack = new StackLayout() { Orientation = StackOrientation.Vertical, HorizontalOptions = LayoutOptions.EndAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand};
            Label headline = new Label() { TextColor = Color.White, FontSize = 18, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center };
            headline.SetBinding(Label.TextProperty, "Headline");
            Label text = new Label() { TextColor = Color.White, FontSize = 12, HorizontalTextAlignment = TextAlignment.Center };
            text.SetBinding(Label.TextProperty, "Text");
            innerStack.Children.Add(headline);
            innerStack.Children.Add(text);
            outerStack.Children.Add(image);
            outerStack.Children.Add(innerStack);
            return outerStack;
        }

        /// <summary>
        /// Loads the template for the carousel in portrait layout.
        /// </summary>
        /// <returns>The view used as the template.</returns>
        private View LoadPortraitTemplate()
        {
            // main content
            StackLayout layout = new StackLayout() { Orientation = StackOrientation.Vertical, Padding = new Thickness(0, 80) };
            layout.SetBinding(StackLayout.BackgroundColorProperty, "BackgroundColor");
            CachedImage image = new CachedImage() { VerticalOptions = LayoutOptions.CenterAndExpand, Aspect = Aspect.AspectFit, HorizontalOptions = LayoutOptions.FillAndExpand };
            image.SetBinding(CachedImage.SourceProperty, "Image");
            StackLayout innerStack = new StackLayout() { Orientation = StackOrientation.Vertical, HorizontalOptions = LayoutOptions.CenterAndExpand };
            Label headline = new Label() { TextColor = Color.White, VerticalOptions = LayoutOptions.CenterAndExpand, FontSize = 18, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center };
            headline.SetBinding(Label.TextProperty, "Headline");
            Label text = new Label() { TextColor = Color.White, VerticalOptions = LayoutOptions.CenterAndExpand, FontSize = 12, HorizontalTextAlignment = TextAlignment.Center };
            text.SetBinding(Label.TextProperty, "Text");
            innerStack.Children.Add(headline);
            innerStack.Children.Add(text);
            layout.Children.Add(image);
            layout.Children.Add(innerStack);
            return layout;
        }

    }
}
