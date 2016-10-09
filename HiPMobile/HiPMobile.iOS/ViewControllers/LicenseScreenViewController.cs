using Foundation;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CoreGraphics;
using MaterialControls;
using UIKit;

namespace HiPMobile.iOS
{
    public partial class LicenseScreenViewController : UIViewController, IMainScreenContainable
    {

        /// <summary>
        /// Number of linebreaks between two license entries.
        /// </summary>
        private int LinesBetweenEntries = 1;

        /// <summary>
        /// Style of links.
        /// </summary>
        private UIStringAttributes LinkAttributes;

        /// <summary>
        /// Style for headers.
        /// </summary>
        private UIStringAttributes TitleAttributes;

        /// <summary>
        /// Style for the whole body.
        /// </summary>
        private UIStringAttributes BodyAttributes;

        public string NavigationTitle
        {
            get
            {
                return "Lizenzen";
            }
        }

        public LicenseScreenViewController (IntPtr handle) : base (handle)
        {
            TitleAttributes = new UIStringAttributes()
            {
                Font = UIFont.PreferredHeadline
            };
            BodyAttributes = new UIStringAttributes()
            {
                Font = UIFont.PreferredBody.WithSize(14)
            };
            LinkAttributes = new UIStringAttributes()
            {
                Font = UIFont.PreferredBody.WithSize(14),
            };
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

            NSMutableAttributedString content = new NSMutableAttributedString();
            bool first = true;
            string interEntryLinebreaks = "";
            for (int i = 0; i < LinesBetweenEntries; i++)
            {
                interEntryLinebreaks += Environment.NewLine;
            }

            foreach (var licenseStringKey in Constants.LicenseStringKeys)
            {
                // Add the desired amout of linebreaks between entries, but not before first entry
                if (first)
                {
                    first = false;
                }
                else
                {
                    content.Append (new NSAttributedString(interEntryLinebreaks));
                }

                // Construct and add the entry
                var title = NSBundle.MainBundle.LocalizedString (licenseStringKey + "_title_text", "");
                var body = NSBundle.MainBundle.LocalizedString (licenseStringKey + "_body_text", "");
                content.Append (CreateEntry (title, body));
            }

            LicenseTextView.AttributedText = content;

            LicenseTextView.FlashScrollIndicators ();
        }


        public override void ViewDidLayoutSubviews ()
        {
            base.ViewDidLayoutSubviews ();

            // force the view to scroll to the top
            LicenseTextView.SetContentOffset (CGPoint.Empty, false);
        }

        /// <summary>
        /// Creates an attributed string with a headline and a body. the style conforms to the style set by the UIStringAttributes in this class.
        /// Hyperlinks are made clickable.
        /// </summary>
        /// <param name="headline">The text of the headline.</param>
        /// <param name="body">The text of the body. Can contain html formated links.</param>
        /// <returns>The attributed string.</returns>
        private NSMutableAttributedString CreateEntry (string headline, string body)
        {
            string rawEntry = headline + Environment.NewLine + body + Environment.NewLine;
            List<Tuple<NSRange, string>> linkRanges = null;
            string trimmedString = FindAndReplaceAllLinks (rawEntry, out linkRanges);

            NSMutableAttributedString formated = new NSMutableAttributedString(trimmedString);
            formated.SetAttributes(TitleAttributes, new NSRange(0, headline.Length));
            formated.SetAttributes(BodyAttributes,
                                    new NSRange(headline.Length + Environment.NewLine.Length, trimmedString.Length - headline.Length - 2 + Environment.NewLine.Length));
            foreach (Tuple<NSRange, string> linkRange in linkRanges)
            {
                LinkAttributes.Link = NSUrl.FromString (linkRange.Item2);
                formated.SetAttributes (LinkAttributes, linkRange.Item1);
            }
            
            return formated;
        }

        /// <summary>
        /// Converts a string with html like formated links to a string without html markup. The parts of the text that were hyperlinks are returned with their respective link as tuples.
        /// </summary>
        /// <param name="input">The string to be converted.</param>
        /// <param name="linkRanges">The ranges in the string that were links and their link.</param>
        /// <returns>The converted string.</returns>
        private string FindAndReplaceAllLinks (string input, out List<Tuple<NSRange, string>> linkRanges)
        {
            string output = "";
            linkRanges = new List<Tuple<NSRange, string>> ();

            Regex aElementRegex= new Regex (@"(<a.*?>.*?</a>)");
            input = Regex.Unescape (input);

            // search for all html tags with name a
            MatchCollection aOccurencies = aElementRegex.Matches (input);
            int outputIndex = 0;
            int inputIndex = 0;
            foreach (Match match in aOccurencies)
            {
                // extract the hyperlinks from this tags and write everything that isn't html markup to the output
                output += input.Substring (inputIndex, match.Index-inputIndex);
                outputIndex += match.Index - inputIndex;
                inputIndex += match.Index - inputIndex;
                string aElement = match.Value;
                string link = Regex.Match (aElement, "href=\".*?\"").Value;
                link = link.Replace ("href=\"", "");
                link = link.Replace ("\"", "");
                string linkText = Regex.Match (aElement, ">.*<").Value;
                linkText = linkText.Trim ('<', '>');

                inputIndex += aElement.Length;

                output += linkText;
                linkRanges.Add (Tuple.Create (new NSRange(outputIndex, linkText.Length), link));
                outputIndex += linkText.Length;
            }
            output += input.Substring (inputIndex);
            return output;
        }

    }
}