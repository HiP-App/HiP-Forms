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
using de.upb.hip.mobile.pcl.BusinessLayer.InteractiveSources;
using Foundation;
using UIKit;
using Xamarin.Forms;

namespace HipMobileUI.iOS.Helpers {
    public class SpannableTextBuilder
    {
        /// <summary>
        /// Returns a SpannableString containing interactive sources for subtitles.
        /// </summary>
        /// <param name="action">Action that should be started when the user interacts with the source.</param>
        /// <param name="textWithSubstitutes">The parsed text with substitutes</param>
        /// <param name="sources">The parsed sources</param>
        /// <returns>A SpannableString parsed from the textWithSubstitutes for the subtitles. Returns null, if the provided action is null.</returns>
        public UITextView CreateSubtitlesText(IInteractiveSourceAction action, string textWithSubstitutes, List<Source> sources)
        {
            if (action == null)
                return null;

            var textView = new UITextView();
            textView.Text = textWithSubstitutes;
            textView.AddGestureRecognizer (new UITapGestureRecognizer(x => HandleTap(x, textView, sources, action)));
            return textView;
        }

        public void HandleTap(UITapGestureRecognizer gestureRecognizer, UITextView textView, List<Source> sources, IInteractiveSourceAction action)
        {
            var point = gestureRecognizer.LocationInView(textView);
            point.Y += textView.ContentOffset.Y;

            var tapPos = textView.GetClosestPositionToPoint(point);
            //fetch the word at this position (or nil, if not available)
            UITextInputStringTokenizer tokenizer = new UITextInputStringTokenizer (textView);
            var wordRange = tokenizer.GetRangeEnclosingPosition (tapPos, UITextGranularity.Word, UITextDirection.Right);
            if (wordRange == null)
            {
                return;
            }
            var startPosition = textView.GetOffsetFromPosition (textView.BeginningOfDocument, wordRange.Start);

            foreach (var src in sources)
            {
                if (src == null)
                    continue;

                if (src.StartIndex == startPosition - 1 || src.StartIndex == startPosition - 8)
                {
                    action.Display (src);
                    break;
                }
            }
        }
    }
}