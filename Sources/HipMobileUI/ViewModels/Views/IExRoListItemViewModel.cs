namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views
{
    public interface IExRoListItemViewModel {

        void CloseDownloadPage ();
        void OpenDetailsView (string id);
        void SetDetailsAvailable (bool available);
    }
}
