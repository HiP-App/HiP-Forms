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
using System.Linq;
using de.upb.hip.mobile.pcl.BusinessLayer.InteractiveSources;
using Foundation;
using UIKit;
using Xamarin.Forms;

namespace HipMobileUI.iOS.Helpers {
    public static class TextViewInteractiveSourcesExtension
    {

        /// <summary>
        /// Applies handling for subtitles to textview
        /// </summary>
        /// <param name="action">Action that should be started when the user interacts with the source.</param>
        /// <param name="sources">The parsed sources</param>
        /// <param name="textView">Textiew the SubtitleLinks should be attached to</param>
        public static void ApplySubtitlesLinks(this UITextView textView, IInteractiveSourceAction action, List<Source> sources)
        {
            if (action == null)
                return;

            Dictionary<Source, FinalSourcePosition> sourcePositions = new Dictionary<Source, FinalSourcePosition> ();

            foreach (var source in sources)
            {
                int startIndex = textView.Text.IndexOf (source.SubstituteText, StringComparison.Ordinal);
                sourcePositions.Add (source, new FinalSourcePosition
                {
                    Start = startIndex,
                    End = startIndex + source.SubstituteText.Length - 1
                });
            }

            textView.AddGestureRecognizer (new UITapGestureRecognizer(x => HandleTap(x, textView, sourcePositions, action)));
        }

        private static void HandleTap(UITapGestureRecognizer gestureRecognizer, UITextView textView, Dictionary<Source, FinalSourcePosition> sourcePositions, IInteractiveSourceAction action)
        {
            var point = gestureRecognizer.LocationInView(textView);
            point.Y += textView.ContentOffset.Y;

            var tapPos = textView.GetClosestPositionToPoint(point);

            nint clickedPos = textView.GetOffsetFromPosition(textView.BeginningOfDocument, tapPos);
            int pos = Convert.ToInt32 (clickedPos);
            var clickedSource = sourcePositions.SingleOrDefault (x => x.Value.Start <= pos && pos <= x.Value.End).Key;
            if (clickedSource != null)
            {
                action.Display(clickedSource);
            }

            /*string textRightOfClicked = textView.Text.Substring (pos);
            string textLeftOfClicked = textView.Text.Substring (0, pos + 1);

            if (string.IsNullOrEmpty (textRightOfClicked) || string.IsNullOrEmpty (textLeftOfClicked))
            {
                return;
            }

            if (textLeftOfClicked.Count(x => x == '[') > textLeftOfClicked.Count(x=> x == ']') ||
                textRightOfClicked.Count(x => x == ']') > textRightOfClicked.Count(x => x == '['))
            {
                string substituteText = textLeftOfClicked.Substring (textLeftOfClicked.LastIndexOf ('['))
                                        + textRightOfClicked.Substring (1, textRightOfClicked.IndexOf (']'));

                Source clickedSource = sources.Single(x => x.SubstituteText == substituteText);

                action.Display(clickedSource);
            }*/
        }

        private class FinalSourcePosition
        {
            public int Start { get; set; }

            public int End { get; set; }
        }
    }
}