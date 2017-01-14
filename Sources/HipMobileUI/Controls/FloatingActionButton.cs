using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace HipMobileUI.Controls
{
    public class FloatingActionButton : View {

        public static readonly int IosSize = 55;

        public FloatingActionButton ()
        {
            if (Device.OS == TargetPlatform.iOS)
            {
                // on iOS set the size manually
                WidthRequest = IosSize;
                HeightRequest = IosSize;
            }
        }

        #region NormalColor
        public static readonly BindableProperty NormalColorProperty=
            BindableProperty.Create ("NormalColor", typeof (Color), typeof (FloatingActionButton), Color.White, propertyChanged:NormalColorPropertyChanged);
        public Color NormalColor
        {
            get { return (Color)GetValue(NormalColorProperty); }
            set { SetValue(NormalColorProperty, value); }
        }

        private static void NormalColorPropertyChanged (BindableObject bindable, object oldValue, object newValue)
        {
            if (oldValue != newValue)
                ((FloatingActionButton)bindable).NormalColorChanged?.Invoke((Color)newValue);
        }
        public delegate void ColorChangedEventhandler(Color newColor);
        public event ColorChangedEventhandler NormalColorChanged;
        
        #endregion

        #region Command
        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create("Command", typeof(Command), typeof(FloatingActionButton), new Command(() => {}), propertyChanged:CommandPropertyChanged);
        public Command Command
        {
            get { return (Command)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        private static void CommandPropertyChanged (BindableObject bindable, object oldValue, object newValue)
        {
            if (oldValue != newValue)
                ((FloatingActionButton)bindable).CommandChanged?.Invoke((Command) newValue);
        }
        public delegate void CommandChangedEventHandler(Command newCommand);
        public event CommandChangedEventHandler CommandChanged;

        #endregion

        #region Icon
        public static readonly BindableProperty IconProperty =
            BindableProperty.Create("Icon", typeof(string), typeof(FloatingActionButton), null, propertyChanged:IconPropertyChanged);
        private static void IconPropertyChanged (BindableObject bindable, object oldValue, object newValue)
        {
            if (oldValue != newValue)
                ((FloatingActionButton) bindable).IconChanged?.Invoke (newValue.ToString ());
        }
        public delegate void IconChangedHandler (string newIcon);
        public event IconChangedHandler IconChanged;

        public string Icon
        {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        #endregion

        #region RippleColor
        public static readonly BindableProperty RippleColorProperty =
            BindableProperty.Create("RippleColor", typeof(Color), typeof(FloatingActionButton), Color.Gray, propertyChanged: RippleColorPropertyChanged);
        public Color RippleColor
        {
            get { return (Color)GetValue(RippleColorProperty); }
            set { SetValue(RippleColorProperty, value); }
        }

        private static void RippleColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (oldValue != newValue)
                ((FloatingActionButton)bindable).RippleColorChanged?.Invoke((Color)newValue);
        }
        public event ColorChangedEventhandler RippleColorChanged;

        #endregion

    }
}
