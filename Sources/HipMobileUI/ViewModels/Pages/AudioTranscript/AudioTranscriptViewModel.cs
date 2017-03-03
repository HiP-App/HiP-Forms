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

namespace HipMobileUI.ViewModels.Pages.AudioTranscript
{
    public class AudioTranscriptViewModel : NavigationViewModel{

        public AudioTranscriptViewModel (string subtitles)
        {
            var parser = new InteractiveSourcesParser(new ConsecutiveNumberAndConstantInteractiveSourceSubstitute(1, "Quelle"));
            var result = parser.Parse(subtitles);

            string formatedText = result.TextWithSubstitutes;
            List<Source> references = result.Sources;

            SourcesTab = new SourcesViewModel (references);
            SubtitleTab = new SubtitleViewModel (formatedText, references);
        }

        private SourcesViewModel sourcesTab;
        public SourcesViewModel SourcesTab
        {
            get { return sourcesTab; }
            set { SetProperty (ref sourcesTab, value); }
        }

        private SubtitleViewModel subtitleTab;
        public SubtitleViewModel SubtitleTab
        {
            get { return subtitleTab; }
            set { SetProperty(ref subtitleTab, value); }
        }

    }
}