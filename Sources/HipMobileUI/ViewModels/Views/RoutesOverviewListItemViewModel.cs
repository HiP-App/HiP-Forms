using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using HipMobileUI.Helpers;
using HipMobileUI.Navigation;
using MvvmHelpers;
using Xamarin.Forms;

namespace HipMobileUI.ViewModels.Views {
    public class RoutesOverviewListItemViewModel : BaseViewModel {

        private readonly byte[] imageData;

        public RoutesOverviewListItemViewModel (string routeId)
        {
            var route = RouteManager.GetRoute (routeId);
            RouteTitle = route.Title;

            imageData = route.Image.Data;
            Image = ImageSource.FromStream(() => new MemoryStream(imageData));

            ItemSelectedCommand = new Command (OpenRouteDetails);
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

        public ICommand ItemSelectedCommand { get; set; }

        private async void OpenRouteDetails()
        {
            await NavigationService.Instance.PushAsync(new DummyViewModel
            {
                Color = Color.Pink
            });
        }
    }
}