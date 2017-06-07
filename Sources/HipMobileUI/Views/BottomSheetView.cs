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

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Controls;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers;
using Xamarin.Forms;
using Rectangle = Xamarin.Forms.Rectangle;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Views
{
    /// <summary>
    /// A view consisting of a extensible bottom sheet and a main content view. The bottom sheet can be extended either by swiping or by a FAB.
    /// </summary>
    public class BottomSheetView : ContentView
    {
        /// <summary>
        /// The fraction of how much the bottom sheet is extended relative to the whole view.
        /// </summary>
        private readonly double bottomSheetExtensionFraction = 0.35;
        private BottomSheetState bottomSheetState = BottomSheetState.Collapsed;
        private FloatingActionButton Button { get; set; }
        private bool initLayout = true;

        public BottomSheetView()
        {
            mainContentView = new ContentView();
            BottomSheetContentView = new ContentView { BackgroundColor = Color.White };
            var resources = IoCManager.Resolve<ApplicationResourcesProvider>();
            // Floating Action Button
            Button = new FloatingActionButton
            {
                NormalColor = (Color)resources.GetResourceValue("AccentColor"),
                RippleColor = (Color)resources.GetResourceValue("AccentDarkColor"),
                Command = new Command(ButtonOnClicked),
                Icon = "ic_keyboard_arrow_up",
                AutomationId = "Fab"
            };
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            if (initLayout)
            {
                AbsoluteLayout absoluteLayout = new AbsoluteLayout();
                absoluteLayout.Children.Add(mainContentView, new Rectangle(0, 0, width, height));
                absoluteLayout.Children.Add(BottomSheetContentView, new Rectangle(0, height - 64, width, 64));

                double fabSize;
                if (Device.OS == TargetPlatform.iOS)
                {
                    fabSize = FloatingActionButton.IosSize;
                }
                else
                {
                    fabSize = IoCManager.Resolve<IFabSizeCalculator>().CalculateFabSize();
                }
                absoluteLayout.Children.Add(Button, new Point(width * 0.9 - fabSize, height - 64 - fabSize / 2));

                Content = absoluteLayout;

                // restore the state when the layout changes
                absoluteLayout.LayoutChanged += OnLayoutChanged;
                initLayout = false;
            }
            base.OnSizeAllocated(width, height);
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (!BottomSheetVisible)
            {
                Button.IsVisible = false;
                BottomSheetContentView.IsVisible = false;
            }
        }

        /// <summary>
        /// React to layout changes, for example if the device was rotated.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="eventArgs">The event params.</param>
        private async void OnLayoutChanged(object sender, EventArgs eventArgs)
        {
            // restore the state
            if (bottomSheetState == BottomSheetState.Extended || bottomSheetState == BottomSheetState.Extending)
            {
                await ExtendBottomSheet();
            }
        }

        /// <summary>
        /// React to a click on the FAB.
        /// </summary>
        private async void ButtonOnClicked()
        {
            if (bottomSheetState == BottomSheetState.Collapsed)
            {
                await ExtendBottomSheet();
            }
            else if (bottomSheetState == BottomSheetState.Extended)
            {
                await CollapseBottomSheet();
            }
        }

        #region bottomsheet manipulation

        /// <summary>
        /// Layout the bottom sheet to its extended state, with animation.
        /// </summary>
        /// <returns>The task.</returns>
        private async Task ExtendBottomSheet()
        {
            Rectangle bottomSheetRect = new Rectangle
            {
                Left = 0,
                Top = Height * (1 - bottomSheetExtensionFraction),
                Size = new Size(Width, Height * bottomSheetExtensionFraction)
            };
            Rectangle buttonRect = new Rectangle
            {
                Top = bottomSheetRect.Top - Button.Height / 2,
                Size = new Size(Button.Width, Button.Height),
                X = Button.X
            };
            await Task.WhenAll(BottomSheetContentView.LayoutTo(bottomSheetRect), Button.LayoutTo(buttonRect), Button.RotateXTo(180));
            bottomSheetState = BottomSheetState.Extended;
        }

        /// <summary>
        /// Layout the bottom sheet to its collapsed state, with animation.
        /// </summary>
        /// <returns>The task.</returns>
        private async Task CollapseBottomSheet()
        {
            Rectangle bottomSheetRect = new Rectangle
            {
                Left = 0,
                Top = Height - 64,
                Size = new Size(Width, 64)
            };
            Rectangle buttonRect = new Rectangle
            {
                Top = bottomSheetRect.Top - Button.Height / 2,
                Size = new Size(Button.Width, Button.Height),
                X = Button.X
            };
            await Task.WhenAll(BottomSheetContentView.LayoutTo(bottomSheetRect), Button.LayoutTo(buttonRect), Button.RotateXTo(0));
            bottomSheetState = BottomSheetState.Collapsed;
        }
       
        #endregion

        #region properties
        private ContentView mainContentView;

        public static readonly BindableProperty MainContentProperty =
            BindableProperty.Create("MainContent", typeof(View), typeof(BottomSheetView), null, propertyChanged: MainContentPropertyChanged);

        private static void MainContentPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((BottomSheetView)bindable).mainContentView.Content = (View)newValue;
        }

        /// <summary>
        /// The View displaying the main content.
        /// </summary>
        public View MainContent
        {
            get { return (View)GetValue(MainContentProperty); }
            set { SetValue(MainContentProperty, value); }
        }

        public ContentView BottomSheetContentView;

        public static readonly BindableProperty BottomSheetProperty =
            BindableProperty.Create("BottomSheet", typeof(View), typeof(BottomSheetView), null, propertyChanged: BottomSheetPropertyChanged);

        private static void BottomSheetPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((BottomSheetView)bindable).BottomSheetContentView.Content = (View)newValue;
        }

        /// <summary>
        /// The view displaying the bottom sheet content.
        /// </summary>
        public View BottomSheet
        {
            get { return (View)GetValue(MainContentProperty); }
            set { SetValue(MainContentProperty, value); }
        }


        public static readonly BindableProperty BottomSheetVisibleProperty = BindableProperty.Create(nameof(BottomSheetVisible), typeof(bool), typeof(BottomSheetView), defaultValue: true, propertyChanged: BottomSheetVisiblePropertyChanged);

        private static void BottomSheetVisiblePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((BottomSheetView)bindable).BottomSheetVisible = (bool)newValue;
        }

        /// <summary>
        /// Should be bound to the property of the viewmodel providing the data for the views
        /// so that the value converter can create views for this data.
        /// </summary>
        public bool BottomSheetVisible
        {
            get { return (bool)GetValue(BottomSheetVisibleProperty); }
            set { SetValue(BottomSheetVisibleProperty, value); }
        }


        #endregion
    }

    /// <summary>
    /// States for a bottom sheet.
    /// </summary>
    public enum BottomSheetState
    {
        Collapsed,
        Collapsing,
        Extending,
        Extended
    }
}
