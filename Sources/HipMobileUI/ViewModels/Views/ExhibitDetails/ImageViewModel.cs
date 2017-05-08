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
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views.ExhibitDetails
{
    public class ImageViewModel : ExhibitSubviewHiddeableNavigationViewModel
    {

        private ImageSource image;
        private string description;
        private string headline;
        private bool bottomSheetVisible;

        public ImageViewModel (ImagePage page, Action toggleButtonVisibility) : base(toggleButtonVisibility)
        {
            var data = page.Image.Data;
            Image = ImageSource.FromStream (() => new MemoryStream (data));
            Headline = page.Image.Title;
            Description = page.Image.Description;
            BottomSheetVisible = Headline != "No Image" && !(string.IsNullOrEmpty (Headline) && string.IsNullOrEmpty (Description));
        }

        /// <summary>
        /// The displayed image of this view.
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

        // The description text.
        public string Description {
            get { return description; }
            set { SetProperty (ref description, value); }
        }

        /// <summary>
        /// Inidicates whether the bottomsheet is visible
        /// </summary>
        public bool BottomSheetVisible {
            get { return bottomSheetVisible; }
            set { SetProperty (ref bottomSheetVisible, value); }
        }

    }
}
