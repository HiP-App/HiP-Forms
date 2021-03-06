﻿// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
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
using PaderbornUniversity.SILab.Hip.Mobile.Ios.Helpers;
using System;
using System.ComponentModel;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Views;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ReferenceLink), typeof(IosReferenceLinkRenderer))]

namespace PaderbornUniversity.SILab.Hip.Mobile.Ios.CustomRenderers
{
    public class IosReferenceLinkRenderer : ViewRenderer
    {
        private ReferenceLink referenceLink;
        private UITextView textView;

        protected override void OnElementChanged(ElementChangedEventArgs<View> elementChangedEventArgs)
        {
            base.OnElementChanged(elementChangedEventArgs);

            if (elementChangedEventArgs.NewElement != null)
            {
                referenceLink = (ReferenceLink) elementChangedEventArgs.NewElement;

                if (referenceLink == null)
                {
                    return;
                }
                referenceLink.PropertyChanged += OnPropertyChanged;

                NSAttributedString attributedString = null;
                var htmlData = NSData.FromString(referenceLink.HtmlText);

                if (htmlData != null)
                {
                    NSError error = new NSError();
                    attributedString =
                        new NSAttributedString(htmlData, new NSAttributedStringDocumentAttributes { DocumentType = NSDocumentType.HTML, StringEncoding = NSStringEncoding.UTF8 },
                                               ref error);
                }

                var action = referenceLink.Action;

                textView = new UITextView();
                textView.TextColor = UIColor.Gray;
                textView.ApplySubtitlesLinks(action(), referenceLink.Sources, attributedString);
                textView.Editable = false;
                textView.ScrollEnabled = false;
                textView.Font = UIFont.SystemFontOfSize((nfloat) referenceLink.FontSize);

                // replace old Label with new TextView
                SetNativeControl(textView);
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "FontSize")
            {
                textView.Font = UIFont.SystemFontOfSize((nfloat) referenceLink.FontSize);
            }
        }
    }
}