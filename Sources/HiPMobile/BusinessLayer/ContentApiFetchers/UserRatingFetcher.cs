using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.ModelClasses;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers {

    public class UserRatingFetcher : IUserRatingFetcher {

        private readonly IUserRatingApiAccess client = new UserRatingApiAccess(new ContentApiClient(ServerEndpoints.DevelopApiPath));

        public async Task<UserRating> FetchUserRating(Exhibit exhibit) {
            var userRatingDto = await client.GetUserRating(exhibit);
            UserRating userRating = new UserRating();
            userRating.Id = userRatingDto.Id;
            userRating.Average = userRatingDto.Average;
            userRating.Count = userRatingDto.Count;
            return userRating;
        }
    }
}