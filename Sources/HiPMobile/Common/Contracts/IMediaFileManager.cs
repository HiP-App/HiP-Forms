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

using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts
{
    public interface IMediaFileManager
    {
        /// <summary>
        /// Delete a media file returned by <see cref="WriteMediaToDiskAsync"/>.
        /// </summary>
        void DeleteFile(string filePath);
        
        /// <summary>
        /// Writes the bytes to a file on disk.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns>MD5 sum of the data and the (generated) path the file has been written to.</returns>
        Task<(string md5, string filePath)> WriteMediaToDiskAsync(byte[] bytes);
        
        /// <summary>
        /// Read a media file returned by <see cref="WriteMediaToDiskAsync"/> from disk
        /// into a byte array.
        /// </summary>
        Task<byte[]> ReadFromDiskAsync(string filePath);
        
        /// <summary>
        /// Read a media file returned by <see cref="WriteMediaToDiskAsync"/> from disk
        /// into a byte array.
        /// </summary>
        byte[] ReadFromDisk(string filePath);
    }
}