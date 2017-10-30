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

using Foundation;
using PaderbornUniversity.SILab.Hip.Mobile.Ios.CustomRenderers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Views;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(HtmlLink), typeof(IosHtmlLinkRenderer))]

namespace PaderbornUniversity.SILab.Hip.Mobile.Ios.CustomRenderers
{
    public class IosHtmlLinkRenderer : ViewRenderer
    {
        private HtmlLink formslink;

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            formslink = e.NewElement as HtmlLink;
            if (formslink != null)
            {
                NSAttributedString attributedString = null;
                var htmlData = NSData.FromString(formslink.HtmlText);

                if (htmlData != null)
                {
                    NSError error = new NSError();
                    attributedString =
                        new NSAttributedString(htmlData, new NSAttributedStringDocumentAttributes { DocumentType = NSDocumentType.HTML, StringEncoding = NSStringEncoding.UTF8 },
                                               ref error);
                }

                UITextView licenseTextView = new UITextView
                {
                    AttributedText = attributedString,
                    Editable = false,
                    ScrollEnabled = false,
                    Font = UIFont.SystemFontOfSize(14),
                    TextColor = UIColor.Gray
                };

                // replace old Label with new TextView
                SetNativeControl(licenseTextView);
            }
        }
    }
}