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
using MvvmHelpers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.InteractiveSources;
using Xamarin.Forms;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages.AudioTranscript
{
    public class SourcesViewModel : BaseViewModel
    {
        private List<Source> references;
        private double fontSize;

        public SourcesViewModel(List<Source> referencesToInit)
        {
            References = referencesToInit;
        }

        public List<Source> References
        {
            get { return references; }
            set { SetProperty(ref references, value); }
        }

        public double FontSize
        {
            get { return Settings.AudioTranscriptFontSize; }
            set { SetProperty(ref fontSize, value); }
        }
    }
}