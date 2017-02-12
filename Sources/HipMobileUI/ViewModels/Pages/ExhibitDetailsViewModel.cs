﻿// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
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

using System;
using System.Windows.Input;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using HipMobileUI.ViewModels.Views.ExhibitDetails;
using Xamarin.Forms;
using Page = de.upb.hip.mobile.pcl.BusinessLayer.Models.Page;

namespace HipMobileUI.ViewModels.Pages
{
    public class ExhibitDetailsViewModel : NavigationViewModel
    {

        private ExhibitSubviewViewModel selectedView;
        private readonly Exhibit exhibit;
        private ICommand nextViewCommand;
        private ICommand previousViewCommand;
        private bool previousViewAvailable;
        private bool nextViewAvailable;
        private int currentViewIndex;

        public ExhibitDetailsViewModel (string exhibitId) : this(ExhibitManager.GetExhibit(exhibitId))
        {
        }

        public ExhibitDetailsViewModel (Exhibit exhibit)
        {
            currentViewIndex = 0;
            if (exhibit != null)
            {
                this.exhibit = exhibit;
                SetCurrentView ();
                Title = exhibit.Name;
                if(exhibit.Pages.Count>1)
                    NextViewAvailable = true;
            }
            NextViewCommand = new Command (GotoNextView);
            PreviousViewCommand = new Command (GotoPreviousView);
        }

        private void GotoNextView ()
        {
            if (currentViewIndex < exhibit.Pages.Count - 1)
            {
                currentViewIndex++;
                SetCurrentView ();
                NextViewAvailable = currentViewIndex < exhibit.Pages.Count - 1;
                PreviousViewAvailable = true;
            }
        }

        private void GotoPreviousView ()
        {
            if (currentViewIndex > 0)
            {
                currentViewIndex--;
                SetCurrentView();
                PreviousViewAvailable = currentViewIndex > 0;
                NextViewAvailable = true;
            }
        }

        private void SetCurrentView ()
        {
            Page currentPage = exhibit.Pages [currentViewIndex];
            if (currentPage.IsAppetizerPage ())
            {
                SelectedView = new AppetizerViewModel (exhibit.Name, currentPage.AppetizerPage);
            }
            else if (currentPage.IsImagePage())
            {
                SelectedView = new ImageViewModel(currentPage.ImagePage);
            }
            else if (currentPage.IsTextPage())
            {

            }
            else if (currentPage.IsTimeSliderPage())
            {
                SelectedView = new TimeSliderViewModel(currentPage.TimeSliderPage);
            }
            else
            {
                throw new Exception("Unknown page found: " + currentPage);
            }
        }


        #region propeties
        /// <summary>
        /// The currently displayed subview.
        /// </summary>
        public ExhibitSubviewViewModel SelectedView
        {
            get { return selectedView; }
            set { SetProperty(ref selectedView, value); }
        }

        /// <summary>
        /// The command for switching to the next view, if available.
        /// </summary>
        public ICommand NextViewCommand
        {
            get { return nextViewCommand; }
            set { SetProperty(ref nextViewCommand, value); }
        }

        /// <summary>
        /// The command for switching to the previous view, if available.
        /// </summary>
        public ICommand PreviousViewCommand
        {
            get { return previousViewCommand; }
            set { SetProperty(ref previousViewCommand, value); }
        }

        /// <summary>
        /// Indicator if a previous view is available.
        /// </summary>
        public bool PreviousViewAvailable
        {
            get { return previousViewAvailable; }
            set { SetProperty(ref previousViewAvailable, value); }
        }

        /// <summary>
        /// Indicator if a next view is available.
        /// </summary>
        public bool NextViewAvailable
        {
            get { return nextViewAvailable; }
            set { SetProperty(ref nextViewAvailable, value); }
        }

        #endregion


    }
}
