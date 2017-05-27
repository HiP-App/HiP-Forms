﻿// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
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

using System.Threading;
using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers.Contracts {
    /// <summary>
    /// Fetcher for full exhibits data
    /// </summary>
    public interface IFullExhibitDataFetcher {

        /// <summary>
        /// Gets the pages for an exhibit
        /// </summary>
        /// <param name="idForRestApi">The id for the REST API of the exhibit to be fetched</param>
        /// <param name="token">Used for cancelling the download</param>
        /// <param name="listener">Used for reporting progress</param>
        /// <returns></returns>
        Task FetchFullExhibitDataIntoDatabase(int idForRestApi, CancellationToken token, IProgressListener listener);

    }
}