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
using de.upb.hip.mobile.pcl.BusinessLayer.InteractiveSources;
using MvvmHelpers;

namespace HipMobileUI.ViewModels.Pages.AudioTranscript {
    public class SubtitleViewModel : BaseViewModel {

        private string formatedText;
        private List<Source> sourcesList;

        public SubtitleViewModel(string formatedTextToInit, List<Source> sources)
        {
            FormatedText = formatedTextToInit;
            SourcesList = sources;
        }

        public string FormatedText {
            get { return formatedText; }
            set { SetProperty(ref formatedText, value); }
        }

        public List<Source> SourcesList
        {
            get { return sourcesList; }
            set { SetProperty (ref sourcesList, value); }
        }
    }
}