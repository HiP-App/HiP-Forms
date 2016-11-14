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
using de.upb.hip.mobile.pcl.BusinessLayer.InteractiveSources;

namespace de.upb.hip.mobile.droid.Helpers.InteractiveSources {
    public class SpannableTextBuilder {

        /// <summary>
        /// Returns a SpannableString containing interactive sources for subtitles.
        /// </summary>
        /// <param name="action">Action that should be started when the user interacts with the source.</param>
        /// <param name="textWithSubstitutes">The parsed text with substitutes</param>
        /// <param name="sources">The parsed sources</param>
        /// <returns>A SpannableString parsed from the textWithSubstitutes for the subtitles. Returns null, if the provided action is null.</returns>
        public SpannableString CreateSubtitlesText(IInteractiveSourceAction action, string textWithSubstitutes, List<Source> sources)
        {
            if (action == null)
                return null;

            var str = new SpannableString(textWithSubstitutes);

            foreach (var src in sources)
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
            span.Click += v => action.Display(src);

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