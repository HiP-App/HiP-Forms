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

using Android.OS;
using Android.Text;
using Android.Text.Method;
using Android.Text.Util;
using de.upb.hip.mobile.droid.CustomRenderers;
using de.upb.hip.mobile.droid.Helpers;
using HipMobileUI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ReferenceLink), typeof(DroidReferenceLinkRenderer))]
namespace de.upb.hip.mobile.droid.CustomRenderers
{
    public class DroidReferenceLinkRenderer : LabelRenderer
    {
        private ReferenceLink referenceLink;

        protected override void OnElementChanged(ElementChangedEventArgs<Label> elementChangedEventArgs)
        {
            base.OnElementChanged(elementChangedEventArgs);

            if (elementChangedEventArgs.NewElement != null)
            {
                referenceLink = (ReferenceLink)elementChangedEventArgs.NewElement;
                if (referenceLink == null)
                {
                    return;
                }
                var srcList = referenceLink.Sources;
                var action = referenceLink.Action;
                var spannableTextBuilder = new SpannableTextBuilder();

                ISpanned spannedText;

                string html = new HtmlTagHelper().FormatAdditionalTags(referenceLink.HtmlText);

                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    spannedText = Html.FromHtml(html, FromHtmlOptions.ModeLegacy);
                }
                else
                {
#pragma warning disable 618
                    spannedText = Html.FromHtml(html);
#pragma warning restore 618
                }

                Control.TextFormatted = spannableTextBuilder.CreateSubtitlesText(action(), spannedText, srcList);

                // Make links clickable
                Control.MovementMethod = LinkMovementMethod.Instance;
                Control.AutoLinkMask = MatchOptions.All;
                Control.Clickable = true;
                Control.LinksClickable = true;
            }
        }
    }
}