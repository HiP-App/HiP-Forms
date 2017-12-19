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

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts
{
    public interface IMediaFileManager
    {
        long TotalSizeBytes { get; }

        /// <summary>
        /// Delete a media file returned by <see cref="WriteMediaToDiskAsync"/>.
        /// </summary>
        void DeleteFile(string filePath);

        /// <summary>
        /// Writes the bytes to a file on disk and deletes any file with the same restApiId that
        /// has a different timestamp.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="restApiId">The ID of the media file in the REST API.</param>
        /// <param name="timestamp">Timestamp returned by the REST API.</param>
        /// <returns>The (generated) path the file has been written to.</returns>
        Task<string> WriteMediaToDiskAsync(byte[] bytes, int restApiId, DateTimeOffset timestamp);

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

        /// <summary>
        /// Delete all media files whose IDs are not contained in the
        /// supplied list.
        /// </summary>
        /// <param name="restApiIdsToKeep"></param>
        /// <returns></returns>
        Task PruneAsync(IList<int> restApiIdsToKeep);

        /// <summary>
        /// Returns true iff a file with that restApiId exists with a matching timestamp.
        /// </summary>
        /// <param name="restApiId"></param>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        Task<bool> ContainsMedia(int restApiId, DateTimeOffset timestamp);

        /// <summary>
        /// Returns the path to the file containing the bytes for the media
        /// with the specified REST API ID.
        /// </summary>
        /// <param name="restApiId"></param>
        /// <returns></returns>
        string PathForRestApiId(int restApiId);
    }
}