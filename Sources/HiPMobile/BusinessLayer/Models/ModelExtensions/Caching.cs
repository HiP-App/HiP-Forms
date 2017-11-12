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

        public static async Task<byte[]> GetBytesAsync(string md5, Func<Task<byte[]>> computer) => await Cache.GetAsync(md5, async md5Key => await computer());
        public static byte[] GetBytes(string md5, Func<byte[]> computer) => Cache.GetSync(md5, md5Key => computer());
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