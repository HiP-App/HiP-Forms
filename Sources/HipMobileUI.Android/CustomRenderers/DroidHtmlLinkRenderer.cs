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

using Android.Content;
using Android.OS;
using Android.Text;
using Android.Text.Method;
using Android.Text.Util;
using PaderbornUniversity.SILab.Hip.Mobile.Droid.CustomRenderers;
using PaderbornUniversity.SILab.Hip.Mobile.Droid.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(HtmlLink), typeof(DroidHtmlLinkRenderer))]

namespace PaderbornUniversity.SILab.Hip.Mobile.Droid.CustomRenderers
{
    class DroidHtmlLinkRenderer : LabelRenderer
    {
        private HtmlLink formslink;

        public DroidHtmlLinkRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Label> elementChangedEventArgs)
        {
            base.OnElementChanged(elementChangedEventArgs);

            formslink = elementChangedEventArgs.NewElement as HtmlLink;

            if (formslink != null)
            {
                string html = new HtmlTagHelper().FormatAdditionalTags(formslink.HtmlText);

                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    Control.TextFormatted = Html.FromHtml(html, FromHtmlOptions.ModeLegacy);
                }
                else
                {
#pragma warning disable 618
                    Control.TextFormatted = Html.FromHtml(html);
#pragma warning restore 618
                }

                // Make links clickable
                Control.MovementMethod = LinkMovementMethod.Instance;
                Control.AutoLinkMask = MatchOptions.All;
                Control.Clickable = true;
                Control.LinksClickable = true;
            }
        }
    }
}