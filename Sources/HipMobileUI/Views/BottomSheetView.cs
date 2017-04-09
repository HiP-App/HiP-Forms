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
    public class BottomSheetView : ContentView {

        /// <summary>
        /// The fraction of how much the bottom sheet is extended relative to the whole view.
        /// </summary>
        private readonly double bottomSheetExtensionFraction = 0.35;
        private BottomSheetState bottomSheetState = BottomSheetState.Collapsed;
        private FloatingActionButton Button { get; }

        public BottomSheetView ()
        {
            RelativeLayout layout = new RelativeLayout ();

            // Main content
            mainContentView = new ContentView ();
            layout.Children.Add (mainContentView, Constraint.RelativeToParent (parent => parent.X), Constraint.RelativeToParent (parent => parent.Y),
                                 Constraint.RelativeToParent (parent => parent.Width), Constraint.RelativeToParent (parent => parent.Height));

            // Bottomsheet
            BottomSheetContentView = new ContentView {BackgroundColor = Color.White};
            layout.Children.Add (BottomSheetContentView, Constraint.RelativeToParent (parent => parent.X), Constraint.RelativeToParent (parent => parent.Height - 64),
                                 Constraint.RelativeToParent (parent => parent.Width), Constraint.RelativeToParent (parent => parent.Height));

            var resources = IoCManager.Resolve<ApplicationResourcesProvider>();
            // Floating Action Button
            Button = new FloatingActionButton
            {
                NormalColor = (Color)resources.GetResourceValue("AccentColor"),
                RippleColor = (Color)resources.GetResourceValue("AccentDarkColor"),
                Command = new Command (ButtonOnClicked),
                Icon = "ic_keyboard_arrow_up",
                AutomationId = "Fab"
            };

            double fabSize;
            if (Device.OS == TargetPlatform.iOS)
            {
                fabSize = FloatingActionButton.IosSize;
            }
            else
            {
                fabSize = IoCManager.Resolve<IFabSizeCalculator> ().CalculateFabSize ();
            }

            layout.Children.Add (Button, Constraint.RelativeToParent (parent => parent.Width * 0.9 - fabSize),
                                 Constraint.RelativeToView (BottomSheetContentView, (parent, view) => view.Y - fabSize / 2));

            Content = layout;

           // restore the state when the layout changes
            layout.LayoutChanged+=LayoutOnLayoutChanged;
        }

        /// <summary>
        /// React to layout changes, for example if the device was rotated.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="eventArgs">The event params.</param>
        private async void LayoutOnLayoutChanged (object sender, EventArgs eventArgs)
        {
            // restore the state
            if (bottomSheetState == BottomSheetState.Extended || bottomSheetState == BottomSheetState.Extending)
            {
                await ExtendBottomSheet ();
            }
        }

        #region BottomSheet Gesture
        /// <summary>
        /// Add gesture recognizers to all existing children.
        /// </summary>
        /// <param name="view">The view to which teh gesture recognizer should be added.</param>
        /// <param name="recognizer">The gesture recognizer to be added.</param>
        private void AddGestureRecognizer (View view, GestureRecognizer recognizer)
        {
            if(!view.GestureRecognizers.Contains (recognizer))
                view.GestureRecognizers.Add (recognizer);
            if (view is Layout<View>)
            {
                foreach (View childView in ((Layout<View>)view).Children)
                {
                    AddGestureRecognizer (childView, recognizer);
                }
            }
        }

        /// <summary>
        /// Remove existing gesture recognizers.
        /// </summary>
        /// <param name="view">The view from which the gesture recognizers should be removed.</param>
        /// <param name="recognizer">The recognizer to remove.</param>
        private void RemoveGestureRecognizer(View view, GestureRecognizer recognizer)
        {
            if (!view.GestureRecognizers.Contains(recognizer))
                view.GestureRecognizers.Remove(recognizer);
            if (view is Layout<View>)
            {
                foreach (View childView in ((Layout<View>)view).Children)
                {
                    RemoveGestureRecognizer(childView, recognizer);
                }
            }
        }

        /// <summary>
        /// React to pan events on the bottom sheet.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="panUpdatedEventArgs">The event args.</param>
        private async void GestureRecognizerOnPanUpdated (object sender, PanUpdatedEventArgs panUpdatedEventArgs)
        {
            if (panUpdatedEventArgs.StatusType == GestureStatus.Running)
            {
                // Set the state to pending if not done already
                if (bottomSheetState == BottomSheetState.Collapsed || bottomSheetState == BottomSheetState.Extended)
                {
                    await SetBottomSheetPending ();
                }
                // Update the swipe direction
                bottomSheetState = panUpdatedEventArgs.TotalY < 0 ? BottomSheetState.Extending : BottomSheetState.Collapsing;
            }
            else if (panUpdatedEventArgs.StatusType == GestureStatus.Completed || panUpdatedEventArgs.StatusType == GestureStatus.Canceled)
            {
                // Perform final animation
                if (bottomSheetState == BottomSheetState.Extending)
                {
                    await ExtendBottomSheet ();
                }
                else if (bottomSheetState == BottomSheetState.Collapsing)
                {
                    await CollapseBottomSheet ();
                }
            }
        }
        #endregion

        /// <summary>
        /// React to a click on the FAB.
        /// </summary>
        private async void ButtonOnClicked ()
        {
            if (bottomSheetState == BottomSheetState.Collapsed)
            {
                await ExtendBottomSheet ();
            }
            else if (bottomSheetState == BottomSheetState.Extended)
            {
                await CollapseBottomSheet ();
            }
        }

        #region bottomsheet manipulation

        /// <summary>
        /// Layout the bottom sheet to its extended state, with animation.
        /// </summary>
        /// <returns>The task.</returns>
        private async Task ExtendBottomSheet ()
        {
            Rectangle bottomSheetRect = new Rectangle
            {
                Left = 0,
                Top = Height * (1 - bottomSheetExtensionFraction),
                Size = new Size (Width, Height * bottomSheetExtensionFraction)
            };
            Rectangle buttonRect = new Rectangle
            {
                Top = bottomSheetRect.Top - Button.Height / 2,
                Size = new Size (Button.Width, Button.Height),
                X = Button.X
            };
            await Task.WhenAll (BottomSheetContentView.LayoutTo (bottomSheetRect), Button.LayoutTo (buttonRect), Button.RotateXTo (180));
            bottomSheetState = BottomSheetState.Extended;
        }

        /// <summary>
        /// Layout the bottom sheet to its collapsed state, with animation.
        /// </summary>
        /// <returns>The task.</returns>
        private async Task CollapseBottomSheet ()
        {
            Rectangle bottomSheetRect = new Rectangle
            {
                Left = 0,
                Top = Height - 64,
                Size = new Size (Width, 64)
            };
            Rectangle buttonRect = new Rectangle
            {
                Top = bottomSheetRect.Top - Button.Height / 2,
                Size = new Size (Button.Width, Button.Height),
                X = Button.X
            };
            await Task.WhenAll (BottomSheetContentView.LayoutTo (bottomSheetRect), Button.LayoutTo (buttonRect), Button.RotateXTo (0));
            bottomSheetState = BottomSheetState.Collapsed;
        }

        /// <summary>
        /// Layout the botton sheet to it's pending state, with animation.
        /// </summary>
        /// <returns>The task.</returns>
        private async Task SetBottomSheetPending ()
        {
            Rectangle bottomSheetRect = new Rectangle
            {
                Left = 0,
                Top = Height * (1 - bottomSheetExtensionFraction / 2),
                Size = new Size (Width, Height * bottomSheetExtensionFraction / 2)
            };
            Rectangle buttonRect = new Rectangle
            {
                Top = bottomSheetRect.Top - Button.Height / 2,
                Size = new Size (Button.Width, Button.Height),
                X = Button.X
            };
            await Task.WhenAll (BottomSheetContentView.LayoutTo (bottomSheetRect), Button.LayoutTo (buttonRect));
        }

        #endregion

        #region properties
        private readonly ContentView mainContentView;

        public static readonly BindableProperty MainContentProperty =
            BindableProperty.Create ("MainContent", typeof (View), typeof (BottomSheetView), null, propertyChanged: MainContentPropertyChanged);

        private static void MainContentPropertyChanged (BindableObject bindable, object oldValue, object newValue)
        {
            ((BottomSheetView) bindable).mainContentView.Content = (View) newValue;
        }

        /// <summary>
        /// The View displaying the main content.
        /// </summary>
        public View MainContent {
            get { return (View) GetValue (MainContentProperty); }
            set { SetValue (MainContentProperty, value); }
        }

        public ContentView BottomSheetContentView;

        public static readonly BindableProperty BottomSheetProperty =
            BindableProperty.Create ("BottomSheet", typeof (View), typeof (BottomSheetView), null, propertyChanged: BottomSheetPropertyChanged);

        private static void BottomSheetPropertyChanged (BindableObject bindable, object oldValue, object newValue)
        {
            ((BottomSheetView) bindable).BottomSheetContentView.Content = (View) newValue;
        }

        /// <summary>
        /// The view displaying the bottom sheet content.
        /// </summary>
        public View BottomSheet {
            get { return (View) GetValue (MainContentProperty); }
            set { SetValue (MainContentProperty, value); }
        }


        #endregion
    }

    /// <summary>
    /// States for a bottom sheet.
    /// </summary>
    public enum BottomSheetState {
        Collapsed,
        Collapsing,
        Extending,
        Extended
    }
}
