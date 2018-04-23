using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers
{
    public static class QuizManager
    {

        public static ReadExtensions Quizzes(this IReadOnlyDataAccess dataAccess) => new ReadExtensions(dataAccess);

        public struct ReadExtensions
        {
            private readonly IReadOnlyDataAccess dataAccess;

            public ReadExtensions(IReadOnlyDataAccess dataAccess)
            {
                this.dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
            }

            public Quiz QuizForExhibit(string exhibitId)
            {
                return dataAccess.GetItems<Quiz>(nameof(Quiz.Exhibit), nameof(Quiz.Image))
                                 .SingleOrDefault(quiz => quiz.Exhibit.Id == exhibitId);
            }
        }
    }
}
