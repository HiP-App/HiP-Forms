// Copyright (C) 2016 History in Paderborn App - Universität Paderborn
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

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models
{
    public partial class Page
    {
        /// <summary>
        /// Returns the type of the page
        /// </summary>
        public PageType PageType
        {
            get
            {
                if (IsAppetizerPage())
                {
                    return PageType.AppetizerPage;
                }
                else if (IsImagePage())
                {
                    return PageType.ImagePage;
                }
                else if (IsTextPage())
                {
                    return PageType.TextPage;
                }
                else if (IsTimeSliderPage())
                {
                    return PageType.TimeSliderPage;
                }
                else
                {
                    throw new Exception("Unknown page found: " + this);
                }
            }
        }

        public bool IsAppetizerPage()
        {
            if (AppetizerPage != null)
            {
                return true;
            }
            return false;
        }

        public bool IsTextPage()
        {
            if (TextPage != null)
            {
                return true;
            }
            return false;
        }

        public bool IsTimeSliderPage()
        {
            if (TimeSliderPage != null)
            {
                return true;
            }
            return false;
        }

        public bool IsImagePage()
        {
            if (ImagePage != null)
            {
                return true;
            }
            return false;
        }
    }
}