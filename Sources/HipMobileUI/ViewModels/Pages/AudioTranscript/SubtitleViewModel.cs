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
using System.Collections.Generic;
using de.upb.hip.mobile.pcl.BusinessLayer.InteractiveSources;
using MvvmHelpers;

namespace HipMobileUI.ViewModels.Pages.AudioTranscript {
    public class SubtitleViewModel : BaseViewModel {

        private string formatedText;
        private List<Source> sourcesList;
        private Func<IInteractiveSourceAction> action;

        public SubtitleViewModel(string formatedTextToInit, List<Source> sources, Func<IInteractiveSourceAction> actionToInit)
        {
            FormatedText = formatedTextToInit;
            SourcesList = sources;
            Action = actionToInit;
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

        public Func<IInteractiveSourceAction> Action
        {
            get { return action; }
            set { SetProperty(ref action, value); }
        }
    }
}