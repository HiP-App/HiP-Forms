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
    public class InteractiveSources {

        public const string HtmlStartTag = "<fn>";

        public const string HtmlEndTag = "</fn>";

        public string StartTag { get; set; } = HtmlStartTag;

        public string EndTag { get; set; } = HtmlEndTag;

        /// <summary>
        /// Parses the specified text with source markup and returns a SpannableString containing interactive
        /// sources
        /// </summary>
        /// <param name="text">Text to parse.</param>
        /// <param name="substitute">Text that is substituted for the original source.</param>
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

            string sub = "";

            // collect sources
            while (matcher.Find ())
            {
                string match = matcher.Group ();

                // get source text
                string source = match;
                source = source.Replace (StartTag, "");
                source = source.Replace (EndTag, "");

                // store source 
                sources.Add (new Source (source, matcher.Start ()));

                // replace footnote markup with substitute

                sub = substitute.NextSubstitute ();
                text = text.Replace (match, sub);
                matcher = pattern.Matcher (text); // working with new text to get correct start 
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


        private static ClickableSpan ConvertSrcToClickableSpan (Source src, IInteractiveSourceAction action)
        {
            if (src == null)
                return null;

            InteractiveSourceClickableSpan span = new InteractiveSourceClickableSpan ();
            span.Click += v => action.Display (src.Text);

            return span;
        }

    }

    public class Source {

        public string Text { get; set; }

        public int StartIndex { get; set; }

        public Source (string text, int startIndex)
        {
            Text = text;
            StartIndex = startIndex;
        }

    }

    // adapted from https://forums.xamarin.com/discussion/15310/how-to-use-clickable-span
    public class InteractiveSourceClickableSpan : ClickableSpan {

        public Action<View> Click;

        public override void OnClick (View widget)
        {
            Click?.Invoke (widget);
        }

    }
}