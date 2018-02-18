using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.JoinClasses
{
    public class JoinExhibitPage : IJoinEntity<Exhibit>, IJoinEntity<Page>
    {
        public Exhibit Exhibit { get; set; }

        public Page Page { get; set; }

        Exhibit IJoinEntity<Exhibit>.Navigation { get => Exhibit; set => Exhibit = value; }

        Page IJoinEntity<Page>.Navigation { get => Page; set => Page = value; }
    }
}
