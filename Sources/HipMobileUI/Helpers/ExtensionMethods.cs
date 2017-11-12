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
            var imageData = image.GetDataBlocking();
            if (imageData != null)
            {
                return ImageSource.FromStream(() => new MemoryStream(imageData));
            }
            else
            {
                return ImageSource.FromStream(() => new MemoryStream(BackupData.BackupImageData));
            }
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
            TSource[] sortedList;
            if (comparer == null)
                sortedList = collection.OrderBy(keySelector).ToArray();
            else
                sortedList = collection.OrderBy(keySelector, comparer).ToArray();
            if (!CompareCollectionToArray(collection, sortedList))
            {
                collection.Clear();
                foreach (var item in sortedList)
                    collection.Add(item);
            }
        }

        private static bool CompareCollectionToArray<T>(ObservableCollection<T> collection, T[] array)
        {
            if (collection.Count != array.Length)
            {
                return false;
            }

            for (int i = 0; i < collection.Count; i++)
            {
                if (!array[i].Equals(collection[i]))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Converts the position to a geolocation.
        /// </summary>
        /// <param name="position">The position to convert</param>
        /// <returns>The corresponding geolocation.</returns>
        public static GeoLocation ToGeoLocation(this Position position)
        {
            if (position != null)
            {
                return new GeoLocation(position.Latitude, position.Longitude);
            }
            return null;
        }
    }
}