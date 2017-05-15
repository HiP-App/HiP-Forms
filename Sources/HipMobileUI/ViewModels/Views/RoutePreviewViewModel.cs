using System.IO;
using System.Windows.Input;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Location;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views
{
    class RoutePreviewViewModel : NavigationViewModel
    {
        public RoutePreviewViewModel(Route route, INearbyRouteManager nearbyRouteManager)
        {
            RouteTitle = route.Title;
            RouteId = route.Id;
            Question = Strings.ExhibitOrRouteNearby_Question_Part1 + " \"" + RouteTitle + "\" " + Strings.ExhibitOrRouteNearby_Question_Part2;
            RouteDescription = route.Description;
            var data = route.Image.Data;
            Image = ImageSource.FromStream(() => new MemoryStream(data));
            NearbyRouteManager = nearbyRouteManager;

            Confirm = new Command (Accept);
            Decline = new Command (Deny);
        }

        private INearbyRouteManager NearbyRouteManager { get; set; }
        public string Question { set; get; }
        public string RouteTitle { get; set; }
        public string RouteId { get; set; }
        public string RouteDescription { get; set; }
        public ImageSource Image { set; get; }

        public ICommand Confirm { get; }
        public ICommand Decline { get; }

        void Accept()
        {
            Deny ();
            NearbyRouteManager.OpenRouteDetailsView (RouteId);
        }

        void Deny()
        {
            NearbyRouteManager.ClosePopUp();
        }

    }
}
