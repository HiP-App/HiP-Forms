using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using Xamarin.Forms;

namespace HipMobileUI.Map {
    public class OsmMap : View {

        public static readonly BindableProperty ExhibitSetProperty =
            BindableProperty.Create ("ExhibitSet", typeof (ExhibitSet), typeof (OsmMap), null, propertyChanged: ExhibitPropertyChanged);

        // Property accessor
        public ExhibitSet ExhibitSet {
            get { return (ExhibitSet) GetValue (ExhibitSetProperty); }
            set { SetValue (ExhibitSetProperty, value); }
        }

        // method listening for changes of the property
        private static void ExhibitPropertyChanged (BindableObject bindable, object oldValue, object newValue)
        {
            // check if the property really changed
            if (oldValue == null || !oldValue.Equals (newValue))
            {
                // inform all listeners that the ExhibitSet changed
                var map = (OsmMap) bindable;
                map.ExhibitSetChanged?.Invoke (map.ExhibitSet);
            }
        }

        public delegate void ExhibitSetChangedHandler (ExhibitSet set);

        public event ExhibitSetChangedHandler ExhibitSetChanged;

    }
}