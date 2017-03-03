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

using System.Threading.Tasks;
using System.Windows.Input;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.Common;
using de.upb.hip.mobile.pcl.Common.Contracts;
using de.upb.hip.mobile.pcl.DataAccessLayer;
using de.upb.hip.mobile.pcl.DataLayer;
using de.upb.hip.mobile.pcl.Helpers;
using HipMobileUI.Resources;
using Xamarin.Forms;

namespace HipMobileUI.ViewModels.Pages
{
    class LoadingPageViewModel : NavigationViewModel, IProgressListener
    {

        public LoadingPageViewModel ()
        {
            Text = Strings.LoadingPage_Text;
            Subtext = Strings.LoadingPage_Subtext;
            StartLoading = new Command (Load);
        }

        private string text;
        private string subtext;
        private ICommand startLoading;
        private bool isExtendedViewsVisible;
        private string loadingProgressText;

        public string Text {
            get { return text; }
            set { SetProperty (ref text, value); }
        }

        public string Subtext {
            get { return subtext; }
            set { SetProperty (ref subtext, value); }
        }

        public ICommand StartLoading {
            get { return startLoading; }
            set { SetProperty (ref startLoading, value); }
        }

        public bool IsExtendedViewsVisible {
            get { return isExtendedViewsVisible; }
            set { SetProperty (ref isExtendedViewsVisible, value); }
        }

        public string LoadingProgressText {
            get { return loadingProgressText; }
            set { SetProperty (ref loadingProgressText, value); }
        }

        public void Load ()
        {
            Task.Factory.StartNew (() => {
                IoCManager.RegisterType<IDataAccess, RealmDataAccess>();
                IoCManager.RegisterType<IDataLoader, EmbeddedResourceDataLoader>();

                // show text, activity indicator and image when db is initialized, otherwise just the indicator is shown
                if (!DbManager.IsDatabaseUpToDate())
                {
                    IsExtendedViewsVisible = true;
                }
                DbManager.UpdateDatabase(this);

                // force the db to load the exhibitset into cache
                ExhibitManager.GetExhibitSets ();
                SetProgress(0.9);

                Device.BeginInvokeOnMainThread (() => {
                                                    Navigation.StartNewNavigationStack (new MainPageViewModel ());
                                                });
            });
        }

        /// <summary>
        /// React to progress updates.
        /// </summary>
        /// <param name="newProgress">The new progress value.</param>
        /// <param name="maxProgress">The maximum propgress value.</param>
        public void UpdateProgress (double newProgress, double maxProgress)
        {
            SetProgress ((newProgress / maxProgress)*0.8);
        }

        private void SetProgress (double progress)
        {
            LoadingProgressText = (progress).ToString("p0");
        }

    }
}
