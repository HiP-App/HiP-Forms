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

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models
{
    public class ImagePage : Page
    {
        [NotMapped] // purpose of this property is unclear
        public IList<string> Texts { get; }

        [NotMapped] // purpose of this property is unclear
        public IList<Rectangle> Areas { get; }

        public Image Image { get; set; }

        /// <remarks>
        /// HACK: This property shouldn't be necessary. If it's not defined, EF Core creates
        /// a "shadow property" with the same name. But for some reason, explicitly loading the
        /// navigation property "Image" in <see cref="Managers.PageManager.LoadPageDetails(Page)"/>
        /// fails if this property is not defined. This seems to be an EF Core bug. In the future,
        /// try updating EF Core, remove this property and see if it works.
        /// </remarks>
        public string ImageId { get; set; }

        public ImagePage()
        {
        }
    }
}

