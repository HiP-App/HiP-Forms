using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using HipMobileUI.ViewModels;

namespace HipMobileUI.Map
{
    class MapViewModel : NavigationViewModel
    {
        private ExhibitSet exhibitSet;

        //TODO just for testing can be deleted later
        public MapViewModel ()
        {
            Title = "Map";
            ExhibitSet = ExhibitManager.GetExhibitSets().First();
        }

       

        public ExhibitSet ExhibitSet {
            get { return exhibitSet; }
            set { SetProperty (ref exhibitSet, value); }
        }

    }
}
