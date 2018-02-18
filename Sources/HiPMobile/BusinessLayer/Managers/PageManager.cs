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

using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
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

        public struct Instance
        {
            private readonly ITransactionDataAccess _dataAccess;

            public Instance(ITransactionDataAccess dataAccess)
            {
                _dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
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
                    return _dataAccess.GetItem<ImagePage>(id);
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
                    return _dataAccess.GetItem<TextPage>(id);
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
                    return _dataAccess.GetItem<TimeSliderPage>(id);
                }
                return null;
            }
        }
    }
}