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

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers.Contracts
{
    /// <summary>
    /// Fetcher for full exhibits data
    /// </summary>
    public interface IFullExhibitDataFetcher : IFullDownloadableDataFetcher
    {
        /// <summary>
        /// Fetch exhibit data with prefetched media
        /// </summary>
        /// <param name="exhibitId">The id of the exhibit in the database</param>
        /// <param name="pagesAndMediaContainer">Container for all pages and prefetched media related to the exhibit</param>
        /// <param name="token">For cancellation</param>
        /// <param name="listener">To update the progressbar of the downloadpage</param>
        /// <returns></returns>
        Task FetchFullExhibitDataIntoDatabaseWithFetchedPagesAndMedia(string exhibitId, ExhibitPagesAndMediaContainer pagesAndMediaContainer, CancellationToken token,
                                                                      IProgressListener listener);

        /// <summary>
        /// Load pages for related exhibit and prefetch media of an exhibit for later use in a full route fetcher
        /// </summary>
        /// <param name="idForRestApi">The id for the REST API of the exhibit to be fetched</param>
        /// <returns></returns>
        Task<ExhibitPagesAndMediaContainer> FetchPagesAndMediaForExhibitFromRouteFetcher(int idForRestApi);
    }
}