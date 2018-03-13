using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.JoinClasses
{
    /// <summary>
    /// Represents the many-to-many relationship between
    /// <see cref="Models.Exhibit"/> and <see cref="Models.Page"/>.
    /// </summary>
    public class JoinExhibitPage : IJoinEntity<Exhibit>, IJoinEntity<Page>
    {
        public Exhibit Exhibit { get; set; }
        public string ExhibitId { get; set; }
        Exhibit IJoinEntity<Exhibit>.Navigation { get => Exhibit; set => Exhibit = value; }

        public Page Page { get; set; }
        public string PageId { get; set; }
        Page IJoinEntity<Page>.Navigation { get => Page; set => Page = value; }
    }
}
