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

using System.Collections.ObjectModel;
using System.ComponentModel;
using de.upb.hip.mobile.pcl.Common;
using de.upb.hip.mobile.pcl.Helpers;
using HipMobileUI.Contracts;
using HipMobileUI.Resources;
using Xamarin.Forms;

namespace HipMobileUI.ViewModels.Pages
{
    public class UserOnboardingPageViewModel : NavigationViewModel
    {

        public UserOnboardingPageViewModel ()
        {
            Pages = new ObservableCollection<UserOnboardingItemViewModel>
            {
                new UserOnboardingItemViewModel (Strings.UserOnboarding_Explore_Title, Strings.UserOnboarding_Explore_Text, "ac_erkunden.jpg", Color.Green),
                new UserOnboardingItemViewModel (Strings.UserOnboarding_Route_Title, Strings.UserOnboarding_Route_Text, "ac_route.jpg", Color.Orange, "ac_route2.jpg"),
                new UserOnboardingItemViewModel (Strings.UserOnboarding_Students_Title, Strings.UserOnboarding_Students_Text, "ac_students", Color.Blue)
            };
            ContentOrientation = StackOrientation.Vertical;
            ForwardCommand = new Command (GotoNextPage);
            FinishCommand = new Command (ClosePage);
            SelectedPage = 0;
            UpdateVisibilityStatus ();
            PropertyChanged +=OnPropertyChanged;
        }

        /// <summary>
        /// React to property changes of the curretnly selected page.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="propertyChangedEventArgs">The event arguments.</param>
        private void OnPropertyChanged (object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName.Equals (nameof (SelectedPage)))
            {
                UpdateVisibilityStatus ();
            }
        }

        /// <summary>
        /// Close the page belonging to this viewmodel.
        /// </summary>
        private void ClosePage ()
        {
            // hide the status bar
            IStatusBarController statusBarController = IoCManager.Resolve<IStatusBarController> ();
            statusBarController.ShowStatusBar ();

            // update the settings to not show this page next time
            Settings.RepeatIntro = false;

            // open the main page
            Navigation.StartNewNavigationStack (new MainPageViewModel ());
        }

        /// <summary>
        /// Go to the next page.
        /// </summary>
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
        private StackOrientation contentOrientation;

        /// <summary>
        /// The displayed pages.
        /// </summary>
        public ObservableCollection<UserOnboardingItemViewModel> Pages {
            get { return pages; }
            set { SetProperty (ref pages, value); }
        }

        /// <summary>
        /// The index of the currently shown page.
        /// </summary>
        public int SelectedPage {
            get { return selectedPage; }
            set { SetProperty (ref selectedPage, value); }
        }

        /// <summary>
        /// The command for navigating forward.
        /// </summary>
        public Command ForwardCommand {
            get { return forwardCommand; }
            set { SetProperty (ref forwardCommand, value); }
        }

        /// <summary>
        /// The command for finishing the user onboarding.
        /// </summary>
        public Command FinishCommand {
            get { return finishCommand; }
            set { SetProperty (ref finishCommand, value); }
        }

        /// <summary>
        /// Flag indicating if the skip button is visible.
        /// </summary>
        public bool IsSkipVisible {
            get { return isSkipVisible; }
            set { SetProperty (ref isSkipVisible, value); }
        }

        /// <summary>
        /// Flag indicating if the forward button is visible.
        /// </summary>
        public bool IsForwardVisible {
            get { return isForwardVisible; }
            set { SetProperty (ref isForwardVisible, value); }
        }

        /// <summary>
        /// Flag indicating if the finish button is visible.
        /// </summary>
        public bool IsFinishVisible {
            get { return isFinishVisible; }
            set { SetProperty (ref isFinishVisible, value); }
        }

        /// <summary>
        /// The current orientation.
        /// </summary>
        public StackOrientation ContentOrientation {
            get { return contentOrientation; }
            set { SetProperty (ref contentOrientation, value); }
        }

        /// <summary>
        /// Update the visibility status of the buttons accrding to the selected page.
        /// </summary>
        private void UpdateVisibilityStatus ()
        {
            if (SelectedPage == Pages.Count - 1)
            {
                // last page
                IsFinishVisible = true;
                IsForwardVisible = false;
                IsSkipVisible = false;
            }
            else
            {
                // not last page
                IsFinishVisible = false;
                IsForwardVisible = true;

                // show skip only if more than two pages are available, otherwise finish button will be shown which makes skip redundant
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
