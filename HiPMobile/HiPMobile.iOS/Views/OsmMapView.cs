using Foundation;
using System;
using HiPMobile.iOS;
using UIKit;
using MapKit;

namespace HiPMobile.iOS
{
    public partial class OsmMapView : MKMapView
    {
        ///I think there will be a need of delegade to implement some methods depending on where the map is placed
        /// check how is TableViewSource used in the HomeScreenViewController and MainViewController
        /// In this case you'll need 
        //public WeakReference<OsmMapViewDelegate> OsmDelegate { get; set; } // weak reference in order to avoid memory leaks (retain cycles)
        //helpful https://www.youtube.com/watch?v=xP7YvdlnHfA
        public OsmMapView (IntPtr handle) : base (handle)
        {
           
        }
    }
}