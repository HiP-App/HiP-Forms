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

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using de.upb.hip.mobile.pcl.BusinessLayer.InteractiveSources;
using HipMobileUI.Properties;

namespace HipMobileUI.ViewModels.Pages.AudioTranscript
{
    public class SourcesViewModel : INotifyPropertyChanged
    {
        private List<Source> references;

        public void Init(List<Source> references)
        {
            References = references;
        }

        public List<Source> References
        {
            get { return references; }
            set
            {
                references = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}