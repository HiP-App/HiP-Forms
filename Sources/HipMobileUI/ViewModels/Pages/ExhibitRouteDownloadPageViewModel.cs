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
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;
using Xamarin.Forms;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views.ExhibitDetails;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages
{
    class ExhibitRouteDownloadViewModel : NavigationViewModel, IProgressListener
    {

        private IDownloadable downloadable;
        public ExhibitRouteDownloadViewModel(IDownloadable downloadable, IDownloadableListItemViewModel downloadableListItemViewModel)
        {
            this.downloadable = downloadable;
            DownloadableId = downloadable.Id;
            DownloadableIdForRestApi = downloadable.IdForRestApi;
            DownloadableName = downloadable.Name;
            DownloadableDescription = downloadable.Description;

            Message = Strings.DownloadDetails_Text_Part1 + DownloadableName + Strings.DownloadDetails_Text_Part2;
            var data = downloadable.Image.Data;
            Image = ImageSource.FromStream(() => new MemoryStream(data));
            DownloadableListItemViewModel = downloadableListItemViewModel;

            CancelCommand = new Command(CancelDownload);
            GoToDetailsCommand = new Command(GoToDetails);
            GoToOverviewCommand = new Command(CloseDownloadPage);
            StartDownload = new Command(DownloadData);

            cancellationTokenSource = new CancellationTokenSource();

            DownloadPending = true;
            DownloadFinished = !DownloadPending;   // Since false is the default value this is just a reminder in case the database wants to set this to true when generating this item
        }

        private readonly CancellationTokenSource cancellationTokenSource;

        private IDownloadableListItemViewModel DownloadableListItemViewModel { get; set; }

        private string downloadableName;
        public string DownloadableName
        {
            get { return downloadableName; }
            set { SetProperty(ref downloadableName, value); }
        }

        private string downloadableId;
        public string DownloadableId
        {
            get { return downloadableId; }
            set { SetProperty(ref downloadableId, value); }
        }

        private int downloadableIdForRestApi;
        public int DownloadableIdForRestApi
        {
            get { return downloadableIdForRestApi; }
            set { SetProperty(ref downloadableIdForRestApi, value); }
        }

        private string downloadableDescription;
        public string DownloadableDescription
        {
            get { return downloadableDescription; }
            set { SetProperty(ref downloadableDescription, value); }
        }

        private string message;
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        private ImageSource image;
        public ImageSource Image
        {
            get { return image; }
            set { SetProperty(ref image, value); }
        }

        private double loadingProgress;
        public double LoadingProgress
        {
            get { return loadingProgress; }
            set { SetProperty(ref loadingProgress, value); }
        }

        private bool downloadFinished;
        public bool DownloadFinished
        {
            get { return downloadFinished; }
            set { SetProperty(ref downloadFinished, value); }
        }

        private bool downloadPending;
        public bool DownloadPending
        {
            get { return downloadPending; }
            set { SetProperty(ref downloadPending, value); }
        }

        private ICommand startDownload;
        public ICommand StartDownload
        {
            get { return startDownload; }
            set { SetProperty(ref startDownload, value); }
        }
        public ICommand CancelCommand { get; }
        public ICommand GoToOverviewCommand { get; }
        public ICommand GoToDetailsCommand { get; }

        void CancelDownload ()
        {
            cancellationTokenSource?.Cancel();
            CloseDownloadPage ();
        }

        void CloseDownloadPage()
        {
            DownloadableListItemViewModel.CloseDownloadPage();
        }

        void GoToDetails()
        {
            DownloadableListItemViewModel.OpenDetailsView(DownloadableId);
        }

        async void DownloadData()
        {
            if (downloadable.GetType() == typeof(Exhibit))
            {
                // We want to download an Exhibit
                string messageToShow = null;
                string titleToShow = null;
                var fullExhibitDataFetcher = IoCManager.Resolve<IFullExhibitDataFetcher>();
                var networkAccessStatus = IoCManager.Resolve<INetworkAccessChecker>().GetNetworkAccessStatus();

                if (networkAccessStatus != NetworkAccessStatus.NoAccess)
                {
                    try
                    {
                        await fullExhibitDataFetcher.FetchFullExhibitDataIntoDatabase(DownloadableId, DownloadableIdForRestApi, cancellationTokenSource.Token, this);
<<<<<<< HEAD
<<<<<<< HEAD
                        if (!cancellationTokenSource.IsCancellationRequested)
                        {
                            SetDetailsAvailable();
                        }
=======
>>>>>>> master
=======
>>>>>>> 8c1cc897a51cf1345bb30b19b5829571b89467e3
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
                }
            }
            else
            {
                if (downloadable.GetType() == typeof(Route))
                {
                    // We want to download a Route
                    // Load audio for description
                    // Load all to this route related exhibits
                }
                else
                {
                    return; // This case should never occure since we only have exhibits and routes that can create the page related to this viewmodel
                }
            }

            // This can be called from the class that actually handles the download of the data
            // Move the updates up to where the actual download happens

<<<<<<< HEAD
<<<<<<< HEAD
            SetDetailsAvailable();
=======
=======
>>>>>>> 8c1cc897a51cf1345bb30b19b5829571b89467e3
            if (!cancellationTokenSource.IsCancellationRequested)
            {
                SetDetailsAvailable();
                IoCManager.Resolve<IDbChangedHandler>().NotifyAll();
            }
<<<<<<< HEAD
>>>>>>> master
=======
>>>>>>> 8c1cc897a51cf1345bb30b19b5829571b89467e3
        }

        void SetDetailsAvailable()
        {
            DownloadPending = false;
            DownloadFinished = !DownloadPending;
            DownloadableListItemViewModel.SetDetailsAvailable (DownloadFinished);

            //Close DownloadPage directly if download was started from the AppetizerView
            if(DownloadFinished && (DownloadableListItemViewModel.GetType() == typeof(AppetizerViewModel)))
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
            base.OnDisappearing ();
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