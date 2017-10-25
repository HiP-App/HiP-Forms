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
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.InteractiveSources;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages.AudioTranscript
{
    public class AudioTranscriptViewModel : NavigationViewModel
    {
        public AudioTranscriptViewModel(string subtitles, string exhibitTitle)
        {
            Title = exhibitTitle;
            var parser = new InteractiveSourcesParser(new ConsecutiveNumberAndConstantInteractiveSourceSubstitute(1, "Quelle"));
            var result = parser.Parse(subtitles);

            var formatedText = result.TextWithSubstitutes;
            var references = result.Sources;

            SourcesTab = new SourcesViewModel(references);
            SubtitleTab = new SubtitleViewModel(formatedText, references, GetAction);

            changeFontSize = new Command(ChangeFontSize);
        }

        private void ChangeFontSize()
        {
            if (Math.Abs(Settings.AudioTranscriptFontSize - 14) < 0.00001)
            {
                Settings.AudioTranscriptFontSize = 19;
            }
            else if (Math.Abs(Settings.AudioTranscriptFontSize - 19) < 0.00001)
            {
                Settings.AudioTranscriptFontSize = 24;
            }
            else
            {
                Settings.AudioTranscriptFontSize = 14;
            }

            SubtitleTab.FontSize = Settings.AudioTranscriptFontSize;
            SourcesTab.FontSize = Settings.AudioTranscriptFontSize;
        }

        private Command changeFontSize;

        public Command ChangeFontSizeCommand
        {
            get { return changeFontSize; }
            set { SetProperty(ref changeFontSize, value); }
        }

        public IInteractiveSourceAction GetAction()
        {
            return Action;
        }

        private SourcesViewModel sourcesTab;

        public SourcesViewModel SourcesTab
        {
            get { return sourcesTab; }
            set { SetProperty(ref sourcesTab, value); }
        }

        private SubtitleViewModel subtitleTab;

        public SubtitleViewModel SubtitleTab
        {
            get { return subtitleTab; }
            set { SetProperty(ref subtitleTab, value); }
        }

        private IInteractiveSourceAction action;

        public IInteractiveSourceAction Action
        {
            get { return action; }
            set { SetProperty(ref action, value); }
        }

        private double fontSize;

        public double FontSize
        {
            get { return Settings.AudioTranscriptFontSize; }
            set { SetProperty(ref fontSize, value); }
        }
    }
}