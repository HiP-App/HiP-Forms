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
    public class AppetizerViewModel : ExhibitSubviewViewModel
    {

        private ImageSource image;
        private string text;
        private string headline;

        public AppetizerViewModel (string exhibitName, AppetizerPage page)
        {
            if (page != null)
            {
                Headline = exhibitName;
                Text = page.Text;

                // workaround for realm bug
                var imageData = page.Image.Data;
                Image = ImageSource.FromStream (() => new MemoryStream (imageData));
            }
        }

        /// <summary>
        /// The appetizer image.
        /// </summary>
        public ImageSource Image {
            get { return image; }
            set { SetProperty (ref image, value); }
        }

        /// <summary>
        /// The headline of the description.
        /// </summary>
        public string Headline {
            get { return headline; }
            set { SetProperty (ref headline, value); }
        }

        /// <summary>
        /// The text of the description.
        /// </summary>
        public string Text {
            get { return text; }
            set { SetProperty (ref text, value); }
        }

    }
}
