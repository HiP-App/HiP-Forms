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

using System.IO;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.OS;
using FFImageLoading.Forms.Droid;
using HockeyApp.Android;
using PaderbornUniversity.SILab.Hip.Mobile.Droid.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.UI.AudioPlayer;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Location;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels;
using Plugin.Permissions;
using Xamarin.Forms;
using App = PaderbornUniversity.SILab.Hip.Mobile.UI.App;
using MainPage = PaderbornUniversity.SILab.Hip.Mobile.UI.Pages.MainPage;
using Acr.UserDialogs;
using Android.Content;
using CarouselView.FormsPlugin.Android;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.NotificationPlayer;
using PaderbornUniversity.SILab.Hip.Mobile.UI.DesignTime;
using TwinTechsForms.NControl.Android;

namespace PaderbornUniversity.SILab.Hip.Mobile.Droid
{
    [Activity(Label = "Historisches Paderborn", Icon = "@drawable/ic_launcher", Theme = "@style/splashscreen", MainLauncher = true,
        LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        // Field, property, and method for Image Picker
        public static readonly int PickImageId = 1000;
        public static MainActivity Instance { get; private set; }
        public TaskCompletionSource<Stream> PickImageTaskCompletionSource { set; get; }

        public MainActivity()
        {
            Instance = this;
        }
        

        protected override void OnCreate(Bundle bundle)
        {
            var dataAccess = IoCManager.Resolve<IDataAccess>();

            if (Settings.ShouldDeleteDbOnLaunch)
            {
                File.Delete(dataAccess.DatabasePath);
                Settings.ShouldDeleteDbOnLaunch = false;
            }

            dataAccess.CreateDatabase(0); // ensures the database exists and is up to date

            IoCManager.RegisterType<IImageDimension, AndroidImageDimensions>();
            IoCManager.RegisterType<IAppCloser, AndroidAppCloser>();

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            SetTheme(Resource.Style.MainTheme);
            Window.SetStatusBarColor(Android.Graphics.Color.Black);
            base.OnCreate(bundle);

            // Init Navigation
            NavigationService.Instance.RegisterViewModels(typeof(MainPage).Assembly);
            IoCManager.RegisterInstance(typeof(INavigationService), NavigationService.Instance);
            IoCManager.RegisterInstance(typeof(IViewCreator), NavigationService.Instance);

            // init other inversion of control classes
            IoCManager.RegisterInstance(typeof(IFabSizeCalculator), new AndroidFabSizeCalculator());
            IoCManager.RegisterType<IAudioPlayer, DroidAudioPlayer>();
            IoCManager.RegisterInstance(typeof(INotificationPlayer), new DroidNotificationPlayer());
            IoCManager.RegisterInstance(typeof(ILocationManager), new LocationManager());
            IoCManager.RegisterInstance(typeof(IKeyProvider), new AndroidKeyProvider());
            IoCManager.RegisterInstance(typeof(IBarsColorsChanger), new DroidBarsColorsChanger(this));
            IoCManager.RegisterInstance(typeof(IDbChangedHandler), new DbChangedHandler());
            IoCManager.RegisterInstance(typeof(INetworkAccessChecker), new DroidNetworkAccessChecker());
            IoCManager.RegisterInstance(typeof(IStorageSizeProvider), new DroidStorageSizeProvider());

            // setup crash reporting
            IKeyProvider keyProvider = IoCManager.Resolve<IKeyProvider>();
            CrashManager.Register(this, keyProvider.GetKeyByName("hockeyapp.android"));

            // init forms and third party libraries
            CachedImageRenderer.Init(enableFastRenderer: true);
            Forms.Init(this, bundle);
            SvgImageViewRenderer.Init();
            CarouselViewRenderer.Init();

            UserDialogs.Init(() => (Activity) Forms.Context);

            DesignMode.IsEnabled = false;
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
        protected override void OnDestroy()
        {
            CallOnDisappearingForAllOpenPages();

            Xamarin.Forms.Application.Current.MainPage = new ContentPage();
            base.OnDestroy();
        }

        private void CallOnDisappearingForAllOpenPages()
        {
            var tabController = Xamarin.Forms.Application.Current.MainPage as TabbedPage;
            var masterController = Xamarin.Forms.Application.Current.MainPage as MasterDetailPage;

            // First check to see if we're on a tabbed page, then master detail, finally go to overall fallback
            var nav = tabController?.CurrentPage?.Navigation ??
                      (masterController?.Detail as TabbedPage)?.CurrentPage?.Navigation ?? // special consideration for a tabbed page inside master/detail
                      masterController?.Detail?.Navigation ??
                      Xamarin.Forms.Application.Current.MainPage.Navigation;

            foreach (var page in nav.NavigationStack)
            {
                var navigationViewModel = page.BindingContext as NavigationViewModel;
                navigationViewModel?.OnDisappearing();
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent intent)
        {
            base.OnActivityResult(requestCode, resultCode, intent);

            if (requestCode == PickImageId)
            {
                if ((resultCode == Result.Ok) && (intent != null))
                {
                    Android.Net.Uri uri = intent.Data;
                    Stream stream = ContentResolver.OpenInputStream(uri);

                    // Set the Stream as the completion of the Task
                    PickImageTaskCompletionSource.SetResult(stream);
                }
                else
                {
                    PickImageTaskCompletionSource.SetResult(null);
                }
            }
        }
    }
}