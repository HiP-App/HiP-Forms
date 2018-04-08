using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiDtos;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses
{
    public class QuizApiAccess: IQuizApiAccess
    {
        // We need this t download JSONs. So if someone needs a QuizApiAccess, he needs to give us this to 
        // let us fetch JSONs
        private IContentApiClient apiClient;

        public QuizApiAccess(IContentApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        public async Task<List<QuizDto>> GetQuestionsForExhibit(int exhibitId)
        {
            var requestPath = $"/api/Exhibits/{exhibitId}/Questions";
            var json = await apiClient.GetResponseFromUrlAsString(requestPath);
            return JsonConvert.DeserializeObject<List<QuizDto>>(json);
        }
    }
}
