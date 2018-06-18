using System.Linq;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiDtos;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.DtoToModelConverters
{
    public class QuizConverter : DtoToModelConverter<Quiz, QuizDto>
    {
        private readonly IReadOnlyDataAccess dataAccess = DbManager.DataAccess;

        public override void Convert(QuizDto dto, Quiz existingModelObject)
        {
            existingModelObject.Status = dto.Status;
            existingModelObject.Timestamp = dto.Timestamp;
            existingModelObject.Id = dto.Id.ToString();
            existingModelObject.Text = dto.Text;
            existingModelObject.Exhibit = dataAccess.Exhibits().GetExhibits().First(it => it.IdForRestApi == dto.ExhibitId);
            existingModelObject.Options = dto.Options;
            existingModelObject.Image = dataAccess.GetItem<Image>(dto.Image?.ToString());
        }
    }
}