// Copyright (C) 2016 History in Paderborn App - Universität Paderborn
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//       http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
namespace de.upb.hip.mobile.droid.Helpers.InteractiveSources {

    /// <summary>
    /// Represents a source by providing properties storing the source text, the start index and the substitute text.
    /// </summary>
    public class Source {
        public string SubstituteText { get; }

        public string Text { get; }

        public int StartIndex { get; }

        public int NumberInSubtitles { get; }

        public Source(string text, int startIndex, string substituteText, int numberInSubtitles)
        {
            Text = text;
            StartIndex = startIndex;
            SubstituteText = substituteText;
            NumberInSubtitles = numberInSubtitles;
        }
    }
}