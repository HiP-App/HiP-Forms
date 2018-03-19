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
using System.IO;
using System.Threading;
using System.Windows.Input;
using Acr.UserDialogs;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages
{
    public class ExhibitRouteDownloadPageViewModel : NavigationViewModel, IProgressListener
    {
        public ExhibitRouteDownloadPageViewModel(IDownloadable downloadable, IDownloadableListItemViewModel downloadableListItemViewModel)
        {
            Downloadable = downloadable;
            //Remove the description label if no description exists to center the other labels precisely
            DescriptionExists = !(DownloadableDescription == null || DownloadableDescription.Equals(""));

            SetImage();
            DownloadableListItemViewModel = downloadableListItemViewModel;

            CancelCommand = new Command(CancelDownload);
            StartDownload = new Command(DownloadData);

            cancellationTokenSource = new CancellationTokenSource();
        }

        private async void SetImage()
        {
            var imageData = await downloadable.Image.GetDataAsync();
            Image = imageData != null ? ImageSource.FromStream(() => new MemoryStream(imageData)) : ImageSource.FromStream(() => new MemoryStream(BackupData.BackupImageData));
        }

        private readonly CancellationTokenSource cancellationTokenSource;

        private IDownloadableListItemViewModel DownloadableListItemViewModel { get; set; }


        private IDownloadable downloadable;
        public IDownloadable Downloadable
        {
            get => downloadable;
            set => SetProperty(ref downloadable, value);
        }

        public string DownloadableName => Downloadable.Name;

        public string DownloadableDescription => Downloadable.Description;

        public string DownloadableId => Downloadable.Id;

        public int DownloadableIdForRestApi => Downloadable.IdForRestApi;

        private ImageSource image;
        public ImageSource Image
        {
            get => image;
            set => SetProperty(ref image, value);
        }

        private double loadingProgress;
        public double LoadingProgress
        {
            get => loadingProgress;
            set => SetProperty(ref loadingProgress, value);
        }

        private bool descriptionExists;
        public bool DescriptionExists
        {
            get => descriptionExists;
            set => SetProperty(ref descriptionExists, value);
        }

        private ICommand startDownload;
        public ICommand StartDownload
        {
            get => startDownload;
            set => SetProperty(ref startDownload, value);
        }

        public ICommand CancelCommand { get; }
        private void CancelDownload()
        {
            cancellationTokenSource?.Cancel();
            CloseDownloadPage();
        }

        private void OpenDetailsView()
        {
            DownloadableListItemViewModel.OpenDetailsView(DownloadableId);
        }

        private void CloseDownloadPage()
        {
            DownloadableListItemViewModel.CloseDownloadPage();
        }

        private async void DownloadData()
        {
            string messageToShow = null;
            string titleToShow = null;
            var isDownloadAllowed = true;
            var networkAccessStatus = IoCManager.Resolve<INetworkAccessChecker>().GetNetworkAccessStatus();
            IFullDownloadableDataFetcher fullDownloadableDataFetcher;

            if (downloadable.Type == DownloadableType.Exhibit)
                fullDownloadableDataFetcher = IoCManager.Resolve<IFullExhibitDataFetcher>();
            else
                fullDownloadableDataFetcher = IoCManager.Resolve<IFullRouteDataFetcher>();

            if (networkAccessStatus != NetworkAccessStatus.NoAccess)
            {
                if (networkAccessStatus == NetworkAccessStatus.MobileAccess && Settings.WifiOnly)
                {
                    isDownloadAllowed = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig()
                    {
                        Title = Strings.ExhibitRouteDownloadPageViewModel_Wifi_Only_Title,
                        Message = Strings.ExhibitRouteDownloadPageViewModel_Wifi_Only_Message,
                        OkText = Strings.Yes,
                        CancelText = Strings.No
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
                await Navigation.DisplayAlert(titleToShow, messageToShow, Strings.Ok);
                CloseDownloadPage();
                return;
            }

            if (!cancellationTokenSource.IsCancellationRequested && isDownloadAllowed)
            {
                DownloadableListItemViewModel.SetDetailsAvailable(true);

                if (DownloadableListItemViewModel.GetType() != typeof(AppetizerPageViewModel))
                    OpenDetailsView();
                else
                    CloseDownloadPage();
            }
            IoCManager.Resolve<IDbChangedHandler>().NotifyAll();
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

        private double maximumProgress;
        private double currentProgress;

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