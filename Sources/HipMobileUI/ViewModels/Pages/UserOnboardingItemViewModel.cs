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

using HipMobileUI.Helpers;
using Xamarin.Forms;

namespace HipMobileUI.ViewModels.Pages
{
    class UserOnboardingItemViewModel : NavigationViewModel
    {

        public UserOnboardingItemViewModel (string headline, string text, string portraitImage, Color background, string landscapeImage=null)
        {
            Headline = headline;
            Text = text;
            Image = ImageSource.FromFile (portraitImage);
            BackgroundColor = background;
            portraitImagePath = portraitImage;
            landscapeImagePath = landscapeImage;
        }

        private string headline;
        private string text;
        private ImageSource image;
        private Color backgroundColor;

        private readonly string portraitImagePath;
        private readonly string landscapeImagePath;

        public string Headline {
            get { return headline; }
            set { SetProperty (ref headline, value); }
        }

        public string Text {
            get { return text; }
            set { SetProperty (ref text, value); }
        }

        public ImageSource Image {
            get { return image; }
            set { SetProperty (ref image, value); }
        }

        public Color BackgroundColor {
            get { return backgroundColor; }
            set { SetProperty (ref backgroundColor, value); }
        }

        public void OrientationChanged (DeviceOrientation orientation)
        {
            if (orientation == DeviceOrientation.Portrait)
            {
                Image = ImageSource.FromFile (portraitImagePath);
            }
            else if(orientation == DeviceOrientation.Landscape && !string.IsNullOrEmpty (landscapeImagePath))
            {
                Image = ImageSource.FromFile (landscapeImagePath);
            }
        }

    }
}
