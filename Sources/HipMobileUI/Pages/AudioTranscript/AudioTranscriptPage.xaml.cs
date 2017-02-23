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
using System.Linq;
using de.upb.hip.mobile.pcl.BusinessLayer.InteractiveSources;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using HipMobileUI.Navigation;
using HipMobileUI.Resources;
using HipMobileUI.ViewModels.Pages.AudioTranscript;
using Xamarin.Forms;

namespace HipMobileUI.Pages.AudioTranscript
{
    public partial class AudioTranscriptPage : TabbedPage, IViewFor<AudioTranscriptViewModel>
    {
        private readonly Page subtitlesPage;
        private readonly Page sourcesPage;
        public AudioTranscriptPage()
        {
            //InitializeComponent();
            var id = ExhibitManager.GetExhibitSet().Last();
            var subtitles = id.Pages[1].Audio.Caption;

            var parser = new InteractiveSourcesParser(new ConsecutiveNumberAndConstantInteractiveSourceSubstitute(1, "Quelle"));
            var result = parser.Parse(subtitles);

            string formatedText = result.TextWithSubstitutes;
            List<Source> references = result.Sources;

            subtitlesPage = new SubtitlesPage(formatedText);
            sourcesPage = new SourcesPage(references);

            subtitlesPage.Title = Strings.AudioTranscriptView_Subtitles_Title;
            sourcesPage.Title = Strings.AudioTranscriptView_Sources_Title;

            Children.Add(subtitlesPage);
            Children.Add(sourcesPage);
        }
    }
}
