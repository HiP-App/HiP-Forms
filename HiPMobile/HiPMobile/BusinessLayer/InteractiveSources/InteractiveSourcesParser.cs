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


using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace de.upb.hip.mobile.pcl.BusinessLayer.InteractiveSources
{
    /// <summary>
    /// Provides functionality replace source markdown in a string with interactive sources.
    /// </summary>
    public class InteractiveSourcesParser
    {

        /// <summary>
        /// Tag that indicates the start of a source.
        /// </summary>
        private const string HtmlStartTag = "<fn>";

        /// <summary>
        /// Tag that indicates the end of a source.
        /// </summary>
        private const string HtmlEndTag = "</fn>";

        public List<Source> Sources { get; }

        public string TextWithSubstitutes { get; private set; }

        /// <summary>
        /// Creates a parser for the specified textWithSubstitutes with source markup.
        /// </summary>
        /// <param name="textWithSubstitutes">TextWithSubstitutes to parse.</param>
        /// <param name="substitute">Substitute that replaces the source markdown.</param>
        public InteractiveSourcesParser(string textWithSubstitutes, IInteractiveSourceSubstitute substitute)
        {
            Sources = new List<Source>();
            TextWithSubstitutes = textWithSubstitutes;

            Parse(substitute);
        }

        /// <summary>
        /// Parses the specified textWithSubstitutes with source markup (according to set start and end tags) as preperation
        /// for getting subtitles and references texts.
        /// </summary>
        /// <param name="substitute">Substitute that replaces the source markdown.</param>
        private void Parse(IInteractiveSourceSubstitute substitute)
        {
            if (substitute == null)
                return;

            var pattern = new Regex (HtmlStartTag + ".+?" + HtmlEndTag);
            var match = pattern.Match(TextWithSubstitutes);

            int index = 0;

            while(match.Success)
            {
                string oldText = match.Value;
                string srcText = oldText;
                srcText = srcText.Replace(HtmlStartTag, string.Empty);
                srcText = srcText.Replace(HtmlEndTag, string.Empty);

                string sub = substitute.NextSubstitute();

                Sources.Add(new Source(srcText, match.Index, sub, index));

                // replace footnote markup with substitute
                TextWithSubstitutes = TextWithSubstitutes.Replace(oldText, sub);
                match = pattern.Match(TextWithSubstitutes); // working with new textWithSubstitutes to get correct start index
                index++;
            }
        }

    }

}