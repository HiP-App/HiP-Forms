using System.IO;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using MvvmHelpers;
using Xamarin.Forms;

namespace HipMobileUI.ViewModels.Views {
    public class RoutesOverviewListItemViewModel : BaseViewModel {

        private readonly byte[] imageData;

        public Route Route { get; }

        public RoutesOverviewListItemViewModel (string routeId)
        {
            Route = RouteManager.GetRoute (routeId);

            RouteTitle = Route.Title;
            RouteDescription = Route.Description;
            Duration = GetRouteDurationText (Route.Duration);
            Distance = GetRouteDistanceText (Route.Distance);

            var tags = Route.RouteTags;

            imageData = Route.Image.Data;
            Image = ImageSource.FromStream(() => new MemoryStream(imageData));
        }

        private string GetRouteDistanceText (double routeDistance)
        {
            return string.Format ("{0} km", routeDistance);
        }

        private string GetRouteDurationText (int routeDuration)
        {
            int durationInMinutes = routeDuration / 60;
            return string.Format ("{0} Minuten", durationInMinutes);
        }

        private ImageSource image; 
        public ImageSource Image
        {
            get { return image; }
            set { SetProperty(ref image, value); }
        }

        private string routeTitle;
        public string RouteTitle
        {
            get { return routeTitle; }
            set { SetProperty (ref routeTitle, value); }
        }

        private string routeDescription;
        public string RouteDescription
        {
            get { return routeDescription; }
            set { SetProperty(ref routeDescription, value); }
        }

        private string duration;
        public string Duration
        {
            get { return duration; }
            set { SetProperty(ref duration, value); }
        }

        private string distance;
        public string Distance
        {
            get { return distance; }
            set { SetProperty(ref distance, value); }
        }
    }
}