﻿// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
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
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.DtoToModelConverters;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataLayer;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Location;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages
{
    class LoadingPageViewModel : NavigationViewModel, IProgressListener
    {

        public LoadingPageViewModel()
        {
            Text = Strings.LoadingPage_Text;
            Subtext = Strings.LoadingPage_Subtext;
            StartLoading = new Command(Load);
            cancellationTokenSource = new CancellationTokenSource();

            // listen to sleep and wake up messages as the main screen cannot be started when the app is sleeping
            MessagingCenter.Subscribe<App>(this, AppSharedData.WillSleepMessage, WillSleep);
            MessagingCenter.Subscribe<App>(this, AppSharedData.WillWakeUpMessage, WillWakeUp);
        }

        private string text;
        private string subtext;
        private ICommand startLoading;
        private bool isExtendedViewsVisible;
        private double loadingProgress;
        private readonly CancellationTokenSource cancellationTokenSource;

        private bool isSleeping;
        private Action startupAction;

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

        public void Load()
        {
            Task.Factory.StartNew(async () =>
            {
                string messageToShowOnStartup = null;
                string titleToShowOnStartup = null;

                try
                {
                    IoCManager.RegisterType<IDataAccess, RealmDataAccess>();
                    IoCManager.RegisterType<IDataLoader, EmbeddedResourceDataLoader>();
                    IoCManager.RegisterInstance(typeof(ApplicationResourcesProvider), new ApplicationResourcesProvider(Application.Current.Resources));

                    //init serviceaccesslayer
                    IoCManager.RegisterType<IContentApiClient, ContentApiClient>();
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
                    IoCManager.RegisterType<IMediaDataFetcher, MediaDataFetcher>();
                    IoCManager.RegisterType<IDataToRemoveFetcher, DataToRemoveFetcher>();
                    IoCManager.RegisterType<IExhibitsBaseDataFetcher, ExhibitsBaseDataFetcher>();
                    IoCManager.RegisterType<IFullExhibitDataFetcher, FullExhibitDataFetcher> ();
                    IoCManager.RegisterType<IRoutesBaseDataFetcher, RoutesBaseDataFetcher>();
                    IoCManager.RegisterType<IBaseDataFetcher, BaseDataFetcher>();

                    IoCManager.RegisterInstance(typeof(INearbyExhibitManager), new NearbyExhibitManager());
                    IoCManager.RegisterInstance(typeof(INearbyRouteManager), new NearbyRouteManager());

                    var baseDataFetcher = IoCManager.Resolve<IBaseDataFetcher>();

                    var networkAccessStatus = IoCManager.Resolve<INetworkAccessChecker>().GetNetworkAccessStatus();

                    if (networkAccessStatus == NetworkAccessStatus.WifiAccess
                        || (networkAccessStatus == NetworkAccessStatus.MobileAccess && !Settings.WifiOnly))
                    {
                        try
                        {
                            var token = cancellationTokenSource.Token;
                            bool isUpToDate = await baseDataFetcher.IsDatabaseUpToDate(token);
                            if (isUpToDate)
                            {
                                IsExtendedViewsVisible = true;
                                await baseDataFetcher.FetchBaseDataIntoDatabase(token, this);
                            }
                        }
                        catch (Exception e)
                        {
                            titleToShowOnStartup = Strings.LoadingPageViewModel_BaseData_DownloadFailed_Title;
                            messageToShowOnStartup = Strings.LoadingPageViewModel_BaseData_DownloadFailed_Text;
                            Debug.WriteLine(e.Message);
                            Debug.WriteLine(e.StackTrace);
                        }
                    }
                    else
                    {
                        titleToShowOnStartup = Strings.LoadingPageViewModel_BaseData_DownloadFailed_Title;
                        messageToShowOnStartup = Strings.LoadingPageViewModel_BaseData_DownloadFailed_Text;
                        if (networkAccessStatus == NetworkAccessStatus.MobileAccess)
                        {
                            messageToShowOnStartup += Environment.NewLine + Strings.LoadingPageViewModel_BaseData_OnlyMobile;
                        }
                    }

                    // show text, progress bar and image when db is initialized, otherwise just the indicator is shown
                    if (!DbManager.IsDatabaseUpToDate())
                    {
                        IsExtendedViewsVisible = true;
                    }
                    DbManager.UpdateDatabase(this);

                    // force the db to load the exhibitset into cache
                    ExhibitManager.GetExhibitSets();
                    LoadingProgress = 0.9;
                    await Task.Delay(100);


                }
                catch (Exception e)
                {
                    // Catch all exceptions happening on startup cause otherwise the loading page will be shown indefinitely 
                    // This should only happen during development
                    messageToShowOnStartup = e.Message;
                    titleToShowOnStartup = "Error";
                }
                // if the app is not sleeping open the main menu, otherwise wait for it to wake up
                startupAction = async () =>
                {
                    if (messageToShowOnStartup != null)
                    {
                        await Navigation.DisplayAlert(titleToShowOnStartup, messageToShowOnStartup, "OK");
                    }

                    var vm = new MainPageViewModel();
                    LoadingProgress = 1;
                    await Task.Delay(100);

                    MessagingCenter.Unsubscribe<App>(this, AppSharedData.WillSleepMessage);
                    MessagingCenter.Unsubscribe<App>(this, AppSharedData.WillWakeUpMessage);
                    Navigation.StartNewNavigationStack(vm);
                };
                if (!isSleeping)
                {
                    Device.BeginInvokeOnMainThread(startupAction);
                }
            });
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
            if (startupAction != null)
            {
                Device.BeginInvokeOnMainThread(startupAction);
            }
        }

        private void WillSleep(App obj)
        {
            isSleeping = true;
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
            cancellationTokenSource.Cancel();
        }
    }
}
