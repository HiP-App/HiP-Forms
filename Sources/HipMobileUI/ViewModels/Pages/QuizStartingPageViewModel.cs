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

using System;
using System.Threading.Tasks;
using System.Windows.Input;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages
{
    public class QuizStartingPageViewModel: NavigationViewModel
    {
        private Exhibit exhibit;
        private String headline;
        private ICommand nextViewCommand;

        public QuizStartingPageViewModel(Exhibit e)
        {
            Exhibit = e;
            Headline = e.Name;
            NextViewCommand = new Command(async () => await GotoNextView());
        }

        private async Task GotoNextView()
        {
            Navigation.InsertPageBefore(new UserRatingPageViewModel(Exhibit), this);
            Navigation.PopAsync(false);
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
        #endregion
    }
}