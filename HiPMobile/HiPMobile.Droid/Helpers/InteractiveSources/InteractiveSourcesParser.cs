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
using Java.Lang;
using Java.Util.Regex;


namespace de.upb.hip.mobile.droid.Helpers.InteractiveSources
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

        private List<Source> Sources { get; }

        private string Text { get; set; }

        /// <summary>
        /// Creates a parser for the specified text with source markup.
        /// </summary>
        /// <param name="text">Text to parse.</param>
        /// <param name="substitute">Substitute that replaces the source markdown.</param>
        public InteractiveSourcesParser(string text, IInteractiveSourceSubstitute substitute)
        {
            Sources = new List<Source>();
            Text = null;

            Parse(text, substitute);
        }

        /// <summary>
        /// Parses the specified text with source markup (according to set start and end tags) as preperation
        /// for getting subtitles and references texts.
        /// </summary>
        /// <param name="text">Text to parse.</param>
        /// <param name="substitute">Substitute that replaces the source markdown.</param>
        private void Parse(string text, IInteractiveSourceSubstitute substitute)
        {
            if (text == null || substitute == null)
                return;

            Text = text;

            // TODO: use C# regexp?
            Pattern pattern = Pattern.Compile(HtmlStartTag + ".+?" + HtmlEndTag);
            Matcher matcher = pattern.Matcher(Text);

            while (matcher.Find())
            {
                string match = matcher.Group();

                string srcText = match;
                srcText = srcText.Replace(HtmlStartTag, string.Empty);
                srcText = srcText.Replace(HtmlEndTag, string.Empty);

                string sub = substitute.NextSubstitute();

                Sources.Add(new Source(srcText, matcher.Start(), sub));

                // replace footnote markup with substitute
                Text = Text.Replace(match, sub);
                matcher = pattern.Matcher(Text); // working with new text to get correct start index
            }
        }

        /// <summary>
        /// Returns a SpannableString containing interactive sources for subtitles.
        /// <see cref="Parse"/> must be called before.
        /// </summary>
        /// <param name="action">Action that should be started when the user interacts with the source.</param>
        /// <returns>A SpannableString parsed from the text for the subtitles. Returns null, if the provided action is null.</returns>
        public SpannableString CreateSubtitlesText(IInteractiveSourceAction action)
        {
            if (action == null)
                return null;

            var str = new SpannableString(Text);

            foreach (var src in Sources)
            {
                if (src == null)
                    continue;

                str.SetSpan(
                    ConvertSrcToClickableSpan(src, action),
                    src.StartIndex,
                    src.StartIndex + src.SubstituteText.Length,
                    SpanTypes.ExclusiveExclusive);
            }

            return str;
        }

        /// <summary>
        /// Returns a SpannableString containing all references of the parsed text. The references headers
        /// are highlighted with the provided color.
        /// </summary>
        /// <param name="colorForHighlighting">Color which is used for highlighting the references headers.</param>
        /// <returns>A SpannableString parsed from the text for the references.</returns>
        public SpannableString CreateReferencesText(Android.Graphics.Color colorForHighlighting)
        {
            var referencesTextBuilder = new StringBuilder();

            for (int i = 0; i < Sources.Count; i++)
            {
                referencesTextBuilder.Append(Sources[i].SubstituteText);
                referencesTextBuilder.Append(":");
                referencesTextBuilder.Append(Environment.NewLine);
                referencesTextBuilder.Append(Sources[i].Text);
                if (i != Sources.Count - 1)
                {
                    referencesTextBuilder.Append(Environment.NewLine);
                    referencesTextBuilder.Append(Environment.NewLine);
                }
            }

            string referencesText = referencesTextBuilder.ToString();
            var spannableReferences = new SpannableString(referencesText);

            foreach (var reference in Sources)
            {
                var index = referencesText.IndexOf(reference.SubstituteText, StringComparison.InvariantCulture);

                spannableReferences.SetSpan(new ForegroundColorSpan(colorForHighlighting), index, index + reference.SubstituteText.Length + 1, SpanTypes.ExclusiveExclusive);
            }

            return spannableReferences;
        }

        /// <summary>
        /// Converts a source to a ClickableSpan that executes the specified action.
        /// </summary>
        /// <param name="src">Source that should be converted to a ClickableSpan.</param>
        /// <param name="action">Action that should be executed when the Click-event of the span is triggered.</param>
        /// <returns>ClickableSpan for the provided Source instance.</returns>
        private static ClickableSpan ConvertSrcToClickableSpan(Source src, IInteractiveSourceAction action)
        {
            if (src == null)
                return null;

            InteractiveSourceClickableSpan span = new InteractiveSourceClickableSpan();
            span.Click += v => action.Display(src.Text);

            return span;
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