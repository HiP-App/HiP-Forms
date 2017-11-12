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
using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models
{
    public partial class Audio
    {
        private readonly IMediaFileManager fileManager = IoCManager.Resolve<IMediaFileManager>();

        public async Task<byte[]> GetDataAsync() => await MediaCache.GetBytesAsync(
            DataMd5,
            async () => await fileManager.ReadFromDiskAsync(DataPath)
        );
        
        [Obsolete("Only use GetDataBlocking in legacy code!")]
        public byte[] GetDataBlocking() => MediaCache.GetBytes(
            DataMd5,
            () => fileManager.ReadFromDisk(DataPath)
        );
    }
}