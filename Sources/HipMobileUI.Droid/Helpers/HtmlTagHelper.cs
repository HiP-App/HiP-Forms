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
using System.Text.RegularExpressions;

namespace PaderbornUniversity.SILab.Hip.Mobile.Droid.Helpers
{
    public class HtmlTagHelper
    {
        public string FormatAdditionalTags(string html)
        {
            html = FormatUnorderedLists(html);
            html = FormatOrderedLists(html);

            return html;
        }

        private string FormatOrderedLists(string html)
        {
            string listTag = "ol";
            MatchCollection ulMatches = GetMatchesForTag(listTag, html);

            foreach (Match ulMatch in ulMatches)
            {
                IListReplacementProvider provider = new OrderedListItemReplacementProvider();
                string ulValue = ulMatch.Value;

                ulValue = ReplaceLiTags(ulValue, provider);
                html = html.Replace(ulMatch.Value, ulValue);
            }

            return RemoveTags(listTag, html);
        }

        private string FormatUnorderedLists(string html)
        {
            string listTag = "ul";
            MatchCollection ulMatches = GetMatchesForTag(listTag, html);

            foreach (Match ulMatch in ulMatches)
            {
                IListReplacementProvider provider = new UnorderedListItemReplacementProvider();

                string ulValue = ulMatch.Value;

                ulValue = ReplaceLiTags(ulValue, provider);
                html = html.Replace(ulMatch.Value, ulValue);
            }

            return RemoveTags(listTag, html);
        }

        private string ReplaceLiTags(string html, IListReplacementProvider replacementProvider)
        {
            while (html.Contains("<li>"))
            {
                html = ReplaceFirst(html, "<li>", replacementProvider.GetReplacement());
            }

            html = html.Replace("</li>", "<br>");

            return html;
        }

        private string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search, StringComparison.Ordinal);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        private string RemoveTags(string tag, string html)
        {
            string result = html.Replace($"<{tag}>", string.Empty);
            return result.Replace($"</{tag}>", string.Empty);
        }

        private MatchCollection GetMatchesForTag(string tag, string html)
        {
            var pattern = new Regex($"<{tag}>.+?</{tag}>");
            return pattern.Matches(html);
        }
    }
}