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

using Acr.UserDialogs;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Input;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages
{
    public class ExhibitRouteDownloadPageViewModel : NavigationViewModel, IProgressListener
    {
        private readonly IDownloadable downloadable;
        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly IDownloadableListItemViewModel downloadableListItemViewModel;

        private bool downloadPending;
        private double loadingProgress;
        private double maximumProgress;
        private double currentProgress;

        public ExhibitRouteDownloadPageViewModel(IDownloadable downloadable, IDownloadableListItemViewModel downloadableListItemViewModel)
        {
            this.downloadable = downloadable;

            var data = downloadable.Image?.GetDataBlocking();
            if (data != null)
                Image = ImageSource.FromStream(() => new MemoryStream(data));

            this.downloadableListItemViewModel = downloadableListItemViewModel;

            CancelCommand = new Command(CancelDownload);
            GoToDetailsCommand = new Command(GoToDetails);
            GoToOverviewCommand = new Command(CloseDownloadPage);
            StartDownload = new Command(DownloadData);

            cancellationTokenSource = new CancellationTokenSource();

            DownloadPending = true;
        }

        public string DownloadableName => downloadable.Name;

        public string DownloadableId => downloadable.Id;

        public int DownloadableIdForRestApi => downloadable.IdForRestApi;

        public string DownloadableDescription => downloadable.Description;

        public string Message =>
            Strings.DownloadDetails_Text_Part1 + 
            downloadable.Name + 
            Strings.DownloadDetails_Text_Part2;

        public ImageSource Image { get; }

        public double LoadingProgress
        {
            get => loadingProgress;
            set => SetProperty(ref loadingProgress, value);
        }

        public bool DownloadFinished => !DownloadPending;

        public bool DownloadPending
        {
            get => downloadPending;
            set
            {
                if (SetProperty(ref downloadPending, value))
                    OnPropertyChanged(nameof(DownloadFinished));
            }
        }

        public ICommand StartDownload { get; }
        public ICommand CancelCommand { get; }
        public ICommand GoToOverviewCommand { get; }
        public ICommand GoToDetailsCommand { get; }

        private void CancelDownload()
        {
            cancellationTokenSource?.Cancel();
            CloseDownloadPage();
        }

        private void CloseDownloadPage()
        {
            downloadableListItemViewModel.CloseDownloadPage();
        }

        private void GoToDetails()
        {
            downloadableListItemViewModel.OpenDetailsView(DownloadableId);
        }

        private async void DownloadData()
        {
            string messageToShow = null;
            string titleToShow = null;
            var isDownloadAllowed = true;
            var networkAccessStatus = IoCManager.Resolve<INetworkAccessChecker>().GetNetworkAccessStatus();

            var fullDownloadableDataFetcher = (downloadable.Type == DownloadableType.Exhibit)
                ? IoCManager.Resolve<IFullExhibitDataFetcher>()
                : IoCManager.Resolve<IFullRouteDataFetcher>() as IFullDownloadableDataFetcher;

            if (networkAccessStatus != NetworkAccessStatus.NoAccess)
            {
                if (networkAccessStatus == NetworkAccessStatus.MobileAccess && Settings.WifiOnly)
                {
                    isDownloadAllowed = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig()
                    {
                        Title = Strings.ExhibitRouteDownloadPageViewModel_Wifi_Only_Title,
                        Message = Strings.ExhibitRouteDownloadPageViewModel_Wifi_Only_Message,
                        OkText = Strings.ExhibitRouteDownloadPageViewModel_Wifi_Only_Ok,
                        CancelText = Strings.ExhibitRouteDownloadPageViewModel_Wifi_Only_Cancel
                    });
                }

                if (isDownloadAllowed)
                {
                    try
                    {
                        await fullDownloadableDataFetcher.FetchFullDownloadableDataIntoDatabase(DownloadableId, DownloadableIdForRestApi, cancellationTokenSource.Token, this);
                    }
                    catch (Exception e)
                    {
                        if (!cancellationTokenSource.IsCancellationRequested)
                        {
                            titleToShow = Strings.LoadingPageViewModel_BaseData_DownloadFailed_Title;
                            messageToShow = Strings.LoadingPageViewModel_BaseData_DownloadFailed_Text;
                            Debug.WriteLine(e.Message);
                            Debug.WriteLine(e.StackTrace);
                        }
                    }
                }
                else
                {
                    CloseDownloadPage();
                }
            }
            else
            {
                titleToShow = Strings.LoadingPageViewModel_BaseData_DownloadFailed_Title;
                messageToShow = Strings.LoadingPageViewModel_BaseData_DownloadFailed_Text;
                if (networkAccessStatus == NetworkAccessStatus.MobileAccess)
                {
                    messageToShow += Environment.NewLine + Strings.LoadingPageViewModel_BaseData_DownloadViaMobile;
                }
            }

            if (messageToShow != null)
            {
                await Navigation.DisplayAlert(titleToShow, messageToShow, "OK");
                CloseDownloadPage();
                return;
            }

            if (!cancellationTokenSource.IsCancellationRequested && isDownloadAllowed)
            {
                SetDetailsAvailable();
            }
            IoCManager.Resolve<IDbChangedHandler>().NotifyAll();
        }

        private void SetDetailsAvailable()
        {
            DownloadPending = false;
            downloadableListItemViewModel.SetDetailsAvailable(DownloadFinished);

            //Close DownloadPage directly if download was started from the AppetizerView
            if (DownloadFinished && (downloadableListItemViewModel.GetType() == typeof(AppetizerPageViewModel)))
            {
                CloseDownloadPage();
            }
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            StartDownload.Execute(null);
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
            cancellationTokenSource?.Cancel();
        }

        public void ProgressOneStep()
        {
            currentProgress++;
            LoadingProgress = currentProgress / maximumProgress;
        }

        public void SetMaxProgress(double maxProgress)
        {
            maximumProgress = maxProgress;
        }

        public void UpdateProgress(double newProgress, double maxProgress)
        {
            LoadingProgress = newProgress / maxProgress;
        }
    }
}