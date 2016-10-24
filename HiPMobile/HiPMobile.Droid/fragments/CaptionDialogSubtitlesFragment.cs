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

using Android.Graphics;
using Android.OS;
using Android.Text.Method;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace de.upb.hip.mobile.droid.fragments
{
    public class CaptionDialogSubtitlesFragment : CaptionDialogFragment
    {
        public ICharSequence Subtitles { get; set; }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_exhibit_details_caption_dialog_subtitles, container, false);

            var subtitlesTextView = view.FindViewById<TextView>(Resource.Id.captionSubtitlesTextView);
            subtitlesTextView.TextFormatted = Subtitles;
            subtitlesTextView.MovementMethod = LinkMovementMethod.Instance;
            subtitlesTextView.SetHighlightColor(Color.Transparent);

            return view;
        }

    }
}