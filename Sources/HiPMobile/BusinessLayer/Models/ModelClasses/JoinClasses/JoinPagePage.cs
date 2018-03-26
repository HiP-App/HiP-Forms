using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.JoinClasses
{
    /// <summary>
    /// Represents the many-to-many relationship between a
    /// <see cref="Models.Page"/> and its <see cref="Page.AdditionalInformationPages"/>.
    /// </summary>
    public class JoinPagePage : IJoinEntitySameType<Page>
    {
        public Page Page { get; set; }
        public string PageId { get; set; }

        public Page AdditionalInformationPage { get; set; }
        public string AdditionalInformationPageId { get; set; }

        public Page this[JoinSide side]
        {
            get => side == JoinSide.A ? Page : AdditionalInformationPage;
            set
            {
                switch (side)
                {
                    case JoinSide.A: Page = value; break;
                    case JoinSide.B: AdditionalInformationPage = value; break;
                }
            }
        }
    }
}
