using System;
using System.Threading.Tasks;
using HipMobileUI.Controls;
using Xamarin.Forms;
using Rectangle = Xamarin.Forms.Rectangle;

namespace HipMobileUI.Views
{
    public class BottomSheetView : ContentView {

        private readonly double BottomSheetExtensionFraction = 0.4;
        private BottomSheetState bottomSheetState = BottomSheetState.Collapsed;

        public BottomSheetView()
        {
            var layout = new AbsoluteLayout ();

            MainContentView = new ContentView ();
            AbsoluteLayout.SetLayoutFlags (MainContentView, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds (MainContentView, new Rectangle(0,0,1,0.9));

            BottomSheetContentView = new ContentView();
            BottomSheetContentView.BackgroundColor = Color.White;
            AbsoluteLayout.SetLayoutFlags(BottomSheetContentView, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(BottomSheetContentView, new Rectangle(0, 1, 1, 0.1));
            var gestureRecognizer = new PanGestureRecognizer ();
            gestureRecognizer.PanUpdated+=GestureRecognizerOnPanUpdated;
            BottomSheetContentView.GestureRecognizers.Add (gestureRecognizer);

            button = new FloatingActionButton
            {
                NormalColor = (Color) Application.Current.Resources ["AccentColor"],
                Command = new Command (ButtonOnClicked),
                Icon = "ic_keyboard_arrow_up"
            };
            AbsoluteLayout.SetLayoutFlags(button, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(button, new Rectangle(0.9, 0.92, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));

            layout.Children.Add (MainContentView);
            layout.Children.Add(BottomSheetContentView);
            layout.Children.Add (button);

            Content = layout;
        }

        private async void GestureRecognizerOnPanUpdated (object sender, PanUpdatedEventArgs panUpdatedEventArgs)
        {
            if (panUpdatedEventArgs.StatusType == GestureStatus.Running)
            {
                // Set the state to pending if not done already
                if (bottomSheetState == BottomSheetState.Collapsed || bottomSheetState == BottomSheetState.Extended)
                {
                    await SetBottomSheetPending();
                }
                // Update the swipe direction
                bottomSheetState = panUpdatedEventArgs.TotalY < 0 ? BottomSheetState.Extending : BottomSheetState.Collapsing;
            }
            else if(panUpdatedEventArgs.StatusType == GestureStatus.Completed)
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

        private async Task ExtendBottomSheet() {
            Rectangle bottomSheetRect = new Rectangle
            {
                Left = 0,
                Top = Height * (1 - BottomSheetExtensionFraction),
                Size = new Size(Width, Height * BottomSheetExtensionFraction)
            };
            Rectangle buttonRect = new Rectangle
            {
                Top = bottomSheetRect.Top - button.Height / 2,
                Size = new Size (button.Width, button.Height),
                X = button.X
            };
            await Task.WhenAll(BottomSheetContentView.LayoutTo(bottomSheetRect), button.LayoutTo(buttonRect), button.RotateXTo(180));
            bottomSheetState = BottomSheetState.Extended;
        }

        private async Task CollapseBottomSheet()
        {
            Rectangle bottomSheetRect = new Rectangle
            {
                Left = 0,
                Top = Height * 0.9,
                Size = new Size(Width, Height * 0.1)
            };
            Rectangle buttonRect = new Rectangle
            {
                Top = bottomSheetRect.Top - button.Height / 2,
                Size = new Size(button.Width, button.Height),
                X = button.X
            };
            await Task.WhenAll(BottomSheetContentView.LayoutTo(bottomSheetRect), button.LayoutTo(buttonRect), button.RotateXTo(0));
            bottomSheetState = BottomSheetState.Collapsed;
        }

        private async Task SetBottomSheetPending()
        {
            Rectangle bottomSheetRect = new Rectangle
            {
                Left = 0,
                Top = Height * (1 - BottomSheetExtensionFraction / 2),
                Size = new Size(Width, Height * BottomSheetExtensionFraction /2)
            };
            Rectangle buttonRect = new Rectangle
            {
                Top = bottomSheetRect.Top - button.Height / 2,
                Size = new Size(button.Width, button.Height),
                X = button.X
            };
            await Task.WhenAll(BottomSheetContentView.LayoutTo(bottomSheetRect), button.LayoutTo(buttonRect));
        }

        #endregion

        #region properties

        private FloatingActionButton button;

        public ContentView MainContentView;
        public static readonly BindableProperty MainContentProperty =
            BindableProperty.Create("MainContent", typeof(View), typeof(BottomSheetView), null, propertyChanged: MainContentPropertyChanged);
        private static void MainContentPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((BottomSheetView)bindable).MainContentView.Content = (View)newValue;
        }
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
        public View BottomSheet
        {
            get { return (View)GetValue(MainContentProperty); }
            set { SetValue(MainContentProperty, value); }
        }
    }

    #endregion


    public enum BottomSheetState {
        Collapsed,
        Collapsing,
        Extending,
        Extended
    }
}
