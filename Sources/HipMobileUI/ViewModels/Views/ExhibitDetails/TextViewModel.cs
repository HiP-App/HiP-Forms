﻿// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
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
using System.IO;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using Xamarin.Forms;

namespace HipMobileUI.ViewModels.Views.ExhibitDetails
{
    public class TextViewModel : ExhibitSubviewViewModel
    {
        private string text;
        private string fontFamily;

        public TextViewModel(TextPage page)
        {
            Text = page.Text;
            FontFamily = page.FontFamily;
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
            set { SetProperty (ref fontFamily, value); }
        }

    }
}
