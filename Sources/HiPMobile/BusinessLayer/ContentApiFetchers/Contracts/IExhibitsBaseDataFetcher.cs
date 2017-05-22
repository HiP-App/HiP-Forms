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

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers.Contracts
{
    /// <summary>
    /// Fetcher for base exhibits data
    /// </summary>
    public interface IExhibitsBaseDataFetcher
    {
        /// <summary>
        /// Preload data needed for processing the exhibits
        /// </summary>
        /// <returns></returns>
        Task<int> FetchNeededDataForExhibits();

        /// <summary>
        /// Load the images and put the exhibits into the database
        /// </summary>
        /// <param name="token"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        Task ProcessExhibits(CancellationToken token, IProgressListener listener);

        /// <summary>
        /// Indicates whether any exhibit was changed
        /// </summary>
        /// <returns></returns>
        Task<bool> AnyExhibitChanged();

    }
}