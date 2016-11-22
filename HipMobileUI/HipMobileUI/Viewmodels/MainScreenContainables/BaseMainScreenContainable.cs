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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace HipMobileUI.Viewmodels.MainScreenContainables {
    public class BaseMainScreenContainable : INotifyPropertyChanged {

        private View containedView;
        private Func<View> createView;

        private string title;

        public BaseMainScreenContainable (string title, Func<View> createView)
        {
            Title = title;
            CreateView = createView;
        }

        public Func<View> CreateView {
            get { return createView; }
            set {
                createView = value;
                containedView = null;
                OnPropertyChanged (nameof (ContainedView));
            }
        }

        public string Title {
            get { return title; }
            set {
                title = value;
                OnPropertyChanged ();
            }
        }

        public View ContainedView {
            get {
                if (containedView == null)
                    containedView = CreateView.Invoke ();
                return containedView;
            }
        }

        public void DisposeView ()
        {
            this.containedView = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged ([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke (this, new PropertyChangedEventArgs (propertyName));
        }

    }
}