// Copyright (C) 2016 History in Paderborn App - Universität Paderborn
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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using HipMobileUI.Annotations;
using HiPMobileUI.Pages;
using Xamarin.Forms;

namespace HipMobileUI.Viewmodels.MainScreenContainables {
    public class ExhibitsOverviewViewmodel : INotifyCollectionChanged {

        private INavigation navigation;

        public ExhibitsOverviewViewmodel ()
        {
            Exhibits= new ObservableCollection<ExhibitListItemViewmodel> ();
            SelectedExhibitCommand = new Command<ExhibitListItemViewmodel> (ExhibitSelected);
        }

        public void Init (string exhibitSetId, INavigation navigation)
        {
            var exhibitSet = ExhibitManager.GetExhibitSet (exhibitSetId);
            this.navigation = navigation;
            //for(int i=0; i<10;i++)
            foreach (Exhibit exhibit in exhibitSet.ActiveSet)
            {
                Exhibits.Add (new ExhibitListItemViewmodel (exhibit.Id));
            }
        }

        private ObservableCollection<ExhibitListItemViewmodel> exhibits;

        public ObservableCollection<ExhibitListItemViewmodel> Exhibits {
            get { return exhibits; }
            set {
                exhibits = value;
                OnPropertyChanged ();
            }
        }

        public ICommand SelectedExhibitCommand { get; set; }

        public void ExhibitSelected (ExhibitListItemViewmodel exhibitViewmodel)
        {
            navigation.PushAsync (new ExhibitDetailsPage (exhibitViewmodel.ExhibitId));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged ([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke (this, new PropertyChangedEventArgs (propertyName));
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

    }
}