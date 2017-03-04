// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
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

using System;
using System.IO;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using Xamarin.Forms;

namespace HipMobileUI.ViewModels.Views.ExhibitDetails
{
    public class TextViewModel : ExhibitSubviewHiddeableNavigationViewModel
    {
        private string text;
        private string fontFamily;
        private string description;
        private string headline;

        public TextViewModel(TextPage page, Action toggleButtonVisibility) : base (toggleButtonVisibility)
        {
            Text = page.Text;
            FontFamily = page.FontFamily;
            Headline = page.Title;
            Description = page.Description;
        }

        /// <summary>
        /// Text of the page
        /// </summary>
        public string Text {
            get { return text; }
            set { SetProperty (ref text, value); }
        }

        /// <summary>
        /// Name of the font family to be used
        /// </summary>
        public string FontFamily
        {
            get { return fontFamily; }
            set { SetProperty(ref fontFamily, value); }
        }

        /// <summary>
        /// Name of the font family to be used
        /// </summary>
        public string Headline
        {
            get { return headline; }
            set { SetProperty (ref headline, value); }
        }

        /// <summary>
        /// Name of the font family to be used
        /// </summary>
        public string Description
        {
            get { return description; }
            set { SetProperty(ref description, value); }
        }
    }
}
