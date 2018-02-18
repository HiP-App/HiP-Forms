using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.JoinClasses
{
    public class JoinPagePage : IJoinEntitySameType<Page>
    {
        public Page Page { get; set; }

        public Page AdditionalInformationPage { get; set; }

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
