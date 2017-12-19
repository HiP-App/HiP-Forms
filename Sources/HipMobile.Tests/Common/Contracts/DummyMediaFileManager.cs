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
using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;

namespace PaderbornUniversity.SILab.Hip.Mobile.HipMobileTests.Common.Contracts
{
    public class DummyMediaFileManager : IMediaFileManager
    {
        private readonly IDictionary<string, byte[]> memCache = new Dictionary<string, byte[]>();

        public long TotalSizeBytes => 0;

        public void DeleteFile(string filePath)
        {
            memCache.Remove(filePath);
        }

        public Task<string> WriteMediaToDiskAsync(byte[] bytes, int restApiId, DateTimeOffset timestamp)
        {
            memCache[restApiId.ToString()] = bytes;
            return Task.FromResult(restApiId.ToString());
        }

        public Task<byte[]> ReadFromDiskAsync(string filePath)
        {
            return Task.FromResult(memCache[filePath]);
        }

        public byte[] ReadFromDisk(string filePath)
        {
            return memCache.TryGetValue(filePath, out var value) ? value : new byte[] { 1, 2, 3, 4 };
        }

        public Task PruneAsync(IList<int> restApiIdsToKeep)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ContainsMedia(int restApiId, DateTimeOffset timestamp)
        {
            throw new NotImplementedException();
        }

        public string PathForRestApiId(int restApiId)
        {
            throw new NotImplementedException();
        }
    }
}