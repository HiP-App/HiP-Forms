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
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers.Contracts
{
    /// <summary>
    /// Fetcher for audio and images
    /// </summary>
    public interface IMediaDataFetcher
    {
        /// <summary>
        /// Fetches the media data including the real files for the given ids
        /// </summary>
        /// <param name="mediaIds"></param>
        /// <param name="token">Can be provided for canceling the action</param>
        /// <param name="progressListener">Can be provided for reporting progress on the download</param>
        Task FetchMedias(IList<int?> mediaIds, CancellationToken token, IProgressListener progressListener);

        /// <summary>
        /// Combines media data and files
        /// Must be called inside a database transaction
        /// </summary>
        /// <returns></returns>
        Task<FetchedMediaData> CombineMediasAndFiles();
    }
}