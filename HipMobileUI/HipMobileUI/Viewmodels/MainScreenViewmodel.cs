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

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using HipMobileUI.Viewmodels.MainScreenContainables;
using HipMobileUI.Views;
using Xamarin.Forms;

namespace HipMobileUI.Viewmodels {
    public class MainScreenViewmodel : INotifyPropertyChanged, INotifyCollectionChanged{

        private BaseMainScreenContainable selectedContainable;
        private ObservableCollection<BaseMainScreenContainable> containables;

        public MainScreenViewmodel ()
        {
            var exhibitSet = ExhibitManager.GetExhibitSets ().FirstOrDefault();
            Containables = new ObservableCollection<BaseMainScreenContainable>
            {
                new BaseMainScreenContainable ("Übersicht", () => new ExhibitsOverviewView (exhibitSet.Id)),
                new BaseMainScreenContainable ("Dialog", () => new DialogView()),
                new BaseMainScreenContainable ("Blue", () => new ColorView (Color.Blue)),
                new BaseMainScreenContainable ("Another Text", () => new TextView ("This is a text test!")),
                new BaseMainScreenContainable ("Audio", () => new AudioView ())
            };
        }


        public BaseMainScreenContainable SelectedContainable {
            get { return selectedContainable; }
            set {
                selectedContainable?.DisposeView ();
                selectedContainable = value;
                OnPropertyChanged ();
            }
        }

        public ObservableCollection<BaseMainScreenContainable> Containables {
            get { return containables; }
            set {
                containables = value;
                OnPropertyChanged ();
            }
        }

        public void SetStartItem ()
        {
            SelectedContainable = Containables[0];
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged ([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke (this, new PropertyChangedEventArgs (propertyName));
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

    }
}