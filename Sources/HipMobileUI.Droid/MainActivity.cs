// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using Android.App;
using Android.Content.PM;
using Android.OS;
using de.upb.hip.mobile.droid.Contracts;
using de.upb.hip.mobile.pcl.Common;
using de.upb.hip.mobile.pcl.Common.Contracts;
using FFImageLoading.Forms.Droid;
using HipMobileUI;
using HipMobileUI.AudioPlayer;
using HipMobileUI.Contracts;
using HipMobileUI.Location;
using HipMobileUI.Navigation;
using HipMobileUI.Pages;
using HockeyApp.Android;
using Plugin.Permissions;
using Xamarin.Forms;

namespace de.upb.hip.mobile.droid
{
    [Activity(Label = "Historisches Paderborn", Icon = "@drawable/ic_launcher", Theme = "@style/splashscreen", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            IoCManager.RegisterType<IImageDimension, AndroidImageDimensions>();

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            SetTheme(Resource.Style.MainTheme);
            base.OnCreate(bundle);

            // Init Navigation
            NavigationService.Instance.RegisterViewModels(typeof(MainPage).Assembly);
            IoCManager.RegisterInstance(typeof(INavigationService), NavigationService.Instance);
            IoCManager.RegisterInstance(typeof(IViewCreator), NavigationService.Instance);

            // init other inversion of control classes
            IoCManager.RegisterInstance (typeof(IFabSizeCalculator), new AndroidFabSizeCalculator ());
            IoCManager.RegisterInstance (typeof(IAudioPlayer), new DroidAudioPlayer ());
            IoCManager.RegisterInstance (typeof(IStatusBarController), new DroidStatusBarController ());
            IoCManager.RegisterInstance (typeof(ILocationManager), new LocationManager ());
            IoCManager.RegisterInstance (typeof(IKeyProvider), new AndroidKeyProvider ());

            // setup crash reporting
            IKeyProvider keyProvider = IoCManager.Resolve<IKeyProvider>();
            CrashManager.Register(this, keyProvider.GetKeyByName("hockeyapp.android"));

            // init forms and third party libraries
            CachedImageRenderer.Init ();
            Xamarin.Forms.Forms.Init(this, bundle);
            Xamarin.FormsMaps.Init(this, bundle);

            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        /// <summary>
        /// Workaround for a IllegalStateException in Xamarin Forms 2.3. It should be fixed in 2.4 so this can be removed then.
        /// Further discussion: https://forums.xamarin.com/discussion/83864/java-lang-illegalstateexception-activity-has-been-destroyed-when-using-admob
        /// </summary>
        protected override void OnDestroy ()
        {
            Xamarin.Forms.Application.Current.MainPage = new ContentPage();
            base.OnDestroy ();
        }

    }
}

