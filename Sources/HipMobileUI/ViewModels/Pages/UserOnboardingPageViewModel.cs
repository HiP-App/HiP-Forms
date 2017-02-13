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
using System.Collections.ObjectModel;
using System.ComponentModel;
using HipMobileUI.Resources;
using Xamarin.Forms;

namespace HipMobileUI.ViewModels.Pages
{
    class UserOnboardingPageViewModel : NavigationViewModel
    {

        public UserOnboardingPageViewModel ()
        {
            Pages = new ObservableCollection<UserOnboardingItemViewModel>
            {
                new UserOnboardingItemViewModel (Strings.UserOnboarding_Explore_Title, Strings.UserOnboarding_Explore_Text, "ac_erkunden.jpg", Color.Green),
                new UserOnboardingItemViewModel (Strings.UserOnboarding_Route_Title, Strings.UserOnboarding_Route_Text, "ac_route.jpg", Color.Orange),
                new UserOnboardingItemViewModel (Strings.UserOnboarding_Students_Title, Strings.UserOnboarding_Students_Text, "ac_students", Color.Blue)
            };
            ForwardCommand = new Command (GotoNextPage);
            FinishCommand = new Command (ClosePage);
            SelectedPage = 0;
            UpdateVisibilityStatus ();
            PropertyChanged +=OnPropertyChanged;
        }

        private void OnPropertyChanged (object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName.Equals (nameof (SelectedPage)))
            {
                UpdateVisibilityStatus ();
            }
        }

        private void ClosePage ()
        {
            
        }

        private void GotoNextPage ()
        {
            SelectedPage++;
        }

        private ObservableCollection<UserOnboardingItemViewModel> pages;
        private Command forwardCommand;
        private Command finishCommand;
        private int selectedPage;
        private bool isSkipVisible;
        private bool isForwardVisible;
        private bool isFinishVisible;

        public ObservableCollection<UserOnboardingItemViewModel> Pages {
            get { return pages; }
            set { SetProperty (ref pages, value); }
        }

        public int SelectedPage {
            get { return selectedPage; }
            set { SetProperty (ref selectedPage, value); }
        }

        public Command ForwardCommand {
            get { return forwardCommand; }
            set { SetProperty (ref forwardCommand, value); }
        }

        public Command FinishCommand {
            get { return finishCommand; }
            set { SetProperty (ref finishCommand, value); }
        }

        public bool IsSkipVisible {
            get { return isSkipVisible; }
            set { SetProperty (ref isSkipVisible, value); }
        }

        public bool IsForwardVisible {
            get { return isForwardVisible; }
            set { SetProperty (ref isForwardVisible, value); }
        }

        public bool IsFinishVisible {
            get { return isFinishVisible; }
            set { SetProperty (ref isFinishVisible, value); }
        }

        private void UpdateVisibilityStatus ()
        {
            if (SelectedPage == Pages.Count - 1)
            {
                IsFinishVisible = true;
                IsForwardVisible = false;
                IsSkipVisible = false;
            }
            else
            {
                IsFinishVisible = false;
                IsForwardVisible = true;

                if (Pages.Count > 1)
                {
                    IsSkipVisible = true;
                }
                else
                {
                    IsSkipVisible = false;
                }
            }
        }
    }
}
