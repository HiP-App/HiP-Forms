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

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models
{
    static class MediaCache
    {
        private const double MaxWeightBytes = 64 * 1024 * 1024;

        private static readonly WeightConstrainedLruCache<int, byte[]> Cache =
            new WeightConstrainedLruCache<int, byte[]>((mediaId, bytes) => bytes.Length, MaxWeightBytes);

        /// <summary>
        /// Get the bytes of a media file with the specified REST ID.
        /// </summary>
        /// <param name="mediaId"></param>
        /// <param name="computer">If the value is not present in the LRU cache, this function is called to insert them.</param>
        /// <returns></returns>
        public static async Task<byte[]> GetBytesAsync(int mediaId, Func<Task<byte[]>> computer) =>
            await Cache.GetAsync(mediaId, async mediaIdKey => await computer());

        /// <summary>
        /// Get the bytes of a media file with the specified REST ID.
        /// </summary>
        /// <param name="mediaId"></param>
        /// <param name="computer">If the value is not present in the LRU cache, this function is called to insert them.</param>
        /// <returns></returns>
        public static byte[] GetBytes(int mediaId, Func<byte[]> computer) =>
            Cache.GetSync(mediaId, mediaIdKey => computer());
    }

    /// <summary>
    /// Thread-safe LRU cache.
    /// </summary>
    /// <typeparam name="TK"></typeparam>
    /// <typeparam name="TV"></typeparam>
    class WeightConstrainedLruCache<TK, TV>
    {
        /// <summary>
        /// Called to compute the weight (e.g. bytes) of the key-value pair.
        /// This function must always return the same value for the same key-value pair.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public delegate double WeightOfDelegate(TK key, TV value);

        private readonly SemaphoreSlim sema = new SemaphoreSlim(1);

        private readonly IDictionary<TK, TV> store = new Dictionary<TK, TV>();
        private readonly IList<TK> lruList = new List<TK>();

        private readonly WeightOfDelegate weightOf;
        private readonly double maxWeight;
        private double totalWeight;

        /// <summary>
        /// </summary>
        /// <param name="weightOf">See <see cref="WeightOfDelegate"/></param>
        /// <param name="maxWeight">If the cache's total weight exceeds this weight, least-recently used objects will be evicted until there is enough room.</param>
        public WeightConstrainedLruCache(WeightOfDelegate weightOf, double maxWeight)
        {
            this.weightOf = weightOf;
            this.maxWeight = maxWeight;
        }

        private bool TryGet(TK key, out TV value)
        {
            if (store.TryGetValue(key, out value))
            {
                lruList.Remove(key);
                lruList.Insert(0, key);
                return true;
            }
            return false;
        }

        private void Insert(TK key, TV value)
        {
            var weight = weightOf(key, value);
            while (totalWeight + weight > maxWeight)
            {
                if (lruList.Count == 0)
                {
                    throw new ArgumentException($"Element ({key}:{value}) has Weight={totalWeight} > MaxWeight={maxWeight}");
                }

                var oldestKey = lruList[lruList.Count - 1];
                store.TryGetValue(oldestKey, out var oldestValue);
                lruList.RemoveAt(lruList.Count - 1);
                store.Remove(oldestKey);
                totalWeight -= weightOf(oldestKey, oldestValue);
            }
            store.Add(key, value);
            lruList.Insert(0, key);
            totalWeight += weight;
        }

        /// <summary>
        /// Get the value associated with the key from the cache. If it has not been inserted
        /// yet or evicted, the supplied function is called to compute it.
        /// </summary>
        public TV GetSync(TK key, Func<TK, TV> computer)
        {
            sema.Wait();
            try
            {
                if (TryGet(key, out var storeValue))
                {
                    return storeValue;
                }

                var value = computer(key);
                Insert(key, value);
                return value;
            }
            finally
            {
                sema.Release();
            }
        }

        /// <summary>
        /// Get the value associated with the key from the cache. If it has not been inserted
        /// yet or evicted, the supplied function is called to compute it.
        /// </summary>
        public async Task<TV> GetAsync(TK key, Func<TK, Task<TV>> computer)
        {
            await sema.WaitAsync();
            try
            {
                if (TryGet(key, out var storeValue))
                {
                    return storeValue;
                }

                var value = await computer(key);
                Insert(key, value);
                return value;
            }
            finally
            {
                sema.Release();
            }
        }
    }
}