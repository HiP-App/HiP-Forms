using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Location;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views
{
    class ExhibitRouteDownloadViewModel : NavigationViewModel, IProgressListener
    {
        public ExhibitRouteDownloadViewModel (Exhibit exhibit, ExhibitsOverviewListItemViewModel exhibitsOverviewListItemViewModel)
        {
            InterestTitle = exhibit.Name;
            InterestId = exhibit.Id;
            
            Message = "Lade Daten";    // Modify this message if generic type available; pass in constructor or generate here with Strings.xxx
            var data = exhibit.Image.Data;
            Image = ImageSource.FromStream(() => new MemoryStream(data));
            ExRoListItemViewModel = exhibitsOverviewListItemViewModel;

            // The commands for the buttons
            CancelCommand = new Command(CancelDownload);
            GoToDetailsCommand = new Command(GoToDetails);
            GoToOverviewCommand = new Command(CloseDownloadPage);

            StartDownload = new Command(DownloadData);

            DownloadPending = true;
            DownloadFinished = false;   // Since false is the default value this is just a reminder in case the database wants to set this to true when generating this item
        }
        public ExhibitRouteDownloadViewModel(Route route, RoutesOverviewListItemViewModel routesOverviewListItemViewModel)
        {
            InterestTitle = route.Title;
            InterestId = route.Id;
            
            Message = "Lade Daten";    // Modify this message if generic type available
            var data = route.Image.Data;
            Image = ImageSource.FromStream(() => new MemoryStream(data));
            ExRoListItemViewModel = routesOverviewListItemViewModel;
            
            // The commands for the buttons
            CancelCommand = new Command (CancelDownload);
            GoToDetailsCommand = new Command (GoToDetails);
            GoToOverviewCommand = new Command (CloseDownloadPage);

            StartDownload = new Command (DownloadData);

            DownloadPending = true;
            DownloadFinished = false;
        }

        private IExRoListItemViewModel ExRoListItemViewModel { get; set; }

        private string interestTitle;
        public string InterestTitle
        {
            get { return interestTitle; }
            set { SetProperty(ref interestTitle, value); }
        }

        private string interestId;
        public string InterestId {
            get { return interestId; }
            set { SetProperty(ref interestId, value); }
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
            ExRoListItemViewModel.CloseDownloadPage ();
        }

        void GoToDetails ()
        {
            CloseDownloadPage ();
            ExRoListItemViewModel.OpenDetailsView (InterestId);
        }

        async void DownloadData ()
        {
            // This is where all the the data will be downloaded
            // This method is called within OnAppearing() of the ExhibitRouteDownloadViewModel-page
            // maybe you do something like this:   Database.loadInterestDataFor(InterestId);    // Interests are Routes and Exhibits
            
            Debug.WriteLine ("##### Starting Download #####");
            LoadingProgress = 0;
            for (var x = 0; x < 100; x++)
            {
                UpdateProgress (LoadingProgress+.01, 1);
                await Task.Delay (50);
                if (downloadAborted)
                    return;     // Shouldn't this break the whole function instead of only the loop?
            }

            // At the end: Set parameters for hiding/displaying the buttons
            if (!downloadAborted)
                SetDetailsAvailable ();
        }

        void SetDetailsAvailable ()
        {
            DownloadPending = false;
            DownloadFinished = !DownloadPending;
            ExRoListItemViewModel.SetDetailsAvailable (DownloadFinished);
        }

        public void UpdateProgress (double newProgress, double maxProgress)
        {
            LoadingProgress = newProgress / maxProgress;
        }

    }
}
