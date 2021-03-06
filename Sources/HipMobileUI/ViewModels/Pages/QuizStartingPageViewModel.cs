﻿// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
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

using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages
{
    public class QuizStartingPageViewModel : NavigationViewModel
    {
        private Exhibit exhibit;
        private string headline;
        private ICommand nextViewCommand;
        private ICommand startQuizCommand;
        private string score;

        public QuizStartingPageViewModel(Exhibit e)
        {
            Exhibit = e;
            Headline = e.Name;
            score = ComputeScoreString(e);
            NextViewCommand = new Command(async () => await GotoNextView());
            StartQuizCommand = new Command(async () => await GotoQuizView());
        }

        private static string ComputeScoreString(Exhibit exhibit)
        {
            var score = Settings.ExhibitScores.ScoreFor(exhibit) ?? 0;
            var totalQuestions = DbManager.DataAccess.Quizzes().QuizzesForExhibit(exhibit.Id).Count();
            return $"{score}/{totalQuestions}";
        }

        private async Task GotoNextView()
        {
            Navigation.InsertPageBefore(new UserRatingPageViewModel(Exhibit), this);
            await Navigation.PopAsync(false);
        }

        private async Task GotoQuizView()
        {
            Navigation.InsertPageBefore(new QuizPageViewModel(Exhibit), this);
            await Navigation.PopAsync(false);
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
            get { return headline; }
            set { SetProperty(ref headline, value); }
        }

        public ICommand NextViewCommand
        {
            get { return nextViewCommand; }
            set { SetProperty(ref nextViewCommand, value); }
        }

        public ICommand StartQuizCommand
        {
            get { return startQuizCommand; }
            set { SetProperty(ref startQuizCommand, value); }
        }

        public string Score
        {
            get => score;
            set => SetProperty(ref score, value);
        }

        #endregion
    }
}