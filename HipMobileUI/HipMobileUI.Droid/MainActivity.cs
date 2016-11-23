using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using de.upb.hip.mobile.pcl.Common;
using de.upb.hip.mobile.pcl.Common.Contracts;
using HipMobileUI.Droid.Contracts;
using Microsoft.Practices.Unity;

namespace HipMobileUI.Droid
{
    [Activity(Label = "HipMobileUI", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            IoCManager.UnityContainer.RegisterType<IImageDimension, AndroidImageDimensions>();

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
        }
    }
}

