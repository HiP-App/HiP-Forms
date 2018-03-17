// Copyright (C) 2016 History in Paderborn App - Universität Paderborn
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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.DtoToModelConverters;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using Plugin.Geolocator.Abstractions;
using Xamarin.Forms;
using Image = PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.Image;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers
{
    public static class ExtensionMethods
    {
        public static ImageSource GetImageSource(this Image image)
        {
            var prepared = image.PrepareImageLoad();
            return new StreamImageSource
            {
                Stream = async token => new MemoryStream(await prepared.GetDataAsync() ?? BackupData.BackupImageData)
            };
        }

        /// <summary>
        /// Sort the observable collection accroding to the given function or comparer.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="collection"></param>
        /// <param name="keySelector"></param>
        /// <param name="comparer"></param>
        public static void SortCollection<TSource, TKey>(this ObservableCollection<TSource> collection, Func<TSource, TKey> keySelector, IComparer<TKey> comparer = null)
        {
            var sortedList = (comparer == null)
                ? collection.OrderBy(keySelector).ToList()
                : collection.OrderBy(keySelector, comparer).ToList();

            if (!collection.SequenceEqual(sortedList))
            {
                collection.Clear();
                foreach (var item in sortedList)
                    collection.Add(item);
            }
        }
        
        /// <summary>
        /// Converts the position to a geolocation.
        /// </summary>
        /// <param name="position">The position to convert</param>
        /// <returns>The corresponding geolocation.</returns>
        public static GeoLocation ToGeoLocation(this Position position) => 
            new GeoLocation(position.Latitude, position.Longitude);
    }
}