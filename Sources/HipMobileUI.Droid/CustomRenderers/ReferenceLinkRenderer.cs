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

using Android.Text;
using de.upb.hip.mobile.droid.CustomRenderers;
using de.upb.hip.mobile.droid.Helpers;
using HipMobileUI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ReferenceLink), typeof(ReferenceLinkRenderer))]
namespace de.upb.hip.mobile.droid.CustomRenderers
{
    public class ReferenceLinkRenderer : LabelRenderer
    {
        private ReferenceLink referencelink;

        protected override void OnElementChanged(ElementChangedEventArgs<Label> elementChangedEventArgs)
        {
            base.OnElementChanged(elementChangedEventArgs);

            if (elementChangedEventArgs.NewElement != null)
            {
                referencelink = (ReferenceLink)elementChangedEventArgs.NewElement;
                var srcList = referencelink.Sources;
                var formatedText = referencelink.Text;
                var spannableTextBuilder = new SpannableTextBuilder();

                var formattedSubtitles = spannableTextBuilder.CreateSubtitlesText(null, formatedText, srcList);

                Control.TextFormatted = formattedSubtitles;

                // Make links clickable
                /*Control.MovementMethod = LinkMovementMethod.Instance;
                Control.AutoLinkMask = MatchOptions.All;
                Control.Clickable = true;
                Control.LinksClickable = true;*/
            }
        }
    }
}