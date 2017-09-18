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

using System.Runtime.InteropServices.ComTypes;
using NUnit.Framework;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers {
    /// <summary>
    /// Pagemanager class for getting access to pages.
    /// </summary>
    public static class PageManager {

        private static readonly IDataAccess DataAccess = IoCManager.Resolve<IDataAccess>();

        /// <summary>
        /// Gets an imagepage with a specific id.
        /// </summary>
        /// <param name="id">The id of the imagepage to be retrived.</param>
        /// <returns>The imagepage with the given id. If it doesn't exist, null is returned.</returns>
        public static ImagePage GetImagePage (string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                return DataAccess.GetItem<ImagePage>(id);
            }
            return null;
        }

        /// <summary>
        /// Gets an TextPage with a specific id.
        /// </summary>
        /// <param name="id">The id of the TextPage to be retrived.</param>
        /// <returns>The TextPage with the given id. If it doesn't exist, null is returned.</returns>
        public static TextPage GetTextPage(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                return DataAccess.GetItem<TextPage>(id);
            }
            return null;
        }

        /// <summary>
        /// Gets an TimeSliderPage with a specific id.
        /// </summary>
        /// <param name="id">The id of the TimeSliderPage to be retrived.</param>
        /// <returns>The TimeSliderPage with the given id. If it doesn't exist, null is returned.</returns>
        public static TimeSliderPage GetTimesliderPage(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                return DataAccess.GetItem<TimeSliderPage>(id);
            }
            return null;
        }

        public static bool DeletePage (Page page)
        {
            switch (page.PageType)
            {
                case PageType.AppetizerPage:
                    DeleteAppetizerPage (page.AppetizerPage);
                    break;
                case PageType.ImagePage:
                    DeleteImagePage (page.ImagePage);
                    break;
                case PageType.TextPage:
                    DeleteTextPage (page.TextPage);
                    break;
                case PageType.TimeSliderPage:
                    DeleteTimeSliderPage (page.TimeSliderPage);
                    break;
            }

            if (page.Audio != null)
            {
                DataAccess.DeleteItem<Audio> (page.Audio.Id);
            }

            return DataAccess.DeleteItem<Page> (page.Id);
        }

        public static bool DeleteAppetizerPage (AppetizerPage appetizerPage)
        {
            return true;
        }

        public static bool DeleteImagePage(ImagePage imagePage)
        {
            if (imagePage != null)
            {
                Image image = imagePage.Image;

                if (image != null)
                {
                    DataAccess.DeleteItem<Image> (image.Id);
                    DataAccess.DeleteItem<ImagePage> (imagePage.Id);
                }
            }

            return true;
        }

        public static bool DeleteTextPage(TextPage textPage)
        {
            if (textPage != null)
            {
                DataAccess.DeleteItem<TextPage> (textPage.Id);
            }

            return true;
        }

        public static bool DeleteTimeSliderPage(TimeSliderPage timeSliderPage)
        {
            if (timeSliderPage != null)
            {
                var images = timeSliderPage.Images;

                foreach (var image in images)
                {
                    if (image != null)
                    {
                        DataAccess.DeleteItem<Image> (image.Id);
                    }
                }

                DataAccess.DeleteItem<TimeSliderPage> (timeSliderPage.Id);
            }

            return true;
        }
    }
}