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
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages
{
    public class QuizPageViewModel: NavigationViewModel
    {
        private Exhibit exhibit;
        private String headline;
        private ICommand nextViewCommand;
        private Quiz[] Quizzes;
        private String question;
        private String[] answers;
        private ImageSource quizImage;

        public QuizPageViewModel(Exhibit e)
        {

            Exhibit = e;
            Headline = e.Name;
            answers= new String[4];
            SetQuiz();
            NextViewCommand = new Command(async () => await GotoNextView());

        }
        private async Task GotoNextView()
        {
            Navigation.InsertPageBefore(new UserRatingPageViewModel(Exhibit), this);
            Navigation.PopAsync(false);
        }

        private void SetQuiz()
        {
            QuizImage = ImageSource.FromFile("quiz_default_picture.png");
            Question = "This is a dummy question!";
            Answer1 = "first answer";
            Answer2 = "second answer";
            Answer3 = "third answer";
            Answer4 = "fourth answer";
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
        public ICommand NextViewCommand
        {
            get { return nextViewCommand; }
            set { SetProperty(ref nextViewCommand, value); }
        }
        #endregion
    }
}