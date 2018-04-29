// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages
{
    public class QuizPageViewModel : NavigationViewModel
    {
        private const int AnswerCorrectnessDisplayTimeMs = 3000;

        private readonly Quiz[] quizzes;
        private int currentQuiz = -1;
        private int score = 0;

        private Exhibit exhibit;
        private string headline;
        private ICommand answerACommand;
        private ICommand answerBCommand;
        private ICommand answerCCommand;
        private ICommand answerDCommand;
        private Color answerABackgroundColor = DefaultAnswerBackgroundColor;
        private Color answerBBackgroundColor = DefaultAnswerBackgroundColor;
        private Color answerCBackgroundColor = DefaultAnswerBackgroundColor;
        private Color answerDBackgroundColor = DefaultAnswerBackgroundColor;
        private string question;
        private string[] answers;
        private ImageSource quizImage;
        private static readonly Color DefaultAnswerBackgroundColor = Color.LightGray;

        public QuizPageViewModel(Exhibit e)
        {
            quizzes = DbManager.DataAccess.Quizzes().QuizzesForExhibit(e.Id).Shuffle().ToArray();

            Exhibit = e;
            Headline = e.Name;
            LoadNextQuiz();

            AnswerACommand = new Command(async origin => await GotoNextView(0, bgColor => AnswerABackgroundColor = bgColor));
            AnswerBCommand = new Command(async origin => await GotoNextView(1, bgColor => AnswerBBackgroundColor = bgColor));
            AnswerCCommand = new Command(async origin => await GotoNextView(2, bgColor => AnswerCBackgroundColor = bgColor));
            AnswerDCommand = new Command(async origin => await GotoNextView(3, bgColor => AnswerDBackgroundColor = bgColor));
        }

        private void LoadNextQuiz()
        {
            currentQuiz++;
            var quiz = quizzes[currentQuiz];

            answers = quiz.ShuffledOptions();
            Question = quiz.Text;
            AnswerABackgroundColor = DefaultAnswerBackgroundColor;
            AnswerBBackgroundColor = DefaultAnswerBackgroundColor;
            AnswerCBackgroundColor = DefaultAnswerBackgroundColor;
            AnswerDBackgroundColor = DefaultAnswerBackgroundColor;
            // TODO use actual image
            QuizImage = ImageSource.FromFile("quiz_default_picture.png");
        }

        private async Task GotoNextView(int selectedAnswerIdx, Action<Color> backgroundColorSetter)
        {
            var selectedAnswer = answers[selectedAnswerIdx];
            var isAnswerCorrect = selectedAnswer == quizzes[currentQuiz].CorrectOption();
            backgroundColorSetter(isAnswerCorrect ? Color.Green : Color.DarkRed);
            if (isAnswerCorrect) score++;

            await Task.Delay(AnswerCorrectnessDisplayTimeMs);

            if (currentQuiz + 1 < quizzes.Length)
            {
                LoadNextQuiz();
            }
            else
            {
                var existingScore = DbManager.DataAccess.GetItem<ExhibitQuizScore>(exhibit.Id);
                // TODO Is it necessary to track this object?
                await DbManager.InTransactionAsync(new[] { existingScore }.WhereNotNull(), transaction =>
                {
                    if (existingScore == null)
                    {
                        transaction.DataAccess.AddItem(new ExhibitQuizScore(exhibit, score));
                    }
                    else
                    {
                        existingScore.Score = Math.Max(existingScore.Score, score);
                    }
                });
                Navigation.InsertPageBefore(new QuizStartingPageViewModel(exhibit), this);
                await Navigation.PopAsync(false);
            }
        }

        #region properties

        public Exhibit Exhibit
        {
            get { return exhibit; }
            set { SetProperty(ref exhibit, value); }
        }

        /// <summary>
        /// The headline of the description.
        /// </summary>
        public string Headline
        {
            get { return question; }
            set { SetProperty(ref question, value); }
        }

        public ImageSource QuizImage
        {
            get { return quizImage; }
            set { SetProperty(ref quizImage, value); }
        }

        public string Question
        {
            get { return headline; }
            set { SetProperty(ref headline, value); }
        }

        public string Answer1
        {
            get { return answers[0]; }
            set { SetProperty(ref answers[0], value); }
        }

        public string Answer2
        {
            get { return answers[1]; }
            set { SetProperty(ref answers[1], value); }
        }

        public string Answer3
        {
            get { return answers[2]; }
            set { SetProperty(ref answers[2], value); }
        }

        public string Answer4
        {
            get { return answers[3]; }
            set { SetProperty(ref answers[3], value); }
        }

        public ICommand AnswerACommand
        {
            get { return answerACommand; }
            set { SetProperty(ref answerACommand, value); }
        }

        public ICommand AnswerBCommand
        {
            get { return answerBCommand; }
            set { SetProperty(ref answerBCommand, value); }
        }

        public ICommand AnswerCCommand
        {
            get { return answerCCommand; }
            set { SetProperty(ref answerCCommand, value); }
        }

        public ICommand AnswerDCommand
        {
            get { return answerDCommand; }
            set { SetProperty(ref answerDCommand, value); }
        }

        public Color AnswerABackgroundColor
        {
            get => answerABackgroundColor;
            set => SetProperty(ref answerABackgroundColor, value);
        }

        public Color AnswerBBackgroundColor
        {
            get => answerBBackgroundColor;
            set => SetProperty(ref answerBBackgroundColor, value);
        }

        public Color AnswerCBackgroundColor
        {
            get => answerCBackgroundColor;
            set => SetProperty(ref answerCBackgroundColor, value);
        }

        public Color AnswerDBackgroundColor
        {
            get => answerDBackgroundColor;
            set => SetProperty(ref answerDBackgroundColor, value);
        }

        #endregion
    }
}