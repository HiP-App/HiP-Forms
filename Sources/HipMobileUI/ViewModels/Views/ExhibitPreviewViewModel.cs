using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Location;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views
    {
    class ExhibitPreviewViewModel : NavigationViewModel
        {

            public ExhibitPreviewViewModel (Exhibit exhibit, INearbyExhibitManager exhibitManager)
            {
            ExhibitName = exhibit.Name;
            Question = Strings.ExhibitOrRouteNearby_Question_Part1 + " \"" + ExhibitName + "\" " + Strings.ExhibitOrRouteNearby_Question_Part2;
            var data = exhibit.Image.Data;
            Image = ImageSource.FromStream (() => new MemoryStream (data));
            ExhibitManager = exhibitManager;

            Confirm = new Command (Accept);
            Decline = new Command (Deny);
            }

        private INearbyExhibitManager ExhibitManager { get; }
        public string Question { set; get; }
        public string ExhibitName { get; set; }
        public ImageSource Image { set; get; }


        public ICommand Confirm { get; }
        public ICommand Decline { get; }

        void Accept ()
            {
            MessagingCenter.Send<INearbyExhibitManager, bool> (ExhibitManager, "ReturnValue", true);
            }
        void Deny ()
            {
            MessagingCenter.Send<INearbyExhibitManager, bool> (ExhibitManager, "ReturnValue", false);
            }

        }
    }
