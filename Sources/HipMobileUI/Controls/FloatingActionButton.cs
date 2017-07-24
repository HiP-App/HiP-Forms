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

using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Controls
{
    /// <summary>
    /// Floating action button for android and ios in Xamarin forms. Do not set the width manually as the android support library only supports two sizes and the view will break when set otherwise.
    /// If the width is needed for layouting it can be obtained with the <see cref="IosSize"/> on ios and with the IFabSizeCalculator interface on android.
    /// <example>
    /// The following sample shows the size calculation on android:
    /// <code>
    /// IoCManager.Resolve&lt;IFabSizeCalculator&gt; ().CalculateFabSize ()
    /// </code>
    /// </example>
    ///  </summary>
    public class FloatingActionButton : View {

        public static readonly int IosSize = 55;

        public FloatingActionButton ()
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                // on iOS set the size manually
                WidthRequest = IosSize;
                HeightRequest = IosSize;
            }
        }

        #region NormalColor
        public static readonly BindableProperty NormalColorProperty=
            BindableProperty.Create ("NormalColor", typeof (Color), typeof (FloatingActionButton), Color.White, propertyChanged:NormalColorPropertyChanged);

        /// <summary>
        /// The background color of the button.
        /// </summary>
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

        /// <summary>
        /// The command invoked when the button is pressed.
        /// </summary>
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

        /// <summary>
        /// The path to the icon displayed on the button. The icon needs to be added to each platform specific project. <see href="https://developer.xamarin.com/guides/xamarin-forms/working-with/images/">Further details</see>
        /// </summary>
        public string Icon
        {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        #endregion

        #region RippleColor
        public static readonly BindableProperty RippleColorProperty =
            BindableProperty.Create("RippleColor", typeof(Color), typeof(FloatingActionButton), Color.Gray, propertyChanged: RippleColorPropertyChanged);

        /// <summary>
        /// The color fo the ripple effect.
        /// </summary>
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
