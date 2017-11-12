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
        private static readonly WeightConstrainedLruCache<string, byte[]> Cache =
            new WeightConstrainedLruCache<string, byte[]>((md5, bytes) => bytes.Length, MaxWeightBytes);

        public static async Task<byte[]> GetBytesFor(string md5, Func<Task<byte[]>> computer) => await Cache.Get(md5, async md5Key => await computer());
    }

    class WeightConstrainedLruCache<TK, TV>
    {
        public delegate double WeightOfDelegate(TK key, TV value);
        
        private readonly SemaphoreSlim sema = new SemaphoreSlim(1);

        private readonly IDictionary<TK, TV> store = new Dictionary<TK, TV>();
        private readonly IList<TK> lruList = new List<TK>();
        
        private readonly WeightOfDelegate weightOf;
        private readonly double maxWeight;
        private double totalWeight;

        public WeightConstrainedLruCache(WeightOfDelegate weightOf, double maxWeight)
        {
            this.weightOf = weightOf;
            this.maxWeight = maxWeight;
        }

        public async Task<TV> Get(TK key, Func<TK, Task<TV>> computer)
        {
            await sema.WaitAsync();

            try
            {
                if (store.TryGetValue(key, out var storeValue))
                {
                    lruList.Remove(key);
                    lruList.Insert(0, key);
                    return storeValue;
                }

                var value = await computer(key);
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

                return value;
            }
            finally
            {
                sema.Release();
            }
        }
    }
}