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

using System;
using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models
{
    public partial class Image
    {
        public sealed class PreparedImageLoad
        {
            private readonly string dataPath;
            private readonly int idForRestApi;
            private readonly IMediaFileManager fileManager = IoCManager.Resolve<IMediaFileManager>();

            internal PreparedImageLoad(string dataPath, int idForRestApi)
            {
                this.dataPath = dataPath;
                this.idForRestApi = idForRestApi;
            }

            public async Task<byte[]> GetDataAsync() => await MediaCache.GetBytesAsync(
                idForRestApi,
                async () => await fileManager.ReadFromDiskAsync(dataPath)
            );
        }

        private readonly IImageDimension imgDimension = IoCManager.Resolve<IImageDimension>();
        private readonly IMediaFileManager fileManager = IoCManager.Resolve<IMediaFileManager>();

        /// <summary>
        /// Get the image bytes asynchronously. Must be called from the main thread,
        /// as the <see cref="DataPath"/> is accessed and Realm can only be used from the main
        /// thread. If you need to access the data from another thread, call
        /// <see cref="PrepareImageLoad"/> on the main thread and then call 
        /// <see cref="PreparedImageLoad.GetDataAsync"/> on it.
        /// </summary>
        /// <returns></returns>
        public virtual async Task<byte[]> GetDataAsync()
        {
            var dataPath = DataPath; // DO NOT INLINE! See method documentation.
            Debug2.Assert(dataPath != null, $"dataPath is null for media with REST ID {IdForRestApi}");
            return await MediaCache.GetBytesAsync(
                IdForRestApi,
                async () => await fileManager.ReadFromDiskAsync(dataPath)
            );
        }

        [Obsolete("Only use GetDataBlocking in legacy code!")]
        public virtual byte[] GetDataBlocking() => MediaCache.GetBytes(
            IdForRestApi,
            () => fileManager.ReadFromDisk(DataPath)
        );

        /// <summary>
        /// Call this method on the main thread to avoid an Exception as described in
        /// <see cref="GetDataAsync"/>.
        /// </summary>
        /// <returns></returns>
        public virtual PreparedImageLoad PrepareImageLoad() => new PreparedImageLoad(DataPath, IdForRestApi);
    }
}