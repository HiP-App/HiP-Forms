using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages
{
    class ExhibitRouteDownloadViewModel : NavigationViewModel, IProgressListener {

        private IDownloadable downloadable;
        public ExhibitRouteDownloadViewModel (IDownloadable downloadable, IDownloadableListItemViewModel downloadableListItemViewModel)
        {
            this.downloadable = downloadable;
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

            DownloadPending = true;
            DownloadFinished = !DownloadPending;   // Since false is the default value this is just a reminder in case the database wants to set this to true when generating this item
        }

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
            if (downloadable == null)
                return;
            LoadingProgress = 0;

            if (downloadable.GetType () == typeof (Exhibit))
            {
                // We want to download an Exhibit
                // This should be implemented by Rico
            }
            else
            {
                if (downloadable.GetType () == typeof (Route))
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
        }

        public void UpdateProgress (double newProgress, double maxProgress)
        {
            LoadingProgress = newProgress / maxProgress;
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
