using Android.App;
using Android.Content.PM;
using Android.OS;
using de.upb.hip.mobile.droid;
using de.upb.hip.mobile.pcl.Common;
using de.upb.hip.mobile.pcl.Common.Contracts;
using HipMobileUI.Droid.Contracts;
using HipMobileUI.Navigation;
using Microsoft.Practices.Unity;

namespace HipMobileUI.Droid
{
    [Activity(Label = "Historisches Paderborn", Icon = "@drawable/ic_launcher", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            IoCManager.UnityContainer.RegisterType<IImageDimension, AndroidImageDimensions>();

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            // Init Navigation
            //NavigationService.Instance.RegisterViewModels(typeof(MainPage).Assembly);
            IoCManager.UnityContainer.RegisterInstance(typeof(INavigationService), NavigationService.Instance);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            Xamarin.FormsMaps.Init(this, bundle);
            LoadApplication(new App());
        }
    }
}

