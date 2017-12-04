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
            existingModelObject.ThumbnailUrl = dto.ThumbnailUrl;
            existingModelObject.NextId = dto.NextId.ToString();
            existingModelObject.Title = dto.Title;
            existingModelObject.Points = dto.Points;
            existingModelObject.RouteId = dto.RouteId;
        }
    }
}