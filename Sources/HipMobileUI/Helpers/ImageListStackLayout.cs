using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace HipMobileUI.Helpers
{
    /// <summary>
    /// Extension of the <see cref="StackLayout"/> to enable displaying a dynmaic list of images
    /// </summary>
    public class ImageListStackLayout : StackLayout
    {
        public static readonly BindableProperty ChildElementsProperty = BindableProperty.Create(nameof(ChildElements), typeof(ObservableCollection<ImageSource>), typeof(ImageListStackLayout), propertyChanged: ChildElementsPropertyChanged);

        /// <summary>
        /// Should be bound to the property of the viewmodel providing the image sources
        /// </summary>
        public ObservableCollection<ImageSource> ChildElements
        {
            get { return (ObservableCollection<ImageSource>)GetValue(ChildElementsProperty); }
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
            var layout = bindable as ImageListStackLayout;
            var newList = newValue as ObservableCollection<ImageSource>;
            if (newList != null && layout != null)
            {
                layout.Children.Clear ();
                foreach (var imageSource in newList)
                {
                    layout.Children.Add(new Image { Source = imageSource });
                }
            }
        }
    }
}