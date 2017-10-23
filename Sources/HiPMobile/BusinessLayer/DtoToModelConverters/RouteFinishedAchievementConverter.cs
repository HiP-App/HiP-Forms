using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiDtos;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.DtoToModelConverters
{
    public class RouteFinishedAchievementConverter: DtoToModelConverter<RouteFinishedAchievement, RouteFinishedAchievementDto>
    {
        public override void Convert(RouteFinishedAchievementDto dto, RouteFinishedAchievement existingModelObject)
        {
            existingModelObject.Description = dto.Description;
            existingModelObject.Id = dto.Id.ToString();
            existingModelObject.ImageUrl = dto.ImageUrl;
            existingModelObject.NextId = dto.NextId;
            existingModelObject.Title = dto.Title;
        }
    }
}