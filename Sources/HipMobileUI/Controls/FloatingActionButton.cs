using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace HipMobileUI.Controls
{
    public class FloatingActionButton : View {

        public static readonly BindableProperty NormalColorProperty=
            BindableProperty.Create ("NormalColor", typeof (Color), typeof (FloatingActionButton), Color.White);

        public Color NormalColor
        {
            get { return (Color)GetValue(NormalColorProperty); }
            set { SetValue(NormalColorProperty, value); }
        }

        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create("Command", typeof(Command), typeof(FloatingActionButton), new Command(() => {}));

        public Command Command
        {
            get { return (Command)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

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
    }
}
