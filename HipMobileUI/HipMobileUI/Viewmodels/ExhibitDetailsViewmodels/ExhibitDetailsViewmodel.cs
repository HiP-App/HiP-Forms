// Copyright (C) 2016 History in Paderborn App - Universität Paderborn
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//       http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using HipMobileUI.Annotations;
using HipMobileUI.Views;
using HipMobileUI.Views.ExhibitDetailsViews;
using Xamarin.Forms;
using Page = de.upb.hip.mobile.pcl.BusinessLayer.Models.Page;

namespace HipMobileUI.Viewmodels {
    public class ExhibitDetailsViewmodel : INotifyPropertyChanged {

        private Exhibit exhibit;
        private int currentPageIndex = 0;

        private string title;
        private ContentView mainView;
        private bool isNextPageEnabled;
        private bool isPreviousPageEnabled;
        private bool isFirstPage;

        public ExhibitDetailsViewmodel ()
        {
            this.SwitchToNextPage = new Command(DisplayNextPage);
            this.SwitchToPreviousPage = new Command(DisplayPreviousPage);
        }

        public void Init (string exhibitId)
        {
            exhibit = ExhibitManager.GetExhibit (exhibitId);
            if (exhibit != null)
            {
                this.Title = exhibit.Name;
                SetPage (exhibit.Pages[currentPageIndex]);
            }
            this.IsFirstPage = true;
        }

        public void SetPage (Page modelPage)
        {
            if (currentPageIndex + 1 == exhibit.Pages.Count)
            {
                IsNextPageEnabled = false;
            }
            else
            {
                IsNextPageEnabled = true;
            }

            if (currentPageIndex == 0)
            {
                IsPreviousPageEnabled = false;
                IsFirstPage = true;
            }
            else
            {
                IsPreviousPageEnabled = true;
                IsFirstPage = false;
            }

            var page = exhibit.Pages [currentPageIndex];
            if (page.IsAppetizerPage ())
            {
                MainView= new AppetizerView (page.AppetizerPage);
            }
            else if (page.IsImagePage ())
            {
                MainView = new ImageView (page.ImagePage);
            }
            else if (page.IsTimeSliderPage ())
            {
                MainView = new ContentView();
            }
            else
            {
                MainView = new ContentView ();
            }
        }

        private void DisplayNextPage ()
        {
            if (currentPageIndex < exhibit.Pages.Count-1)
            {
                this.currentPageIndex++;
                this.SetPage (exhibit.Pages[currentPageIndex]);
            }
        }

        private void DisplayPreviousPage ()
        {
            if (currentPageIndex > 0)
            {
                this.currentPageIndex--;
                this.SetPage(exhibit.Pages[currentPageIndex]);
            }
        }

        public string Title {
            get { return title; }
            set {
                title = value;
                OnPropertyChanged ();
            }
        }

        public ContentView MainView {
            get { return mainView; }
            set {
                mainView = value;
                OnPropertyChanged ();
            }
        }

        public bool IsNextPageEnabled {
            get { return isNextPageEnabled; }
            set {
                isNextPageEnabled = value;
                OnPropertyChanged ();
            }
        }

        public bool IsPreviousPageEnabled {
            get { return isPreviousPageEnabled; }
            set {
                isPreviousPageEnabled = value;
                OnPropertyChanged ();
            }
        }

        public bool IsFirstPage {
            get { return isFirstPage; }
            set {
                isFirstPage = value;
                OnPropertyChanged ();
            }
        }

        public ICommand SwitchToNextPage { get; set; }

        public ICommand SwitchToPreviousPage { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged ([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke (this, new PropertyChangedEventArgs (propertyName));
        }

    }
}