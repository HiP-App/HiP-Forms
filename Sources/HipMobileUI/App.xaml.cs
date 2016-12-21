using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.Common;
using de.upb.hip.mobile.pcl.Common.Contracts;
using de.upb.hip.mobile.pcl.DataAccessLayer;
using de.upb.hip.mobile.pcl.DataLayer;
using HipMobileUI.Navigation;
using Microsoft.Practices.Unity;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace HipMobileUI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Handle when your app starts

            // Setup IoC and database
            IoCManager.UnityContainer.RegisterType<IDataAccess, RealmDataAccess>();
            IoCManager.UnityContainer.RegisterType<IDataLoader, EmbeddedResourceDataLoader>();
            IoCManager.UnityContainer.RegisterInstance (typeof (IViewCreator), NavigationService.Instance);
            DbManager.UpdateDatabase();

            MainPage = new NavigationPage(new ContentPage () {Title = "Test"});
        }

        protected override void OnStart()
        {
            
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
