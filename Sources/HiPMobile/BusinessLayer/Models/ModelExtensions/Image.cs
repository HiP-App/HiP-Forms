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
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;
using Realms;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models
{
    public partial class Image
    {
        private readonly IImageDimension imgDimension = IoCManager.Resolve<IImageDimension>();
        private readonly IMediaFileManager fileManager = IoCManager.Resolve<IMediaFileManager>();

        public virtual async Task<byte[]> GetDataAsync() => await MediaCache.GetBytesAsync(
            DataPath,
            async () => await fileManager.ReadFromDiskAsync(DataPath)
        );

        [Obsolete("Only use GetDataBlocking in legacy code!")]
        public virtual byte[] GetDataBlocking() => MediaCache.GetBytes(
            DataPath,
            () => fileManager.ReadFromDisk(DataPath)
        );

        private int ImageWidth { get; set; }

        [Ignored]
        public int Width
        {
            get
            {
                if (ImageWidth != 0)
                {
                    return ImageWidth;
                }
                var w = imgDimension.GetDimensions(this)[0];
                using (DbManager.StartTransaction())
                {
                    ImageWidth = w;
                }
                return w;
            }
        }

        private int ImageHeight { get; set; }

        [Ignored]
        public int Height
        {
            get
            {
                if (ImageHeight != 0)
                {
                    return ImageHeight;
                }
                var h = imgDimension.GetDimensions(this)[1];
                using (DbManager.StartTransaction())
                {
                    ImageHeight = h;
                }
                return h;
            }
        }
    }
}