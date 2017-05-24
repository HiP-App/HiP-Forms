using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using Xamarin.Forms;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views.ExhibitDetails;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views
{
    class ExhibitRouteDownloadViewModel : NavigationViewModel, IProgressListener
    {
        public ExhibitRouteDownloadViewModel (IDownloadable downloadable, IDownloadableListItemViewModel downloadableListItemViewModel)
        {
            DownloadableId = downloadable.Id;
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
            set { SetProperty (ref downloadFinished, value); }
        }

        private bool downloadPending;
        public bool DownloadPending
        {
            get { return downloadPending; }
            set { SetProperty (ref downloadPending, value); }
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

        private bool downloadAborted;
        void CancelDownload ()
        {
            // Do some stuff to abort the download; this distincts this method from the one below
            downloadAborted = true;
            CloseDownloadPage ();
        }

        void CloseDownloadPage ()
        {
            DownloadableListItemViewModel.CloseDownloadPage ();
        }

        void GoToDetails ()
        {
            DownloadableListItemViewModel.OpenDetailsView (DownloadableId);
        }

        async void DownloadData ()
        {
            // This is where all the the data will be downloaded
            // maybe you do something like this:   Database.loadInterestDataFor(InterestId);    // Interests are Routes and Exhibits

            string messageToShow = null;
            string titleToShow = null;
            //var fullExhibitDataFetcher = IoCManager.Resolve<IFullExhibitDataFetcher>(); // TODO: Implementation
            var networkAccessStatus = IoCManager.Resolve<INetworkAccessChecker>().GetNetworkAccessStatus();

            if (networkAccessStatus == NetworkAccessStatus.WifiAccess
                || (networkAccessStatus == NetworkAccessStatus.MobileAccess && !Settings.WifiOnly))
            {
                try
                {
                    var token = cancellationTokenSource.Token;
                    //await fullExhibitDataFetcher.FetchFullExhibitDataIntoDatabase(token, this); // TODO: Implementation
                }
                catch (Exception e)
                {
                    titleToShow = Strings.LoadingPageViewModel_BaseData_DownloadFailed_Title;
                    messageToShow = Strings.LoadingPageViewModel_BaseData_DownloadFailed_Text;
                    Debug.WriteLine(e.Message);
                    Debug.WriteLine(e.StackTrace);
                }
            }
            else
            {
                titleToShow = Strings.LoadingPageViewModel_BaseData_DownloadFailed_Title;
                messageToShow = Strings.LoadingPageViewModel_BaseData_DownloadFailed_Text;
                if (networkAccessStatus == NetworkAccessStatus.MobileAccess)
                {
                    messageToShow += Environment.NewLine + Strings.LoadingPageViewModel_BaseData_OnlyMobile;
                }
            }

            if (messageToShow != null)
            {
                await Navigation.DisplayAlert(titleToShow, messageToShow, "OK");
            }


            // TODO: Bind LoadingProgress with download
            LoadingProgress = 0;
            for (var x = 0; x < 50; x++)
            {
                UpdateProgress (LoadingProgress+.02, 1);
                await Task.Delay (50);
                if (downloadAborted)
                    return;
            }
            SetDetailsAvailable ();
        }

        void SetDetailsAvailable ()
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

        public void UpdateProgress (double newProgress, double maxProgress)
        {
            LoadingProgress = newProgress / maxProgress;
        }

        public void ProgressOneStep ()
        {
            throw new NotImplementedException ();
        }

        public void SetMaxProgress (double maxProgress)
        {
            throw new NotImplementedException ();
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            StartDownload.Execute(null);
        }

        public override void OnDisappearing ()
        {
            base.OnDisappearing ();
            downloadAborted = true;
        }
    }
}
