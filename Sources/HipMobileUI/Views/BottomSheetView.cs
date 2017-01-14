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

using System.Threading.Tasks;
using de.upb.hip.mobile.pcl.Common;
using de.upb.hip.mobile.pcl.Common.Contracts;
using HipMobileUI.Controls;
using Xamarin.Forms;
using Rectangle = Xamarin.Forms.Rectangle;

namespace HipMobileUI.Views
{
    public class BottomSheetView : ContentView {

        private readonly double bottomSheetExtensionFraction = 0.4;
        private BottomSheetState bottomSheetState = BottomSheetState.Collapsed;
        private readonly PanGestureRecognizer swipeGestureRecognizer;

        public BottomSheetView ()
        {
            RelativeLayout layout = new RelativeLayout ();

            // Main content
            MainContentView = new ContentView ();
            layout.Children.Add (MainContentView, Constraint.RelativeToParent (parent => parent.X), Constraint.RelativeToParent (parent => parent.Y),
                                 Constraint.RelativeToParent (parent => parent.Width), Constraint.RelativeToParent (parent => parent.Height));

            // Bottomsheet
            BottomSheetContentView = new ContentView {BackgroundColor = Color.White};
            swipeGestureRecognizer = new PanGestureRecognizer ();
            swipeGestureRecognizer.PanUpdated += GestureRecognizerOnPanUpdated;
            BottomSheetContentView.GestureRecognizers.Add (swipeGestureRecognizer);
            layout.Children.Add (BottomSheetContentView, Constraint.RelativeToParent (parent => parent.X), Constraint.RelativeToParent (parent => parent.Height * 0.9),
                                 Constraint.RelativeToParent (parent => parent.Width), Constraint.RelativeToParent (parent => parent.Height));

            // Floating Action Button
            Button = new FloatingActionButton
            {
                NormalColor = (Color) Application.Current.Resources ["AccentColor"],
                RippleColor = (Color) Application.Current.Resources ["AccentDarkColor"],
                Command = new Command (ButtonOnClicked),
                Icon = "ic_keyboard_arrow_up"
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

            BottomSheetContentView.ChildAdded+=BottomSheetContentViewOnChildAdded;
            BottomSheetContentView.ChildRemoved-=BottomSheetContentViewOnChildRemoved;
        }


        #region BottomSheet Gesture
        private void BottomSheetContentViewOnChildAdded (object sender, ElementEventArgs elementEventArgs)
        {
            View view = (View)elementEventArgs.Element;
            AddGestureRecognizer (view, swipeGestureRecognizer);
            view.GestureRecognizers.Add (swipeGestureRecognizer);
            view.ChildAdded += BottomSheetContentViewOnChildAdded;
        }

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

        private void BottomSheetContentViewOnChildRemoved(object sender, ElementEventArgs elementEventArgs)
        {
            View view = (View)elementEventArgs.Element;
            RemoveGestureRecognizer (view, swipeGestureRecognizer);
            view.GestureRecognizers.Remove(swipeGestureRecognizer);
            view.ChildRemoved -= BottomSheetContentViewOnChildRemoved;
        }
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

        private async Task CollapseBottomSheet ()
        {
            Rectangle bottomSheetRect = new Rectangle
            {
                Left = 0,
                Top = Height * 0.9,
                Size = new Size (Width, Height * 0.1)
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

        public FloatingActionButton Button { get; }

        public ContentView MainContentView;

        public static readonly BindableProperty MainContentProperty =
            BindableProperty.Create ("MainContent", typeof (View), typeof (BottomSheetView), null, propertyChanged: MainContentPropertyChanged);

        private static void MainContentPropertyChanged (BindableObject bindable, object oldValue, object newValue)
        {
            ((BottomSheetView) bindable).MainContentView.Content = (View) newValue;
        }

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

            // Add the swipe gesture recognizer to the new content to catch swipe event on it
            //((BottomSheetView)bindable).BottomSheetContentView.Content.GestureRecognizers.Add (((BottomSheetView)bindable).swipeGestureRecognizer);
        }

        public View BottomSheet {
            get { return (View) GetValue (MainContentProperty); }
            set { SetValue (MainContentProperty, value); }
        }


        #endregion
    }

    public enum BottomSheetState {
        Collapsed,
        Collapsing,
        Extending,
        Extended
    }
}
