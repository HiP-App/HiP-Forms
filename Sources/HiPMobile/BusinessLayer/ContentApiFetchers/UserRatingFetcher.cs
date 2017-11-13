using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.ModelClasses;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers {

    public class UserRatingFetcher : IUserRatingFetcher {

        private readonly IUserRatingApiAccess client = new UserRatingApiAccess(new ContentApiClient());

        public async Task<IEnumerable<UserRating>> FetchUserRating() {
            var userRatingDto = await client.GetUserRatings();

            List<UserRating> useRatings = new List<UserRating>();
            useRatings.Add(new UserRating());
            return useRatings;
            //return 

            /*var existingAchievementIds = AchievementManager.GetAchievements().Select(it => it.Id);

            var achievementDtos = await client.GetUnlockedAchievements();
            using (DbManager.StartTransaction()) {
                var achievements = AchievementConverter.Convert(achievementDtos);
                return achievements.Where(it => !existingAchievementIds.Contains(it.Id));
            }*/
        }
    }
}