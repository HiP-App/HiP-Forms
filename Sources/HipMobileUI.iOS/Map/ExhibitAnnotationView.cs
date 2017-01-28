using System;
using System.Collections.Generic;
using System.Text;
using MapKit;
using UIKit;

namespace HipMobileUI.iOS.Map
{
    sealed class ExhibitAnnotationView : MKAnnotationView {

        public ExhibitAnnotationView (IMKAnnotation annotation, string reuseId) : base (annotation, reuseId)
        {
            Image = UIImage.FromBundle ("ExhibitLocation");
        }
    }
}
