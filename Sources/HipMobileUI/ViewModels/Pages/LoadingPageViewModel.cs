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

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentHandling;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.DtoToModelConverters;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.FeatureToggling;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.UserManagement;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.AuthenticationApiAccess;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.FeatureToggleApiAccess;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Appearance;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Location;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages
{
    class LoadingPageViewModel : NavigationViewModel, IProgressListener
    {
        public LoadingPageViewModel()
        {
            // This lookup NOT required for Windows platforms - the Culture will be automatically set
            if (Device.RuntimePlatform == Device.iOS || Device.RuntimePlatform == Device.Android)
            {
                // determine the correct, supported .NET culture
                var ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
                Strings.Culture = ci; // set the RESX for resource localization
                DependencyService.Get<ILocalize>().SetLocale(ci); // set the Thread for locale-aware methods
            }

            Text = Strings.LoadingPage_Text;
            Subtext = Strings.LoadingPage_Subtext;
            StartLoading = new Command(Load);
            CancelCommand = new Command(CancelLoading);
            cancellationTokenSource = new CancellationTokenSource();

            // listen to sleep and wake up messages as the main screen cannot be started when the app is sleeping
            MessagingCenter.Subscribe<App>(this, AppSharedData.WillSleepMessage, WillSleep);
            MessagingCenter.Subscribe<App>(this, AppSharedData.WillWakeUpMessage, WillWakeUp);
        }

        private string text;
        private string subtext;
        private ICommand startLoading;
        private ICommand cancel;
        private bool isExtendedViewsVisible;
        private double loadingProgress;
        private readonly CancellationTokenSource cancellationTokenSource;

        private bool isSleeping;
        private bool isDatabaseUpToDate = true;
        private string errorMessage;
        private string errorTitle = "";

        private Action actionOnUiThread;

        /// <summary>
        /// The headline text.
        /// </summary>
        public string Text
        {
            get { return text; }
            set { SetProperty(ref text, value); }
        }

        /// <summary>
        /// the sub text.
        /// </summary>
        public string Subtext
        {
            get { return subtext; }
            set { SetProperty(ref subtext, value); }
        }

        /// <summary>
        /// The command for starting loading of database.
        /// </summary>
        public ICommand StartLoading
        {
            get { return startLoading; }
            set { SetProperty(ref startLoading, value); }
        }

        /// <summary>
        /// The command for canceling loading of database.
        /// </summary>
        public ICommand CancelCommand
        {
            get { return cancel; }
            set { SetProperty(ref cancel, value); }
        }

        /// <summary>
        /// Indicates wether the extended view is visible or not. The extended view gives more information about the current progress.
        /// </summary>
        public bool IsExtendedViewsVisible
        {
            get { return isExtendedViewsVisible; }
            set { SetProperty(ref isExtendedViewsVisible, value); }
        }

        /// <summary>
        /// Value indicating the current loading progress.
        /// </summary>
        public double LoadingProgress
        {
            get { return loadingProgress; }
            set { SetProperty(ref loadingProgress, value); }
        }

        private IBaseDataFetcher baseDataFetcher;

        public async void Load()
        {
            try
            {
                await InitIoCContainerAsync();
                await BackupData.Init();

                baseDataFetcher = IoCManager.Resolve<IBaseDataFetcher>();

                var networkAccessStatus = IoCManager.Resolve<INetworkAccessChecker>().GetNetworkAccessStatus();

                if (networkAccessStatus != NetworkAccessStatus.NoAccess)
                {
                    await CheckForUpdatedDatabase();
                }
                else
                {
                    errorTitle = Strings.LoadingPageViewModel_BaseData_DownloadFailed_Title;
                    errorMessage = Strings.LoadingPageViewModel_BaseData_DatabaseUpToDateCheckFailed;
                }

                if (!isDatabaseUpToDate)
                {
                    if (networkAccessStatus == NetworkAccessStatus.MobileAccess && Settings.WifiOnly)
                    {
                        actionOnUiThread = AskUserDownloadDataViaMobile;
                        // if the app is not sleeping ask the user whether to download via mobile otherwise wait for wake up
                        if (!isSleeping)
                        {
                            Device.BeginInvokeOnMainThread(actionOnUiThread);
                        }

                        return;
                    }

                    await UpdateDatabase();
                }

                await AchievementManager.UpdateServerAndLocalState();
            }
            catch (Exception e)
            {
                // Catch all exceptions happening on startup cause otherwise the loading page will be shown indefinitely 
                // This should only happen during development
                errorMessage = null;
                errorTitle = null;
                Debug.WriteLine(e);
            }

            LoadCacheAndStart();
        }

        private async void AskUserDownloadDataViaMobile()
        {
            actionOnUiThread = null;
            var downloadData = await Navigation.DisplayAlert(Strings.LoadingPageViewModel_BaseData_DataAvailable,
                                                             Strings.LoadingPageViewModel_BaseData_DownloadViaMobile,
                                                             Strings.LoadingPageViewModel_BaseData_MobileDownload_Confirm,
                                                             Strings.LoadingPageViewModel_BaseData_MobileDownload_Cancel);

            try
            {
                if (downloadData)
                {
                    await UpdateDatabase();
                }

                LoadCacheAndStart();
            }
            catch (Exception e)
            {
                // Catch all exceptions happening on startup cause otherwise the loading page will be shown indefinitely 
                // This should only happen during development
                errorMessage = null;
                errorTitle = null;
                Debug.WriteLine(e);
            }
        }

        private async Task CheckForUpdatedDatabase()
        {
            try
            {
                isDatabaseUpToDate = await baseDataFetcher.IsDatabaseUpToDate();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);

                errorTitle = Strings.LoadingPageViewModel_BaseData_DownloadFailed_Title;
                errorMessage = Strings.LoadingPageViewModel_BaseData_DatabaseUpToDateCheckFailed;
            }
        }

        private async Task UpdateDatabase()
        {
            IsExtendedViewsVisible = true;
            try
            {
                await baseDataFetcher.FetchBaseDataIntoDatabase(cancellationTokenSource.Token, this);
            }
            catch (Exception e)
            {
                errorTitle = Strings.LoadingPageViewModel_BaseData_DownloadFailed_Title;
                errorMessage = Strings.LoadingPageViewModel_BaseData_DownloadFailed_Text;
                Debug.WriteLine(e);
            }
        }

        private async void LoadCacheAndStart()
        {
            try
            {
                // force the db to load the exhibitset into cache
                ExhibitManager.GetExhibitSets();
                LoadingProgress = 0.9;
                await Task.Delay(100);
            }
            catch (Exception e)
            {
                // Catch all exceptions happening on startup cause otherwise the loading page will be shown indefinitely 
                // This should only happen during development
                errorMessage = null;
                errorTitle = null;
                Debug.WriteLine(e);
            }

            actionOnUiThread = async () =>
            {
                if (errorMessage != null && errorTitle != null)
                {
                    await Navigation.DisplayAlert(errorTitle, errorMessage, Strings.LoadingPageViewModel_LoadingError_Confirm);
                }

                StartMainApplication();
            };
            // if the app is not sleeping open the main menu, otherwise wait for it to wake up
            if (!isSleeping)
            {
                Device.BeginInvokeOnMainThread(actionOnUiThread);
            }
        }

        private async void StartMainApplication()
        {
            Settings.InitialThemeSelected = true;

            var vm = new MainPageViewModel();
            LoadingProgress = 1;
            await Task.Delay(100);

            MessagingCenter.Unsubscribe<App>(this, AppSharedData.WillSleepMessage);
            MessagingCenter.Unsubscribe<App>(this, AppSharedData.WillWakeUpMessage);
            Navigation.StartNewNavigationStack(vm);
        }

        private async Task InitIoCContainerAsync()
        {
            IoCManager.RegisterType<IDataLoader, EmbeddedResourceDataLoader>();
            IoCManager.RegisterInstance(typeof(ApplicationResourcesProvider), new ApplicationResourcesProvider(Application.Current.Resources));

            //init serviceaccesslayer
            IoCManager.RegisterInstance(typeof(IContentApiClient), new ContentApiClient());
            IoCManager.RegisterInstance(typeof(IAchievementsApiAccess), new AchievementsApiAccess(new ContentApiClient(ServerEndpoints.AchievementsApiPath)));
            IoCManager.RegisterInstance(typeof(IFeatureToggleApiAccess), new FeatureToggleApiAccess(new ContentApiClient(ServerEndpoints.FeatureTogglesApiPath)));
            IoCManager.RegisterType<IExhibitsApiAccess, ExhibitsApiAccess>();
            IoCManager.RegisterType<IMediasApiAccess, MediasApiAccess>();
            IoCManager.RegisterType<IFileApiAccess, FileApiAccess>();
            IoCManager.RegisterType<IPagesApiAccess, PagesApiAccess>();
            IoCManager.RegisterType<IRoutesApiAccess, RoutesApiAccess>();
            IoCManager.RegisterType<ITagsApiAccess, TagsApiAccess>();

            //init converters
            IoCManager.RegisterType<ExhibitConverter>();
            IoCManager.RegisterType<MediaToAudioConverter>();
            IoCManager.RegisterType<MediaToImageConverter>();
            IoCManager.RegisterType<PageConverter>();
            IoCManager.RegisterType<RouteConverter>();
            IoCManager.RegisterType<TagConverter>();

            //init fetchers
            IoCManager.RegisterInstance(typeof(INewDataCenter), new NewDataCenter());
            IoCManager.RegisterType<IMediaDataFetcher, MediaDataFetcher>();
            IoCManager.RegisterType<IDataToRemoveFetcher, DataToRemoveFetcher>();
            IoCManager.RegisterType<IExhibitsBaseDataFetcher, ExhibitsBaseDataFetcher>();
            IoCManager.RegisterType<IFullExhibitDataFetcher, FullExhibitDataFetcher>();
            IoCManager.RegisterType<IRoutesBaseDataFetcher, RoutesBaseDataFetcher>();
            IoCManager.RegisterType<IBaseDataFetcher, BaseDataFetcher>();
            IoCManager.RegisterType<IFullRouteDataFetcher, FullRouteDataFetcher>();
            IoCManager.RegisterType<IAchievementFetcher, AchievementFetcher>();

            IoCManager.RegisterInstance(typeof(INearbyExhibitManager), new NearbyExhibitManager());
            IoCManager.RegisterInstance(typeof(INearbyRouteManager), new NearbyRouteManager());

            IoCManager.RegisterType<IAuthApiAccess, AuthApiAccess>();
            IoCManager.RegisterInstance(typeof(IUserRatingApiAccess), new UserRatingApiAccess(new ContentApiClient()));
            IoCManager.RegisterInstance(typeof(IUserManager), new UserManager());
            IoCManager.RegisterInstance(typeof(IUserRatingManager), new UserRatingManager());
            IoCManager.RegisterInstance(typeof(IThemeManager), new ThemeManager());
            IoCManager.RegisterInstance(typeof(AchievementNotificationViewModel), new AchievementNotificationViewModel());
            
            var featureToggleRouter = await FeatureToggleRouter.CreateAsync();
            IoCManager.RegisterInstance(typeof(IFeatureToggleRouter), featureToggleRouter);
        }

        /// <summary>
        /// React to progress updates.
        /// </summary>
        /// <param name="newProgress">The new progress value.</param>
        /// <param name="maxProgress">The maximum propgress value.</param>
        public void UpdateProgress(double newProgress, double maxProgress)
        {
            LoadingProgress = (newProgress / maxProgress) * 0.8;
        }

        private double maximumProgress;
        private double currentProgress;

        public void ProgressOneStep()
        {
            currentProgress++;
            LoadingProgress = (currentProgress / maximumProgress) * 0.8;
        }

        public void SetMaxProgress(double maxProgress)
        {
            maximumProgress = maxProgress;
        }

        private void WillWakeUp(App obj)
        {
            isSleeping = false;

            // app was send to sleep before the main menu could be opened, open the menu now
            if (actionOnUiThread != null)
            {
                Device.BeginInvokeOnMainThread(actionOnUiThread);
            }
        }

        private void WillSleep(App obj)
        {
            isSleeping = true;
        }

        private void CancelLoading()
        {
            cancellationTokenSource?.Cancel();
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
            cancellationTokenSource.Cancel();
        }
    }
}