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
using System.IO;
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

        private string answer1;
        private string answer2;
        private string answer3;
        private string answer4;

        private string[] Answers
        {
            get => new[] { Answer1, Answer2, Answer3, Answer4 };
            set
            {
                Debug2.Assert(value.Length == 4, "A quiz must have exactly 4 options.");
                Answer1 = value[0];
                Answer2 = value[1];
                Answer3 = value[2];
                Answer4 = value[3];
            }
        }

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

        private async void LoadNextQuiz()
        {
            currentQuiz++;
            var quiz = quizzes[currentQuiz];

            Answers = quiz.ShuffledOptions();
            Question = quiz.Text;
            AnswerABackgroundColor = DefaultAnswerBackgroundColor;
            AnswerBBackgroundColor = DefaultAnswerBackgroundColor;
            AnswerCBackgroundColor = DefaultAnswerBackgroundColor;
            AnswerDBackgroundColor = DefaultAnswerBackgroundColor;
            var imageDataTask = quiz.Image?.GetDataAsync();
            var imageData = imageDataTask != null ? await imageDataTask : null;
            QuizImage = imageData != null ? ImageSource.FromStream(() => new MemoryStream(imageData)) : ImageSource.FromFile("quiz_default_picture.png");
        }

        private async Task GotoNextView(int selectedAnswerIdx, Action<Color> backgroundColorSetter)
        {
            var correctOption = quizzes[currentQuiz].CorrectOption();
            var selectedAnswer = Answers[selectedAnswerIdx];
            for (var i = 0; i < Answers.Length; i++)
            {
                if (correctOption == Answers[i])
                {
                    MarkCorrectOption(i);
                }
            }
            
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
                var newHighScore = Math.Max(Settings.ExhibitScores.ScoreFor(exhibit) ?? score, score);
                Settings.ExhibitScores.SaveScoreFor(exhibit, newHighScore);
                Navigation.InsertPageBefore(new QuizStartingPageViewModel(exhibit), this);
                await Navigation.PopAsync(false);
            }
        }

        private void MarkCorrectOption(int index)
        {
            switch (index)
            {
                case 0:
                    AnswerABackgroundColor = Color.Green;
                    break;
                case 1:
                    AnswerBBackgroundColor = Color.Green;
                    break;
                case 2:
                    AnswerCBackgroundColor = Color.Green;
                    break;
                case 3:
                    AnswerDBackgroundColor = Color.Green;
                    break;
            }
        }

        #region properties

        public Exhibit Exhibit
        {
            get => exhibit;
            set => SetProperty(ref exhibit, value);
        }

        /// <summary>
        /// The headline of the description.
        /// </summary>
        public string Headline
        {
            get => question;
            set => SetProperty(ref question, value);
        }

        public ImageSource QuizImage
        {
            get => quizImage;
            set => SetProperty(ref quizImage, value);
        }

        public string Question
        {
            get => headline;
            set => SetProperty(ref headline, value);
        }

        public string Answer1
        {
            get => answer1;
            set => SetProperty(ref answer1, value);
        }

        public string Answer2
        {
            get => answer2;
            set => SetProperty(ref answer2, value);
        }

        public string Answer3
        {
            get => answer3;
            set => SetProperty(ref answer3, value);
        }

        public string Answer4
        {
            get => answer4;
            set => SetProperty(ref answer4, value);
        }

        public ICommand AnswerACommand
        {
            get => answerACommand;
            set => SetProperty(ref answerACommand, value);
        }

        public ICommand AnswerBCommand
        {
            get => answerBCommand;
            set => SetProperty(ref answerBCommand, value);
        }

        public ICommand AnswerCCommand
        {
            get => answerCCommand;
            set => SetProperty(ref answerCCommand, value);
        }

        public ICommand AnswerDCommand
        {
            get => answerDCommand;
            set => SetProperty(ref answerDCommand, value);
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
