namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models
{
    public interface IDownloadable
    {
        string Id { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        Image Image { get; set; }
    }
}
