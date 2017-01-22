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
using System.Text.RegularExpressions;
using Foundation;
using HipMobileUI.iOS;
using HipMobileUI.Views;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Link), typeof(LinkRenderer))]
namespace HipMobileUI.iOS
{
    public class LinkRenderer : LabelRenderer
    {
        private Link formslink;

        protected override void OnElementChanged(ElementChangedEventArgs<Label> elementChangedEventArgs)
        {
            base.OnElementChanged(elementChangedEventArgs);

            if (elementChangedEventArgs.NewElement != null)
            {
                formslink = (Link)elementChangedEventArgs.NewElement;

                NSMutableAttributedString content = new NSMutableAttributedString();
                List<Tuple<NSRange, string>> linkRanges = new List<Tuple<NSRange, string>>();
                string output = "";

                Regex aElementRegex = new Regex(@"(<a.*?>.*?</a>)");
                string input = formslink.Text;
                input = Regex.Unescape(input);

                // search for all html tags with name a
                MatchCollection aOccurencies = aElementRegex.Matches(input);
                int outputIndex = 0;
                int inputIndex = 0;
                foreach (Match match in aOccurencies)
                {
                    // extract the hyperlinks from this tags and write everything that isn't html markup to the output
                    output += input.Substring(inputIndex, match.Index - inputIndex);
                    outputIndex += match.Index - inputIndex;
                    inputIndex += match.Index - inputIndex;
                    string aElement = match.Value;
                    string link = Regex.Match(aElement, "href=\".*?\"").Value;
                    link = link.Replace("href=\"", "");
                    link = link.Replace("\"", "");
                    string linkText = Regex.Match(aElement, ">.*<").Value;
                    linkText = linkText.Trim('<', '>');

                    inputIndex += aElement.Length;

                    output += linkText;
                    linkRanges.Add(Tuple.Create(new NSRange(outputIndex, linkText.Length), link));
                    outputIndex += linkText.Length;
                }
                output += input.Substring(inputIndex);

                NSMutableAttributedString formated = new NSMutableAttributedString(output);
                UIStringAttributes linkAttributes = new UIStringAttributes()
                {
                    Font = UIFont.PreferredBody.WithSize(14),
                };
                foreach (Tuple<NSRange, string> linkRange in linkRanges)
                {
                    linkAttributes.Link = NSUrl.FromString(linkRange.Item2);
                    formated.SetAttributes(linkAttributes, linkRange.Item1);
                }

                Control.AttributedText = formated;
                Control.UserInteractionEnabled = true;
                
            }
        }
    }
}