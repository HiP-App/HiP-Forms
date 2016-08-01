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


using System;
using System.Collections.Generic;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Java.Util.Regex;


namespace de.upb.hip.mobile.droid.Helpers.InteractiveSources {
    /// <summary>
    /// Provides functionality replace source markdown in a string with interactive sources.
    /// </summary>
    public class InteractiveSources {

        public const string HtmlStartTag = "<fn>";

        public const string HtmlEndTag = "</fn>";

        /// <summary>
        /// Tag that indicates the start of a source.
        /// </summary>
        public string StartTag { get; set; } = HtmlStartTag;

        /// <summary>
        /// Tag that indicates the end of a source.
        /// </summary>
        public string EndTag { get; set; } = HtmlEndTag;

        /// <summary>
        /// Parses the specified text with source markup (according to set start and end tags) and returns 
        /// a SpannableString containing interactive sources.
        /// </summary>
        /// <param name="text">Text to parse.</param>
        /// <param name="substitute">Substitute that replaces the source markdown.</param>
        /// <param name="action">Action that should be started when the user interacts with the source.</param>
        /// <returns>A SpannableString parsed from the text. Returns null, if a parameter is null.</returns>
        public SpannableString Parse (string text,
                                      IInteractiveSourceSubstitute substitute, IInteractiveSourceAction action)
        {
            if (text == null || substitute == null || action == null)
                return null;

            // TODO: use C# regexp?
            Pattern pattern = Pattern.Compile (StartTag + ".+?" + EndTag);
            Matcher matcher = pattern.Matcher (text);

            // used to store the starting index of the source and the source itself
            List<Source> sources = new List<Source> ();

            string sub = ""; // substitute for "current" source

            // collect sources
            while (matcher.Find ())
            {
                string match = matcher.Group ();

                // get source text
                string srcText = match;
                srcText = srcText.Replace (StartTag, "");
                srcText = srcText.Replace (EndTag, "");

                // store source 
                sources.Add (new Source (srcText, matcher.Start ()));

                // replace footnote markup with substitute
                sub = substitute.NextSubstitute ();
                text = text.Replace (match, sub);
                matcher = pattern.Matcher (text); // working with new text to get correct start index
            }

            // create SpannableString with ClickableSpans
            SpannableString str = new SpannableString (text);

            foreach (var src in sources)
            {
                if (src == null)
                    continue;

                str.SetSpan (
                    ConvertSrcToClickableSpan (src, action),
                    src.StartIndex,
                    src.StartIndex + sub.Length,
                    SpanTypes.ExclusiveExclusive);
            }

            return str;
        }

        /// <summary>
        /// Converts a source to a ClickableSpan that executes the specified action.
        /// </summary>
        /// <param name="src">Source that should be converted to a ClickableSpan.</param>
        /// <param name="action">Action that should be executed when the Click-event of the span is triggered.</param>
        /// <returns>ClickableSpan for the provided Source instance.</returns>
        private static ClickableSpan ConvertSrcToClickableSpan (Source src, IInteractiveSourceAction action)
        {
            if (src == null)
                return null;

            InteractiveSourceClickableSpan span = new InteractiveSourceClickableSpan ();
            span.Click += v => action.Display (src.Text);

            return span;
        }

        /// <summary>
        /// Represents a source by providing properties storing the source text and the start index.
        /// </summary>
        private class Source
        {

            public string Text { get; set; }

            public int StartIndex { get; set; }

            public Source(string text, int startIndex)
            {
                Text = text;
                StartIndex = startIndex;
            }

        }

        /// <summary>
        /// Adapted from https://forums.xamarin.com/discussion/15310/how-to-use-clickable-span
        /// </summary>
        private class InteractiveSourceClickableSpan : ClickableSpan
        {

            public Action<View> Click;

            public override void OnClick(View widget)
            {
                Click?.Invoke(widget);
            }

        }

    }
    
}