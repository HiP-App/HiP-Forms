using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.Common;
using de.upb.hip.mobile.pcl.Common.Contracts;
using de.upb.hip.mobile.pcl.DataAccessLayer;
using de.upb.hip.mobile.pcl.DataLayer;
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
            IoCManager.UnityContainer.RegisterType<IDataAccess, RealmDataAccess>();
            IoCManager.UnityContainer.RegisterType<IDataLoader, EmbeddedResourceDataLoader>();
            DbManager.UpdateDatabase();


            MainPage = new ContentPage (); 
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
