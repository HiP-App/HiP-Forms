using System.Collections.ObjectModel;
using System.IO;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using MvvmHelpers;
using Xamarin.Forms;

namespace HipMobileUI.ViewModels.Views {

    /// <summary>
    /// View model of a list item in the <see cref="RoutesOverviewViewModel"/> screen
    /// </summary>
    public class RoutesOverviewListItemViewModel : BaseViewModel {

        /// <summary>
        /// Route data displayed by this list item
        /// </summary>
        public Route Route { get; }

        /// <summary>
        /// Creates a list item using the provided route data
        /// </summary>
        /// <param name="route"></param>
        public RoutesOverviewListItemViewModel (Route route)
        {
            Route = route;
            RouteTitle = Route.Title;
            RouteDescription = Route.Description;
            Duration = GetRouteDurationText (Route.Duration);
            Distance = GetRouteDistanceText (Route.Distance);

            Tags = new ObservableCollection<ImageSource> ();

            foreach (var tag in Route.RouteTags)
            {
                // Required to reference first due to threading problems in Realm
                byte[] currentTagImageData  = tag.Image.Data;

                Tags.Add (ImageSource.FromStream(() => new MemoryStream(currentTagImageData)));
            }

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

        /// <summary>
        /// Required due to threading problems in Realm
        /// </summary>
        private readonly byte[] imageData;
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

        private ObservableCollection<ImageSource> tags;
        public ObservableCollection<ImageSource> Tags
        {
            get { return tags; }
            set { SetProperty(ref tags, value); }
        }
    }
}