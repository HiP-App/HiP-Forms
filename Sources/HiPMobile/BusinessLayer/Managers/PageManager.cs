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

using Microsoft.EntityFrameworkCore;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.JoinClasses;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;
using System;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers
{
    /// <summary>
    /// Pagemanager class for getting access to pages.
    /// </summary>
    public static class PageManager
    {
        public static Instance Pages(this ITransactionDataAccess dataAccess) => new Instance(dataAccess);

        /// <summary>
        /// Explicitly loads all audio and images associated with the specified page.
        /// Additional information pages are loaded recursively in the same way.
        /// </summary>
        /// <param name="page"></param>
        public static void LoadPageDetails(Page page)
        {
            // Loading must happen in one DbContext scope so that
            // references between pages are correctly fixed up.
            using (var db = new AppDatabaseContext(QueryTrackingBehavior.TrackAll))
            {
                db.Attach(page);
                var pageEntry = db.Entry(page);
                pageEntry.Navigation(nameof(Page.Audio)).Load();

                switch (page)
                {
                    case ImagePage imagePage:
                        db.Entry(imagePage).Navigation(nameof(ImagePage.Image)).Load();
                        break;

                    case TimeSliderPage timeSliderPage:
                        pageEntry.Navigation(nameof(TimeSliderPage.SliderImages)).Load();
                        foreach (var img in timeSliderPage.SliderImages)
                            db.Entry(img).Navigation(nameof(TimeSliderPageImage.Image)).Load();
                        break;
                }

                var subpageNav = pageEntry.Navigation(nameof(Page.AdditionalInformationPagesRefs));

                // Recursively load additional information pages
                // (Note: page references may be circular - this check breaks the recursion)
                if (!subpageNav.IsLoaded) 
                {
                    subpageNav.Load();
                    foreach (var subpageRef in page.AdditionalInformationPagesRefs)
                    {
                        db.Entry(subpageRef).Navigation(nameof(JoinPagePage.AdditionalInformationPage)).Load();
                        LoadPageDetails(subpageRef.AdditionalInformationPage);
                    }
                }
            }
        }

        public struct Instance
        {
            private readonly ITransactionDataAccess dataAccess;

            public Instance(ITransactionDataAccess dataAccess)
            {
                this.dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
            }

            /// <summary>
            /// Gets an imagepage with a specific id.
            /// </summary>
            /// <param name="id">The id of the imagepage to be retrived.</param>
            /// <returns>The imagepage with the given id. If it doesn't exist, null is returned.</returns>
            public ImagePage GetImagePage(string id)
            {
                if (!string.IsNullOrEmpty(id))
                {
                    return dataAccess.GetItem<ImagePage>(id);
                }
                return null;
            }

            /// <summary>
            /// Gets an TextPage with a specific id.
            /// </summary>
            /// <param name="id">The id of the TextPage to be retrived.</param>
            /// <returns>The TextPage with the given id. If it doesn't exist, null is returned.</returns>
            public TextPage GetTextPage(string id)
            {
                if (!string.IsNullOrEmpty(id))
                {
                    return dataAccess.GetItem<TextPage>(id);
                }
                return null;
            }

            /// <summary>
            /// Gets an TimeSliderPage with a specific id.
            /// </summary>
            /// <param name="id">The id of the TimeSliderPage to be retrived.</param>
            /// <returns>The TimeSliderPage with the given id. If it doesn't exist, null is returned.</returns>
            public TimeSliderPage GetTimesliderPage(string id)
            {
                if (!string.IsNullOrEmpty(id))
                {
                    return dataAccess.GetItem<TimeSliderPage>(id);
                }
                return null;
            }
        }
    }
}