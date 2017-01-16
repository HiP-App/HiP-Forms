using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace HipMobileUI.Container
{
    /// <summary>
    /// Extension of the <see cref="StackLayout"/> to enable displaying a dynmaic list of images
    /// </summary>
    public class BindableChildrenStackLayout : StackLayout
    {
        public static readonly BindableProperty ChildElementsProperty = BindableProperty.Create(nameof(ChildElements), typeof(ObservableCollection<View>), typeof(BindableChildrenStackLayout), propertyChanged: ChildElementsPropertyChanged);

        /// <summary>
        /// Should be bound to the property of the viewmodel providing the image sources
        /// </summary>
        public ObservableCollection<View> ChildElements
        {
            get { return (ObservableCollection<View>)GetValue(ChildElementsProperty); }
            set { SetValue(ChildElementsProperty, value); }
        }

        /// <summary>
        /// Called if the property <see cref="ChildElements"/> is set. Adds the set image sources as images to the stack layout
        /// </summary>
        /// <param name="bindable"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        private static void ChildElementsPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var layout = bindable as BindableChildrenStackLayout;
            var newList = newValue as ObservableCollection<View>;
            if (newList != null && layout != null)
            {
                layout.Children.Clear ();
                foreach (var view in newList)
                {
                    layout.Children.Add (view);
                }
            }
        }
    }
}